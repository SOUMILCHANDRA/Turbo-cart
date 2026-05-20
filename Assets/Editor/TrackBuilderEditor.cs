using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Editor utility for TrackPiece.
/// Provides a "Rapid Track Builder" interface in the inspector, allowing developers to quickly
/// snap and chain track piece prefabs together by aligning pivots to ExitPoints.
/// </summary>
[CustomEditor(typeof(TrackPiece))]
public class TrackBuilderEditor : Editor
{
    private GameObject nextPiecePrefab;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector variables (type, exitPoint)
        base.OnInspectorGUI();

        TrackPiece currentPiece = (TrackPiece)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rapid Track Builder", EditorStyles.boldLabel);

        // Prefab assignment field
        nextPiecePrefab = (GameObject)EditorGUILayout.ObjectField("Next Piece Prefab", nextPiecePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Append Next Piece"))
        {
            if (nextPiecePrefab == null)
            {
                Debug.LogError("TrackBuilderEditor: Please assign a 'Next Piece Prefab' in the inspector before appending.");
                return;
            }

            if (currentPiece.exitPoint == null)
            {
                Debug.LogError("TrackBuilderEditor: The currently selected piece does not have an 'Exit Point' assigned.", currentPiece);
                return;
            }

            AppendPiece(currentPiece, nextPiecePrefab);
        }
    }

    /// <summary>
    /// Instantiates the prefab, aligns it to the exit point, groups it under the same parent,
    /// and selects it so the developer can chain pieces rapidly.
    /// </summary>
    private void AppendPiece(TrackPiece startPiece, GameObject prefab)
    {
        // Instantiate the object as a prefab link in the scene
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (newObj == null)
        {
            Debug.LogError("TrackBuilderEditor: Failed to instantiate prefab.");
            return;
        }

        // Register action with the undo system
        Undo.RegisterCreatedObjectUndo(newObj, "Append Track Piece");

        // Align position and rotation of the new piece with the exit point of the current piece
        // Note: Assumes the pivot of the prefab mesh is at the track's entry point
        newObj.transform.position = startPiece.exitPoint.position;
        newObj.transform.rotation = startPiece.exitPoint.rotation;

        // Group under the same parent transform
        newObj.transform.SetParent(startPiece.transform.parent);

        // Select the newly spawned piece automatically
        Selection.activeGameObject = newObj;

        Debug.Log($"TrackBuilderEditor: Successfully appended {newObj.name} to {startPiece.name}.", newObj);
    }
}
