using UnityEngine;

/// <summary>
/// Handles the drift-and-boost mechanics.
/// Intercepts KartMovement parameter settings to reduce grip and increase steering
/// when drifting, and applies a forward speed boost when the drift is released.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(KartMovement), typeof(AntiGravityPhysics))]
public class DriftSystem : MonoBehaviour
{
    private Rigidbody rb;
    private KartMovement movement;
    private AntiGravityPhysics agPhysics;
    private IKartInput kartInput;

    private bool isDrifting;
    private float driftDirection;
    private float currentBoostCharge;

    /// <summary>
    /// Returns true if the kart is currently drifting.
    /// </summary>
    public bool IsDrifting => isDrifting;

    /// <summary>
    /// The current charge amount of the drift boost (between 0.0 and maxBoostCharge).
    /// </summary>
    public float CurrentBoostCharge => currentBoostCharge;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<KartMovement>();
        agPhysics = GetComponent<AntiGravityPhysics>();
        kartInput = GetComponent<IKartInput>();
    }

    private void FixedUpdate()
    {
        if (movement.Settings == null) return;

        // Immediately cancel drift if we fly off the track
        if (!agPhysics.IsGrounded)
        {
            if (isDrifting) StopDrifting(applyBoost: false);
            return;
        }

        HandleDrifting();
    }

    /// <summary>
    /// Monitored drift entry/exit checks and boost value accretion.
    /// </summary>
    private void HandleDrifting()
    {
        bool isPressingDrift = kartInput.IsDrifting;
        float steerInput = kartInput.Steering;

        // Try to start a drift (must be holding drift key, turning, and going above minimum velocity)
        if (!isDrifting && isPressingDrift && Mathf.Abs(steerInput) > 0.2f && rb.velocity.magnitude > movement.Settings.minSpeedToDrift)
        {
            StartDrifting(steerInput);
        }

        // Handle active drift updates
        if (isDrifting)
        {
            // Exit drift if drift button is released or kart slows down too much
            if (!isPressingDrift || rb.velocity.magnitude < movement.Settings.minSpeedToDrift * 0.4f)
            {
                StopDrifting(applyBoost: true);
            }
            else
            {
                // Accumulate boost charge over time
                currentBoostCharge = Mathf.MoveTowards(
                    currentBoostCharge,
                    movement.Settings.maxBoostCharge,
                    movement.Settings.boostChargeRate * Time.fixedDeltaTime
                );
            }
        }
    }

    /// <summary>
    /// Configures grip and steering overrides for slide state.
    /// </summary>
    private void StartDrifting(float steerInput)
    {
        isDrifting = true;
        driftDirection = Mathf.Sign(steerInput);
        currentBoostCharge = 0f;

        // Override physics behaviors on KartMovement to allow sliding and sharper turning
        movement.GripOverride = movement.Settings.driftGripAmount;
        movement.SteeringMultiplierOverride = movement.Settings.driftSteeringMultiplier;
    }

    /// <summary>
    /// Resets overrides and applies accumulated boost forces.
    /// </summary>
    private void StopDrifting(bool applyBoost)
    {
        isDrifting = false;

        // Reset overrides to defaults
        movement.GripOverride = -1f;
        movement.SteeringMultiplierOverride = 1f;

        // Threshold check: drift must last long enough to count for a boost
        if (applyBoost && currentBoostCharge > 0.5f)
        {
            // Apply instant velocity boost forward in local direction
            Vector3 boostForceVector = transform.forward * (currentBoostCharge * movement.Settings.boostForce);
            rb.AddForce(boostForceVector, ForceMode.VelocityChange);
            
            Debug.Log($"Drift Boost Released! Applied: {currentBoostCharge:F2}");
        }

        currentBoostCharge = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isDrifting || movement == null || movement.Settings == null) return;

        // Visual charge progress sphere
        Gizmos.color = Color.yellow;
        float normalizedCharge = currentBoostCharge / movement.Settings.maxBoostCharge;
        Gizmos.DrawWireSphere(transform.position + transform.up * 1.5f, normalizedCharge * 0.75f);
    }
}
