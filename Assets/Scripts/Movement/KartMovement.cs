using UnityEngine;

/// <summary>
/// Handles engine acceleration, braking/reverse, and yaw steering.
/// Computes lateral forces (grip) relative to kart orientation to control sliding.
/// Interacts directly with the Rigidbody and AntiGravityPhysics components.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(AntiGravityPhysics))]
public class KartMovement : MonoBehaviour
{
    private Rigidbody rb;
    private AntiGravityPhysics agPhysics;
    private IKartInput kartInput;

    // External boost modifiers (e.g. from Boost Pads)
    private float externalBoostMultiplier = 1f;
    private float externalBoostTimer = 0f;

    /// <summary>
    /// Property indicating if the kart is currently grounded.
    /// </summary>
    public bool IsGrounded => agPhysics.IsGrounded;

    /// <summary>
    /// Accessor for current active KartSettings ScriptableObject.
    /// </summary>
    public KartSettings Settings => agPhysics.Settings;

    /// <summary>
    /// Allows external scripts (e.g., DriftSystem) to override the lateral grip value.
    /// Set to a negative value to restore default settings grip.
    /// </summary>
    [HideInInspector] public float GripOverride = -1f;

    /// <summary>
    /// Allows external scripts (e.g., DriftSystem) to scale steering sensitivity.
    /// </summary>
    [HideInInspector] public float SteeringMultiplierOverride = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agPhysics = GetComponent<AntiGravityPhysics>();
        kartInput = GetComponent<IKartInput>();
    }

    private void FixedUpdate()
    {
        if (Settings == null) return;

        UpdateExternalBoost();

        if (agPhysics.IsGrounded)
        {
            ApplyPropulsion();
            ApplySteering();
            ApplyLateralGrip();
        }
        else
        {
            // Apply standard airborne drag
            rb.drag = Settings.baseDrag;
        }
    }

    /// <summary>
    /// Processes countdown timer for active speed boosters.
    /// </summary>
    private void UpdateExternalBoost()
    {
        if (externalBoostTimer > 0f)
        {
            externalBoostTimer -= Time.fixedDeltaTime;
            if (externalBoostTimer <= 0f)
            {
                externalBoostMultiplier = 1f;
            }
        }
    }

    /// <summary>
    /// Calculates and applies forward motor forces or deceleration forces.
    /// </summary>
    private void ApplyPropulsion()
    {
        float targetAccel = Settings.acceleration;
        float targetMaxSpeed = Settings.maxSpeed;

        // Apply external boost modifiers
        if (externalBoostTimer > 0f)
        {
            targetAccel *= externalBoostMultiplier;
            targetMaxSpeed *= externalBoostMultiplier;
        }

        float throttle = kartInput.Throttle;

        if (throttle < -0.05f)
        {
            // Apply braking force opposite to the direction of current travel
            Vector3 brakeForce = -transform.forward * Settings.brakingForce;
            rb.AddForce(brakeForce, ForceMode.Acceleration);
        }
        else if (throttle > 0.05f)
        {
            // Measure current speed projection along kart's forward vector
            float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
            if (forwardSpeed < targetMaxSpeed)
            {
                Vector3 motorForce = transform.forward * throttle * targetAccel;
                rb.AddForce(motorForce, ForceMode.Acceleration);
            }
        }

        // Apply drag modifications: higher drag when coasting (zero throttle input)
        rb.drag = (Mathf.Abs(throttle) < 0.05f && rb.velocity.magnitude > 1.0f)
            ? Settings.baseDrag * 2.5f
            : Settings.baseDrag;
    }

    /// <summary>
    /// Handles yaw steering relative to the kart's local UP axis, scaled by speed.
    /// </summary>
    private void ApplySteering()
    {
        float steerInput = kartInput.Steering;
        if (Mathf.Abs(steerInput) < 0.01f) return;

        // Scale steering sensitivity so turning feels stable at high speeds and 
        // prevents pivoting in place at zero speed (arcade style).
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 12.0f);
        float turnAmount = steerInput * Settings.steeringSensitivity * speedFactor * SteeringMultiplierOverride;

        // Rotate Rigidbody orientation around local UP vector
        Quaternion yawRotation = Quaternion.AngleAxis(turnAmount, transform.up);
        rb.MoveRotation(rb.rotation * yawRotation);
    }

    /// <summary>
    /// Applies counter-lateral force (grip) to prevent karts from sliding sideways.
    /// </summary>
    private void ApplyLateralGrip()
    {
        float grip = (GripOverride >= 0f) ? GripOverride : Settings.gripAmount;
        
        // Dynamically scale grip at higher speeds to aid high-G banked cornering
        grip *= (1.0f + (rb.velocity.magnitude * Settings.speedGripFactor));

        // Isolate the lateral (sideways) component of velocity
        Vector3 lateralVelocity = Vector3.Project(rb.velocity, transform.right);

        // Apply friction force countering the side-slip direction
        rb.AddForce(-lateralVelocity * grip, ForceMode.Acceleration);
    }

    /// <summary>
    /// Public API for external pads to apply speed and acceleration modifiers.
    /// </summary>
    public void ApplyExternalBoost(float multiplier, float duration)
    {
        externalBoostMultiplier = multiplier;
        externalBoostTimer = duration;

        // Apply an immediate velocity kick forward in local space
        rb.AddForce(transform.forward * (multiplier * 15.0f), ForceMode.VelocityChange);
    }
}
