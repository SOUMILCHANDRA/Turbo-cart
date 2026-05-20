using UnityEngine;

/// <summary>
/// Trigger-based speed booster placed on the track.
/// Detects passing karts and applies temporary speed and acceleration multipliers.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class BoostPad : MonoBehaviour
{
    [Header("Boost Config")]
    [Tooltip("Velocity and acceleration multiplier applied during boost.")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [Tooltip("How long the speed boost lasts in seconds.")]
    [SerializeField] private float duration = 2.0f;

    [Header("Optional Feedback")]
    [Tooltip("Optional particle system to emit when boost is triggered.")]
    [SerializeField] private ParticleSystem boostVFX;
    [Tooltip("Optional audio source to play when boost is triggered.")]
    [SerializeField] private AudioSource boostSFX;

    private void Awake()
    {
        // Ensure collider is correctly configured as a trigger
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Resolve KartMovement script on the entering kart
        KartMovement movement = other.GetComponentInParent<KartMovement>();
        if (movement != null)
        {
            movement.ApplyExternalBoost(speedMultiplier, duration);

            if (boostVFX != null)
            {
                boostVFX.Play();
            }

            if (boostSFX != null)
            {
                boostSFX.Play();
            }

            Debug.Log($"Boost Pad activated on: {movement.gameObject.name}", this);
        }
    }
}
