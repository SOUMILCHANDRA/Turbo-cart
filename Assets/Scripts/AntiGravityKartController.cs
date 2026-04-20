using UnityEngine;

/// <summary>
/// A Rigidbody-based anti-gravity kart controller for Unity.
/// Aligns to surfaces (walls, ceilings, loops) using raycasts and applies local gravity.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AntiGravityKartController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float steeringSensitivity = 5f;
    [SerializeField] private float brakingForce = 30f;
    [SerializeField] private float dragBase = 0.5f;

    [Header("Anti-Gravity Settings")]
    [SerializeField] private float gravityForce = 30f;
    [SerializeField] private float hoverHeight = 1.0f;
    [SerializeField] private float hoverSnappiness = 20f;
    [SerializeField] private float groundCheckDistance = 3.0f;
    [SerializeField] private float rotationSmoothness = 10f;
    [SerializeField] private LayerMask trackLayer;

    [Header("Drift & Boost Settings")]
    [SerializeField] private float driftTurningMultiplier = 2.0f;
    [SerializeField] private float driftGripAmount = 2.0f; // Lower means more sliding
    [SerializeField] private float normalGripAmount = 10.0f;
    [SerializeField] private float boostChargeRate = 0.5f;
    [SerializeField] private float maxBoost = 2.0f;
    [SerializeField] private float boostForce = 100f;

    [Header("Adhesion & Grip Settings")]
    [SerializeField] private float adhesionStrength = 1.5f; // Extra gravity based on speed
    [SerializeField] private float gripSpeedFactor = 0.05f; // Extra lateral grip based on speed
    [SerializeField] private float maxRaycastDistance = 5.0f;

    [Header("Refinement Settings")]
    [SerializeField] private Transform[] groundPoints; // Positions (e.g. 4 corners) to cast rays from

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private Vector3 surfaceNormal = Vector3.up;

    // Boost State
    private bool isDrifting;
    private float currentBoost;
    private float driftDirection; 
    private float externalBoostTimer;
    private float externalBoostMultiplier = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Settings for smoother physics interactions
        rb.useGravity = false; // We use custom gravity
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.drag = dragBase;

        // If no ground points assigned, use the center transform
        if (groundPoints == null || groundPoints.Length == 0)
        {
            groundPoints = new Transform[] { transform };
        }
    }

    private void Update()
    {
        // Get user input in Update for better responsiveness
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        HandleDriftInput();
    }

    private void FixedUpdate()
    {
        HandleSurfaceDetection();
        ApplyAntiGravity();
        
        if (isGrounded)
        {
            HandleMovement();
            HandleLateralGrip();
            
            if (isDrifting)
            {
                currentBoost = Mathf.MoveTowards(currentBoost, maxBoost, boostChargeRate * Time.fixedDeltaTime);
            }
        }
        else
        {
            ApplyAirLogic();
        }

        // Apply all rotations (Alignment + Steering) in one go for stability
        UpdateOrientation();
    }

    private void HandleDriftInput()
    {
        // Start drifting
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            isDrifting = true;
            driftDirection = Mathf.Sign(horizontalInput);
            currentBoost = 0f;
        }

        // Release drift / Apply Boost
        if (Input.GetKeyUp(KeyCode.LeftShift) && isDrifting)
        {
            ApplyBoost();
            isDrifting = false;
        }
    }

    private void ApplyBoost()
    {
        if (currentBoost > 0.5f) // Minimum charge to boost
        {
            float totalBoost = currentBoost * boostForce;
            rb.AddForce(transform.forward * totalBoost, ForceMode.VelocityChange);
            Debug.Log($"Boost Released! Charge: {currentBoost:F2}");
        }
        currentBoost = 0f;
    }

    /// <summary>
    /// Detects the surface by averaging raycasts from multiple points.
    /// Uses dynamic distance to stay glued at high speeds.
    /// </summary>
    private void HandleSurfaceDetection()
    {
        Vector3 averageNormal = Vector3.zero;
        int hitCount = 0;
        float totalHitDistance = 0;

        foreach (Transform point in groundPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.position, -transform.up, out hit, maxRaycastDistance, trackLayer))
            {
                averageNormal += hit.normal;
                totalHitDistance += hit.distance;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            isGrounded = true;
            surfaceNormal = (averageNormal / hitCount).normalized;
            float averageDistance = totalHitDistance / hitCount;

            // Maintain hover height
            float heightError = hoverHeight - averageDistance;
            
            // Magnetic Snapping: Increase hover strength at speed to prevent detachment
            float speedMultiplier = 1f + (rb.velocity.magnitude * 0.02f);
            float liftForce = heightError * hoverSnappiness * speedMultiplier;
            
            float verticalVelocity = Vector3.Dot(rb.velocity, transform.up);
            liftForce -= verticalVelocity * 2f; 

            rb.AddForce(transform.up * liftForce, ForceMode.Acceleration);
        }
        else
        {
            isGrounded = false;
            isDrifting = false;
            surfaceNormal = Vector3.Lerp(surfaceNormal, Vector3.up, Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Updates the kart's rotation to align with the surface normal and respond to steering/drifting.
    /// </summary>
    private void UpdateOrientation()
    {
        // 1. Handle Steering
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 10f);
        float multiplier = isDrifting ? driftTurningMultiplier : 1.0f;
        
        float finalHorizontalInput = isDrifting ? driftDirection : horizontalInput;
        float rotationAmount = finalHorizontalInput * steeringSensitivity * speedFactor * multiplier;
        
        Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);

        // 2. Align to Surface Normal
        Vector3 combinedForward = transform.rotation * turnRotation * Vector3.forward;
        Vector3 projectedForward = Vector3.ProjectOnPlane(combinedForward, surfaceNormal);
        
        if (projectedForward.sqrMagnitude < 0.001f)
            projectedForward = Vector3.ProjectOnPlane(transform.up, surfaceNormal);

        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, surfaceNormal);
        
        // 3. Smoothly interpolate
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Cancels out lateral velocity to simulate "grip". Scales with speed for stability.
    /// </summary>
    private void HandleLateralGrip()
    {
        float currentGrip = isDrifting ? driftGripAmount : normalGripAmount;
        
        // Magnetic Adhesion: Increase grip at higher speeds to handle curves
        currentGrip *= (1f + (rb.velocity.magnitude * gripSpeedFactor));

        Vector3 lateralVelocity = Vector3.Project(rb.velocity, transform.right);
        rb.AddForce(-lateralVelocity * currentGrip, ForceMode.Acceleration);
    }

    private void ApplyAntiGravity()
    {
        // Magnetic Adhesion: Pull harder at high speed to stay on track
        float customGravityStrength = gravityForce + (rb.velocity.magnitude * adhesionStrength);
        Vector3 customGravity = -surfaceNormal * customGravityStrength;
        
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void HandleMovement()
    {
        // Apply temporary boost multiplier
        float effectiveMaxSpeed = maxSpeed;
        float effectiveAcceleration = acceleration;

        if (externalBoostTimer > 0)
        {
            effectiveMaxSpeed *= externalBoostMultiplier;
            effectiveAcceleration *= externalBoostMultiplier;
            externalBoostTimer -= Time.fixedDeltaTime;
        }

        if (rb.velocity.magnitude < effectiveMaxSpeed)
        {
            Vector3 driveForce = transform.forward * verticalInput * effectiveAcceleration;
            rb.AddForce(driveForce, ForceMode.Acceleration);
        }

        if (verticalInput == 0 && rb.velocity.magnitude > 0.1f)
        {
            rb.drag = dragBase * 2;
        }
        else
        {
            rb.drag = dragBase;
        }
    }

    /// <summary>
    /// Applies a temporary speed and acceleration boost from external sources like boost pads.
    /// </summary>
    public void ApplyExternalBoost(float multiplier, float duration)
    {
        externalBoostMultiplier = multiplier;
        externalBoostTimer = duration;
        
        // Immediate velocity burst for "kick" feel
        rb.AddForce(transform.forward * multiplier * 10f, ForceMode.VelocityChange);
    }

    private void ApplyAirLogic()
    {
        rb.drag = dragBase * 0.1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * groundCheckDistance);
        
        // Visualize boost charge
        if (isDrifting)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + transform.up * 2f, currentBoost * 0.5f);
        }
    }
}
