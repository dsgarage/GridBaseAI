using UnityEngine;
using UnityEditor;

public class AlignHeightEditor : EditorWindow {
    private float targetHeight = 0f; // 高さの目標値

    [MenuItem("Tools/Align Height")] // メニューに追加
    public static void ShowWindow() {
        GetWindow<AlignHeightEditor>("Align Height");
    }

    void OnGUI() {
        GUILayout.Label("Align Selected Objects Height", EditorStyles.boldLabel);

        targetHeight = EditorGUILayout.FloatField("Target Height", targetHeight);

        if (GUILayout.Button("Align Height")) {
            AlignHeight();
        }
    }

    void AlignHeight() {
        foreach (GameObject obj in Selection.gameObjects) {
            Vector3 position = obj.transform.position;
            position.y = targetHeight; // Y軸の座標を目標値に設定
            obj.transform.position = position;
        }
    }
}