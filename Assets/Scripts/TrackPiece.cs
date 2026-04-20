using UnityEngine;

/// <summary>
/// A component to manage modular track pieces for anti-gravity racing.
/// Handles alignment points for snapping pieces together in the editor.
/// </summary>
public class TrackPiece : MonoBehaviour
{
    public enum PieceType { Straight, Curve, Loop, WallTransition, Jump }

    [Header("Piece Info")]
    public PieceType type;
    
    [Header("Alignment Points")]
    [Tooltip("Where the next piece should connect to this one.")]
    public Transform exitPoint;

    private void OnDrawGizmos()
    {
        if (exitPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(exitPoint.position, 0.5f);
            
            // Draw a forward arrow to show the exit direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(exitPoint.position, exitPoint.forward * 2f);
            
            // Draw an up arrow to show the exit orientation (important for anti-gravity)
            Gizmos.color = Color.green;
            Gizmos.DrawRay(exitPoint.position, exitPoint.up * 1f);
        }
    }
}
