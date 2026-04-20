using UnityEngine;

/// <summary>
/// A smooth third-person camera system specifically designed for anti-gravity karts.
/// Tracks position and orientation while aligning to the kart's local up vector
/// to handle loops, walls, and ceiling driving without flipping.
/// </summary>
public class AntiGravityCamera : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform target; // The kart or a follow point on the kart
    
    [Header("Positioning")]
    [SerializeField] private float distance = 6.0f;
    [SerializeField] private float height = 2.0f;
    [SerializeField] private float smoothSpeed = 10.0f;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSmoothness = 5.0f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(10f, 0f, 0f); // Slight pitch offset for better view

    private Vector3 currentVelocity;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("AntiGravityCamera: No target assigned. Please assign the Kart Transform.");
            return;
        }

        // Snap to target immediately on start
        UpdateCamera(1f); 
    }

    private void LateUpdate()
    {
        if (target == null) return;

        UpdateCamera(Time.deltaTime);
    }

    /// <summary>
    /// Handles camera movement and rotation logic.
    /// </summary>
    /// <param name="deltaTime">Time step for smoothing.</param>
    private void UpdateCamera(float deltaTime)
    {
        // 1. Calculate the ideal position behind and above the kart relative to its orientation
        Vector3 desiredPosition = target.position 
                                 - (target.forward * distance) 
                                 + (target.up * height);

        // 2. Smoothly move to that position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * deltaTime);

        // 3. Calculate the target rotation
        // The camera should always look at the kart while aligning its "up" with the kart's "up"
        Vector3 directionToTarget = target.position - transform.position;
        
        // If the camera is too close to the target, avoid zero-magnitude vector logic
        if (directionToTarget.sqrMagnitude < 0.001f)
            directionToTarget = target.forward;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, target.up);
        
        // Apply a slight pitch offset if desired (look slightly down at the kart)
        targetRotation *= Quaternion.Euler(rotationOffset);

        // 4. Smoothly rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * deltaTime);
    }

    // Context menu to snap camera in editor
    [ContextMenu("Snap to Target")]
    public void SnapToTarget()
    {
        if (target != null) UpdateCamera(1.0f);
    }
}
