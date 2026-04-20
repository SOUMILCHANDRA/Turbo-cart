using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackPiece))]
public class TrackBuilderEditor : Editor
{
    private GameObject nextPiecePrefab;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrackPiece currentPiece = (TrackPiece)target;
        EditorGUILayout.Space();
        nextPiecePrefab = (GameObject)EditorGUILayout.ObjectField("Next Piece Prefab", nextPiecePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Append Next Piece"))
        {
            if (nextPiecePrefab == null || currentPiece.exitPoint == null) return;
            AppendPiece(currentPiece, nextPiecePrefab);
        }
    }

    private void AppendPiece(TrackPiece startPiece, GameObject prefab)
    {
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(newObj, "Append Track Piece");
        newObj.transform.SetParent(startPiece.transform.parent);
        newObj.transform.position = startPiece.exitPoint.position;
        newObj.transform.rotation = startPiece.exitPoint.rotation;
        Selection.activeGameObject = newObj;
    }
}
