using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom inspector for TrackPiece to allow rapid track building via snapping.
/// </summary>
[CustomEditor(typeof(TrackPiece))]
public class TrackBuilderEditor : Editor
{
    private GameObject nextPiecePrefab;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TrackPiece currentPiece = (TrackPiece)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rapid Builder", EditorStyles.boldLabel);

        nextPiecePrefab = (GameObject)EditorGUILayout.ObjectField("Next Piece Prefab", nextPiecePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Append Next Piece"))
        {
            if (nextPiecePrefab == null)
            {
                Debug.LogError("TrackBuilder: Please assign a prefab first!");
                return;
            }

            if (currentPiece.exitPoint == null)
            {
                Debug.LogError("TrackBuilder: Current piece has no Exit Point assigned!");
                return;
            }

            AppendPiece(currentPiece, nextPiecePrefab);
        }
    }

    private void AppendPiece(TrackPiece startPiece, GameObject prefab)
    {
        // Instantiate the new piece
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(newObj, "Append Track Piece");

        // Set parent for organization
        newObj.transform.SetParent(startPiece.transform.parent);

        // Align the new piece's origin to the current piece's exit point
        // Note: This assumes the prefab's pivot is at its entrance point
        newObj.transform.position = startPiece.exitPoint.position;
        newObj.transform.rotation = startPiece.exitPoint.rotation;

        // Select the new piece to continue building
        Selection.activeGameObject = newObj;
        
        Debug.Log($"Appended {newObj.name} to {startPiece.name}");
    }
}
