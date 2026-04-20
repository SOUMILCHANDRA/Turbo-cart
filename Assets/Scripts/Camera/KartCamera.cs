using UnityEngine;

public class KartCamera : MonoBehaviour
{
    [SerializeField] private KartController target;
    [SerializeField] private float distance = 6.0f;
    [SerializeField] private float height = 2.0f;
    [SerializeField] private float smoothSpeed = 10.0f;
    [SerializeField] private float rotationSmoothness = 5.0f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(10f, 0f, 0f);

    private void LateUpdate()
    {
        if (target == null) return;

        // Position
        Vector3 desiredPos = target.transform.position 
                           - (target.transform.forward * distance) 
                           + (target.transform.up * height);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // Rotation
        Vector3 dir = target.transform.position - transform.position;
        if (dir.sqrMagnitude < 0.001f) dir = target.transform.forward;

        Quaternion targetRot = Quaternion.LookRotation(dir, target.transform.up);
        targetRot *= Quaternion.Euler(rotationOffset);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmoothness * Time.deltaTime);
    }
}
