using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    // SeekerとTargetのTransformを設定します
    public Transform seeker, target;
    // 移動速度と停止時間を設定します
    public float moveSpeed = 1f; // 1秒間に1ユニット移動
    public float stopDuration = 0.2f; // 停止時間（秒）
    // グリッドの参照
    Grid grid;
    // 経路を保持するリスト
    List<Node> path;

    // 距離計算のタイプを定義する列挙型
    public enum DistanceType { Euclidean, Manhattan }
    // デフォルトの距離タイプをユークリッド距離に設定
    public DistanceType distanceType = DistanceType.Euclidean;

    // グリッドコンポーネントの取得
    void Awake() {
        grid = GetComponent<Grid>();
        if (grid == null) {
            Debug.LogError("Grid component not found on the same GameObject.");
        }
    }

    // 毎フレーム経路探索を実行
    void Update() {
        if (grid != null && seeker != null && target != null) {
            Debug.Log("Seeker Position: " + seeker.position);
            Debug.Log("Target Position: " + target.position);
            FindPath(seeker.position, target.position);
            if (path != null) {
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
            }
        } else {
            if (seeker == null) {
                Debug.LogError("Seeker Transform is null.");
            }

            if (target == null) {
                Debug.LogError("Target Transform is null.");
            }
        }
    }

    // 経路探索を行うメソッド
    void FindPath(Vector3 startPos, Vector3 targetPos) {
        // スタートノードとターゲットノードを取得
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null) {
            Debug.LogError("Start or target node is null. Ensure grid is initialized correctly.");
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode, distanceType)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    // 経路を再構築するメソッド
    void RetracePath(Node startNode, Node endNode) {
        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

        // 経路のデバッグログを追加
        foreach (Node node in path) {
            Debug.Log("Path Node: (" + node.gridX + ", " + node.gridY + ")");
        }
    }

    // 距離を計算するメソッド
    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceType == DistanceType.Euclidean) {
            // ユークリッド距離の計算
            return Mathf.RoundToInt(Mathf.Sqrt(dstX * dstX + dstY * dstY));
        } else if (distanceType == DistanceType.Manhattan) {
            // マンハッタン距離の計算
            // 斜め移動を許容しつつ、縦移動と横移動を組み合わせる
            return 10 * (dstX + dstY) + (14 - 2 * 10) * Mathf.Min(dstX, dstY);
        }
        return 0;
    }

    // 経路に沿ってシーカーを移動させるコルーチン
    IEnumerator FollowPath() {
        foreach (Node node in path) {
            Vector3 startPosition = seeker.position;
            Vector3 endPosition = node.worldPosition;
            float journeyLength = Vector3.Distance(startPosition, endPosition);
            float journeyDuration = journeyLength / moveSpeed; // 移動速度に応じて移動時間を調整

            float startTime = Time.time;

            while (Time.time - startTime < journeyDuration) {
                float fractionOfJourney = (Time.time - startTime) / journeyDuration;
                seeker.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

                // デバッグログを追加して座標情報を確認
                Debug.Log("Seeker Position: " + seeker.position);
                Debug.Log("Moving to: " + endPosition);

                yield return null;
            }

            // 最終位置を強制的に設定して、正確に到達する
            seeker.position = endPosition;

            // 停止時間
            Debug.Log("Stopping for " + stopDuration + " seconds");
            yield return new WaitForSeconds(stopDuration);
        }
    }
}
