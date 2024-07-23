using UnityEngine;

public class Node {
    public bool walkable; // このノードが歩行可能かどうかを示すフラグ
    public Vector3 worldPosition; // ノードのワールド空間における位置
    public int gridX; // グリッド内のX座標
    public int gridY; // グリッド内のY座標
    public int gCost; // スタートノードからこのノードまでの移動コスト
    public int hCost; // このノードからゴールノードまでの推定コスト（ヒューリスティック）
    public Node parent; // パスを再構築するための親ノード
    public Color nodeColor; // ノードの色

    // コンストラクタ
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, Color _color) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        nodeColor = _color; // ノードの色を設定
    }

    // fCostはgCostとhCostの合計。経路探索アルゴリズムで使用される
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    // ノードの色を設定するメソッド
    public void SetColor(Color color) {
        nodeColor = color;
    }
}