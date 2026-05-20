using UnityEngine;

/// <summary>
/// Defines a modular track segment's geometry and snap/alignment anchors.
/// Used in combination with TrackBuilderEditor to snap karts-compatible track pieces together.
/// </summary>
public class TrackPiece : MonoBehaviour
{
    public enum PieceType { Straight, Curve, Loop, WallTransition, Jump, Corkscrew }

    [Header("Track Segment Details")]
    [Tooltip("Category of track shape represented by this object.")]
    public PieceType type;

    [Header("Connection Anchors")]
    [Tooltip("The pivot point indicating where the entrance of the NEXT track piece should snap.")]
    public Transform exitPoint;

    private void OnDrawGizmos()
    {
        if (exitPoint != null)
        {
            // Draw sphere at snap junction
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(exitPoint.position, 0.4f);

            // Draw Forward Vector (Blue) representing flow of the track
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(exitPoint.position, exitPoint.forward * 1.5f);

            // Draw Up Vector (Green) representing gravity normal alignment
            Gizmos.color = Color.green;
            Gizmos.DrawRay(exitPoint.position, exitPoint.up * 1.0f);
        }
    }
}
