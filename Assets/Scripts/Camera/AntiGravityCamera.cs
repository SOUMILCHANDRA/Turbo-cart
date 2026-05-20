using UnityEngine;

/// <summary>
/// A specialized follow-camera designed for anti-gravity racing.
/// Aligns its UP vector with the target kart's local UP vector to allow 
/// seamless 360-degree loops and vertical drives without flipping.
/// Runs in LateUpdate to avoid trailing jitter.
/// </summary>
public class AntiGravityCamera : MonoBehaviour
{
    [Header("Target Tracking")]
    [Tooltip("The kart transform to follow.")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [Tooltip("Ideal horizontal follow distance from the kart.")]
    [SerializeField] private float distance = 7.0f;
    [Tooltip("Ideal vertical offset height above the kart.")]
    [SerializeField] private float height = 2.0f;
    [Tooltip("Smoothing factor for position tracking.")]
    [SerializeField] private float positionSmoothness = 10.0f;

    [Header("Rotation Settings")]
    [Tooltip("Smoothing factor for matching orientation.")]
    [SerializeField] private float rotationSmoothness = 5.0f;
    [Tooltip("Angle offset to tilt the camera downward slightly.")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(8.0f, 0f, 0f);

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculate ideal camera position relative to target's local orientation
        Vector3 targetForward = target.forward;
        Vector3 targetUp = target.up;

        Vector3 desiredPosition = target.position 
                                 - (targetForward * distance) 
                                 + (targetUp * height);

        // 2. Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothness * Time.deltaTime);

        // 3. Compute target rotation looking at the kart's center
        // Target offset is tilted upward relative to the kart's local frame
        Vector3 centerOffset = target.rotation * Vector3.up * (height * 0.4f);
        Vector3 lookDirection = (target.position + centerOffset) - transform.position;

        if (lookDirection.sqrMagnitude < 0.001f)
        {
            lookDirection = targetForward;
        }

        // Create target rotation using the target kart's local UP direction as the roll reference
        // This ensures the camera rotates with the kart in loops, avoiding gimbal locks
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, targetUp);

        // Apply pitch adjustment (look down at kart)
        targetRotation *= Quaternion.Euler(rotationOffset);

        // 4. Smoothly interpolate rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }
}
