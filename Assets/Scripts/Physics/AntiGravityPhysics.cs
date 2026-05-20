using UnityEngine;

/// <summary>
/// Handles anti-gravity surface detection, magnetic adhesion gravity, 
/// spring-damper hover suspension, and smooth track normal alignment.
/// Runs entirely in FixedUpdate to ensure physics stability.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AntiGravityPhysics : MonoBehaviour
{
    [Header("Configuration Reference")]
    [SerializeField] private KartSettings settings;

    [Header("Suspension Anchors")]
    [Tooltip("Transforms at the corners of the kart from which ground checks are cast.")]
    [SerializeField] private Transform[] groundPoints;

    private Rigidbody rb;

    /// <summary>
    /// Gets the current Settings configuration.
    /// </summary>
    public KartSettings Settings => settings;

    /// <summary>
    /// Returns true if at least one suspension point detects the track.
    /// </summary>
    public bool IsGrounded { get; private set; }

    /// <summary>
    /// The current calculated track surface normal. Defaults to Vector3.up in mid-air.
    /// </summary>
    public Vector3 SurfaceNormal { get; private set; } = Vector3.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Rigidbody setup optimization for custom anti-gravity physics
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (groundPoints == null || groundPoints.Length == 0)
        {
            groundPoints = new Transform[] { transform };
        }
    }

    private void FixedUpdate()
    {
        if (settings == null)
        {
            Debug.LogWarning("AntiGravityPhysics: KartSettings is not assigned!", this);
            return;
        }

        HandleSurfaceDetection();
        ApplyAntiGravity();
        AlignOrientation();
    }

    /// <summary>
    /// Raycasts down from suspension points, averaging track normal and calculating spring-damper forces.
    /// </summary>
    private void HandleSurfaceDetection()
    {
        Vector3 averageNormal = Vector3.zero;
        float totalDistance = 0f;
        int hitCount = 0;

        foreach (Transform point in groundPoints)
        {
            if (point == null) continue;

            // Cast ray down along the local negative UP vector of the kart
            if (Physics.Raycast(point.position, -transform.up, out RaycastHit hit, settings.maxRaycastDistance, settings.trackLayer))
            {
                averageNormal += hit.normal;
                totalDistance += hit.distance;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            IsGrounded = true;
            SurfaceNormal = (averageNormal / hitCount).normalized;

            float avgDistance = totalDistance / hitCount;

            // Spring Force: F = k * x (error distance)
            float compressionError = settings.hoverHeight - avgDistance;
            float springForce = compressionError * settings.hoverSnappiness;

            // Damper Force: F = -d * v (velocity projected on local up)
            float localUpVelocity = Vector3.Dot(rb.velocity, transform.up);
            float damperForce = -localUpVelocity * settings.dampening;

            float suspensionForce = springForce + damperForce;

            // Boost hover stiffness slightly at higher speeds to prevent bottoming out on loops/curves
            float dynamicSpeedFactor = 1f + (rb.velocity.magnitude * 0.015f);
            suspensionForce *= dynamicSpeedFactor;

            // Apply suspension force upwards relative to the kart's current orientation
            rb.AddForce(transform.up * suspensionForce, ForceMode.Acceleration);
        }
        else
        {
            IsGrounded = false;
            // Slowly return normal toward Vector3.up in mid-air
            SurfaceNormal = Vector3.Lerp(SurfaceNormal, Vector3.up, Time.fixedDeltaTime * 2.0f);
        }
    }

    /// <summary>
    /// Applies downforce in the direction of the surface normal, scaling with kart velocity.
    /// </summary>
    private void ApplyAntiGravity()
    {
        // Magnetic adhesion: dynamic downforce ensuring kart sticks to walls/ceilings
        float gravityMagnitude = settings.baseGravity + (rb.velocity.magnitude * settings.speedAdhesionFactor);
        rb.AddForce(-SurfaceNormal * gravityMagnitude, ForceMode.Acceleration);
    }

    /// <summary>
    /// Projects forward vector onto surface normal plane and interpolates the body's rotation.
    /// </summary>
    private void AlignOrientation()
    {
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, SurfaceNormal);

        // Fallback for extreme vertical transitions to avoid division by zero or NaN
        if (projectedForward.sqrMagnitude < 0.001f)
        {
            projectedForward = Vector3.ProjectOnPlane(transform.up, SurfaceNormal);
        }

        Quaternion targetRotation = Quaternion.LookRotation(projectedForward.normalized, SurfaceNormal);
        
        // Smoothly interpolate rotation of the Rigidbody
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, settings.rotationSmoothness * Time.fixedDeltaTime));
    }

    private void OnDrawGizmosSelected()
    {
        if (groundPoints == null || settings == null) return;

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        foreach (Transform point in groundPoints)
        {
            if (point == null) continue;
            Gizmos.DrawRay(point.position, -transform.up * settings.maxRaycastDistance);
            Gizmos.DrawWireSphere(point.position, 0.15f);
        }

        // Draw calculated normal
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, SurfaceNormal * 2.0f);
    }
}
