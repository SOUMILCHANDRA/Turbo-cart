using UnityEngine;

/// <summary>
/// A reusable boost pad script for anti-gravity racing.
/// Detects karts, applies a temporary speed boost, and triggers visual feedback.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BoostPad : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float duration = 2.0f;
    
    [Header("Feedback")]
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private AudioClip boostSound;
    [SerializeField] private Color padActiveColor = Color.cyan;

    private void Awake()
    {
        // Ensure the collider is set to trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is a kart
        AntiGravityKartController kart = other.GetComponentInParent<AntiGravityKartController>();
        
        if (kart != null)
        {
            ApplyBoost(kart);
        }
    }

    private void ApplyBoost(AntiGravityKartController kart)
    {
        // Apply the logic to the kart
        kart.ApplyExternalBoost(speedMultiplier, duration);
        
        // Visual/Audio Feedback
        if (boostParticles != null)
        {
            boostParticles.Play();
        }

        if (boostSound != null)
        {
            // Play sound at pad position
            AudioSource.PlayClipAtPoint(boostSound, transform.position);
        }

        Debug.Log($"Kart {kart.name} hit Boost Pad!");
    }

    private void OnDrawGizmos()
    {
        // Helper visualization in editor
        Gizmos.color = padActiveColor;
        Matrix4x4 oldRotation = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldRotation;
    }
}
