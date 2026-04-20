using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public Transform exitPoint;

    private void OnDrawGizmos()
    {
        if (exitPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(exitPoint.position, 0.5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(exitPoint.position, exitPoint.forward * 2f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(exitPoint.position, exitPoint.up * 1f);
        }
    }
}
