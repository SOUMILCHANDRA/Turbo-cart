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

    [Header("Refinement Settings")]
    [SerializeField] private Transform[] groundPoints; // Positions (e.g. 4 corners) to cast rays from

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private Vector3 surfaceNormal = Vector3.up;

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
    }

    private void FixedUpdate()
    {
        HandleSurfaceDetection();
        ApplyAntiGravity();
        
        if (isGrounded)
        {
            HandleMovement();
            // Steering is now part of the rotation calculation
        }
        else
        {
            ApplyAirLogic();
        }

        // Apply all rotations (Alignment + Steering) in one go for stability
        UpdateOrientation();
    }

    /// <summary>
    /// Detects the surface by averaging raycasts from multiple points.
    /// This prevents jitter when passing over edges or small bumps.
    /// </summary>
    private void HandleSurfaceDetection()
    {
        Vector3 averageNormal = Vector3.zero;
        int hitCount = 0;
        float totalHitDistance = 0;

        foreach (Transform point in groundPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.position, -transform.up, out hit, groundCheckDistance, trackLayer))
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

            // Maintain hover height using a spring-like force
            float heightError = hoverHeight - averageDistance;
            float liftForce = heightError * hoverSnappiness;
            
            // Dampen vertical velocity relative to local up
            float verticalVelocity = Vector3.Dot(rb.velocity, transform.up);
            liftForce -= verticalVelocity * 2f; 

            rb.AddForce(transform.up * liftForce, ForceMode.Acceleration);
        }
        else
        {
            isGrounded = false;
            // Gradually tilt back to "up" if in the air
            surfaceNormal = Vector3.Lerp(surfaceNormal, Vector3.up, Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Updates the kart's rotation to align with the surface normal and respond to steering.
    /// </summary>
    private void UpdateOrientation()
    {
        // 1. Handle Steering (rotate around local UP axis)
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 10f);
        float rotationAmount = horizontalInput * steeringSensitivity * speedFactor;
        Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);

        // 2. Align to Surface Normal
        // We first apply the steering to our current forward, then project onto surface
        Vector3 combinedForward = transform.rotation * turnRotation * Vector3.forward;
        Vector3 projectedForward = Vector3.ProjectOnPlane(combinedForward, surfaceNormal);
        
        if (projectedForward.sqrMagnitude < 0.001f)
            projectedForward = Vector3.ProjectOnPlane(transform.up, surfaceNormal);

        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, surfaceNormal);
        
        // 3. Smoothly interpolate to the target
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Applies gravity towards the surface normal.
    /// </summary>
    private void ApplyAntiGravity()
    {
        // Gravity is applied in the opposite direction of the surface normal
        Vector3 customGravity = -surfaceNormal * gravityForce;
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    private void HandleMovement()
    {
        // Limit speed
        if (rb.velocity.magnitude < maxSpeed)
        {
            // Apply forward/backward force
            Vector3 driveForce = transform.forward * verticalInput * acceleration;
            rb.AddForce(driveForce, ForceMode.Acceleration);
        }

        // Apply braking
        if (verticalInput == 0 && rb.velocity.magnitude > 0.1f)
        {
            rb.drag = dragBase * 2; // Increase drag when not accelerating
        }
        else
        {
            rb.drag = dragBase;
        }
    }

    private void ApplyAirLogic()
    {
        // In the air, we might want to allow some air control or just fall
        // For anti-gravity games, we usually want to search for the track more aggressively
        rb.drag = dragBase * 0.1f; // Less drag in air
    }

    private void OnDrawGizmos()
    {
        // Debug visualization for the raycast
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * groundCheckDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, surfaceNormal * 2f);
    }
}
