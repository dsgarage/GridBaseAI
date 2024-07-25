# GridBaseAI

## 経路探索A*(A Star)アルゴリズム

このプロジェクトは、Unityで2D経路探査AIを実現するためのデモになります。
Unityで標準搭載されているNavMeshではなく、2Dで使える経路探査AIという切り口で実装しています。

NavMeshが3Dの頂点を辿って最短ルートを導く機能であるのに対し、ノードベース(グリッド)に対して最短ルートを導き出すという機能です。
とても軽く、ルート検索だけであればCPUに負荷がかかりません。

## A*アルゴリズムとは

A*アルゴリズムは、**最短経路問題**を解決するために広く使用される検索アルゴリズムです。特に、グラフやグリッドのような構造内で効率的に最適な経路を見つけるために設計されています。A*は、ダイクストラ法（Dijkstra's Algorithm）とグリーディー法（Greedy Best-First Search）の利点を組み合わせたもので、次の要素を持ちます：

1. **g値**（g(n)）: スタートノードから現在のノードまでの実際のコスト。
2. **h値**（h(n)）: 現在のノードからゴールノードまでの推定コスト（ヒューリスティック関数）。
3. **f値**（f(n)）: g値とh値の合計（f(n) = g(n) + h(n)）。A*は、f値が最も低いノードを優先して探索します。

### アルゴリズムの動作

A*アルゴリズムは以下の手順で動作します：

1. **初期化**: スタートノードをオープンリストに追加し、そのg値を0に設定、f値をヒューリスティックに基づいて設定します。
2. **探索**: オープンリストからf値が最も低いノードを取り出し、ターゲットノードかどうかをチェックします。
3. **ノードの展開**: 現在のノードの隣接ノードを評価し、それらのノードのg値、h値、f値を計算します。
4. **リストの更新**: 隣接ノードがオープンリストにない場合は追加し、既に存在する場合はより低いg値が得られるかを確認し、必要に応じて更新します。
5. **反復**: ターゲットノードがオープンリストに追加されるか、オープンリストが空になるまで探索を繰り返します。
6. **経路の再構築**: ターゲットノードに到達した場合は、ノードの親情報を辿って経路を再構築します。

### 特徴と利点

- **最適解**: A*は適切なヒューリスティック関数を用いることで、最適な経路を保証します。
- **効率性**: 探索スペースを減らし、効率的に経路を見つけることができます。
- **汎用性**: さまざまなヒューリスティック関数を使用することで、異なるタイプの問題に適用可能です。

### まとめ

A*はアルゴリズムとしての分類が正しく、経路探索の分野で非常に重要な役割を果たしています。その効率性と最適性から、ゲームAI、ロボティクス、地図アプリケーションなど多くの分野で利用されています。

### 参考文献

- [A* Search Algorithm](https://en.wikipedia.org/wiki/A*_search_algorithm)
- [Introduction to the A* Algorithm](https://www.redblobgames.com/pathfinding/a-star/introduction.html)
- [A* Pathfinding for Beginners](https://www.policyalmanac.org/games/aStarTutorial.htm)