using UnityEngine;
using UnityEditor;

public class SnapToNodeEditor : EditorWindow {
    private Grid grid;
    private float nodeHeight = 0f; // ノードの高さ

    [MenuItem("Tools/Snap to Node")] // メニューに追加
    public static void ShowWindow() {
        GetWindow<SnapToNodeEditor>("Snap to Node");
    }

    void OnGUI() {
        GUILayout.Label("Snap Selected Objects to Nearest Node", EditorStyles.boldLabel);

        grid = (Grid)EditorGUILayout.ObjectField("Grid", grid, typeof(Grid), true);

        nodeHeight = EditorGUILayout.FloatField("Node Height", nodeHeight);

        if (GUILayout.Button("Snap to Node")) {
            SnapToNode();
        }
    }

    void SnapToNode() {
        if (grid == null) {
            Debug.LogError("Grid not assigned.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects) {
            Node closestNode = GetClosestNode(obj.transform.position);
            if (closestNode != null) {
                Vector3 newPosition = closestNode.worldPosition;
                newPosition.y = nodeHeight; // ノードの高さに揃える
                obj.transform.position = newPosition;
            }
        }
    }

    Node GetClosestNode(Vector3 position) {
        Node closestNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = position;

        foreach (Node node in grid.GetAllNodes()) {
            Vector3 directionToTarget = node.worldPosition - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                closestNode = node;
            }
        }
        return closestNode;
    }
}