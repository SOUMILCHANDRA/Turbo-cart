using UnityEngine;

/// <summary>
/// The main coordinator for the Kart system. Ties together physics, movement, and input.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class KartController : MonoBehaviour
{
    [header("Configuration")]
    [SerializeField] private KartSettings settings;
    [SerializeField] private Transform[] groundPoints;

    // Components
    public Rigidbody Rb { get; private set; }
    public IKartInput InputSource { get; private set; }
    public KartSettings Settings => settings;
    public Transform[] GroundPoints => groundPoints;

    // State
    public bool IsGrounded { get; set; }
    public Vector3 SurfaceNormal { get; set; } = Vector3.up;
    public bool IsDrifting { get; set; }
    public float DriftDirection { get; set; }
    public float CurrentBoostCharge { get; set; }

    // External modifiers
    public float ExternalBoostTimer { get; set; }
    public float ExternalBoostMultiplier { get; set; } = 1.0f;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        InputSource = GetComponent<IKartInput>();
        
        // Setup Rigidbody
        Rb.useGravity = false;
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
        Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Rb.drag = settings.dragBase;

        if (groundPoints == null || groundPoints.Length == 0)
            groundPoints = new Transform[] { transform };
    }

    public void ApplyExternalBoost(float multiplier, float duration)
    {
        ExternalBoostMultiplier = multiplier;
        ExternalBoostTimer = duration;
        Rb.AddForce(transform.forward * multiplier * 10f, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        // Physics update order
        KartPhysicsProcessor.ProcessSurfaceDetection(this);
        KartPhysicsProcessor.ApplyAntiGravity(this);
        
        if (IsGrounded)
        {
            KartMovementProcessor.ProcessMovement(this);
            KartMovementProcessor.ProcessDrift(this);
        }
        
        KartPhysicsProcessor.UpdateOrientation(this);
    }
}
