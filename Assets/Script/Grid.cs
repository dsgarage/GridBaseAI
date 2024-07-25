using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public bool displayGridGizmos; // グリッドのGizmos表示を制御するフラグ
    public LayerMask unwalkableMask; // 障害物を示すレイヤーマスク
    public Vector2 gridWorldSize; // グリッドのワールド空間でのサイズ
    public float nodeRadius; // ノードの半径
    public Color defaultWalkableColor = Color.white; // 歩行可能ノードのデフォルト色
    public Color defaultUnwalkableColor = Color.red; // 歩行不可能ノードのデフォルト色
    public Color pathNodeColor = Color.yellow; // 経路ノードの色

    Node[,] grid; // ノードの2次元配列
    public List<Node> path; // 経路を保持するリスト

    float nodeDiameter; // ノードの直径
    int gridSizeX, gridSizeY; // グリッドのX軸とY軸のサイズ

    void Awake() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        if (gridSizeX <= 0 || gridSizeY <= 0) {
            Debug.LogError("Grid size calculation is invalid. Check gridWorldSize and nodeRadius.");
            return;
        }

        CreateGrid(); // グリッドを生成
    }

    // グリッドを生成するメソッド
    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY]; // グリッドの2次元配列を初期化
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); // 障害物があるかどうかをチェック
                Color nodeColor = walkable ? defaultWalkableColor : defaultUnwalkableColor; // ノードの色を決定
                grid[x, y] = new Node(walkable, worldPoint, x, y, nodeColor); // ノードを作成してグリッドに追加
            }
        }
    }

    // 指定した座標のノードの色を設定するメソッド
    public void SetNodeColor(int x, int y, Color color) {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY) {
            grid[x, y].SetColor(color);
        } else {
            Debug.LogError("Invalid grid coordinates.");
        }
    }

    // ワールド座標から対応するノードを取得するメソッド
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        if (grid == null) {
            Debug.LogError("Grid is not initialized. Ensure CreateGrid() is called.");
            return null;
        }

        Debug.Log("NodeFromWorldPoint: (" + x + ", " + y + ")");
        return grid[x, y];
    }

    // 指定したノードの近隣ノードを取得するメソッド
    public List<Node> GetNeighbours(Node node, Pathfinding.DistanceType distanceType) {
        List<Node> neighbours = new List<Node>();

        int[,] directions;
        if (distanceType == Pathfinding.DistanceType.Manhattan) {
            directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; // 上、右、下、左
        } else {
            directions = new int[,] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } }; // 8方向
        }

        for (int i = 0; i < directions.GetLength(0); i++) {
            int checkX = node.gridX + directions[i, 0];
            int checkY = node.gridY + directions[i, 1];

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    // Gizmosを使用してグリッドを視覚的に表示するメソッド
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos) {
            foreach (Node n in grid) {
                Gizmos.color = n.nodeColor; // ノードの色を使用
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }

        if (path != null) {
            foreach (Node n in path) {
                Gizmos.color = pathNodeColor; // 経路ノードの色を設定
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
    
    // すべてのノードを取得するメソッド
    public List<Node> GetAllNodes() {
        List<Node> allNodes = new List<Node>();
        foreach (Node node in grid) {
            allNodes.Add(node);
        }
        return allNodes;
    }
}
