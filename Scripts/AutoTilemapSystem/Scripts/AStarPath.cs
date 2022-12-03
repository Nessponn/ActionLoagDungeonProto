using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class cellInfo
{
    public Vector3 pos { get; set; }        // 対象の位置情報
    public float cost { get; set; }         // 実コスト(今まで何歩歩いたか)
    public float heuristic { get; set; }    // 推定コスト(ゴールまでの距離)
    public float sumConst { get; set; }     // 総コスト = 実コスト + 推定コスト
    public Vector3 parent { get; set; }     // 親セルの位置情報
    public bool isOpen { get; set; }        // 調査対象となっているかどうか
}

public class AStarPath : MonoBehaviour
{
    public Tilemap map;                     // 移動範囲
    public TileBase replaceTile;            // 移動線上に位置するタイルの色を代える
    public GameObject player;               // プレイヤーのゲームオブジェクト
    public GameObject enemy;                // 敵のゲームオブジェクト
    private List<cellInfo> cellInfoList;    // 調査セルを記憶しておくリスト
    private Vector3 goal;                   // ゴールの位置情報
    private bool exitFlg;                   // 処理が終了したかどうか

    // Start is called before the first frame update
    void Start()
    {
        cellInfoList = new List<cellInfo>();

        astarSearchPathFinding();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// AStarアルゴリズムです。
    /// </summary>
    public void astarSearchPathFinding()
    {
        // ゴールはプレイヤーの位置情報
        goal = player.transform.position;

        // スタートの情報を設定する(スタートは敵)
        cellInfo start = new cellInfo();
        start.pos = enemy.transform.position;
        start.cost = 0;
        start.heuristic = Vector2.Distance(enemy.transform.position, goal);
        start.sumConst = start.cost + start.heuristic;
        start.parent = new Vector3(-9999, -9999, 0);    // スタート時の親の位置はありえない値にしておきます
        start.isOpen = true;
        cellInfoList.Add(start);

        exitFlg = false;

        //オープンが存在する限りループ
        //調査対象として、含まれているマス目がある限り、それが終わりのマス目でなければ続く
        //なお、cellInfoはwhile内でも増え続ける
        while (cellInfoList.Where(x => x.isOpen == true).Select(x => x).Count() > 0 && exitFlg == false)
        {
            //最小コストのノードを探す
            //経路探索の前に、調査対象のみを条件とし、探索済みの進行方向の推定コストが、最も少ないものを選出。
            cellInfo minCell = cellInfoList.Where(x => x.isOpen == true).OrderBy(x => x.sumConst).Select(x => x).First();

            //調査対象の開拓を行う
            openSurround(minCell);

            // 中心のノードを閉じる
            minCell.isOpen = false;
        }
    }

    /// <summary>
    /// 周辺のセルを開きます
    /// </summary>
    private void openSurround(cellInfo center)
    {
        // ポジションをVector3Intへ変換
        Vector3Int centerPos = map.WorldToCell(center.pos);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // 上下左右のみ可とする、かつ、中心は除外
                if (((i != 0 && j == 0) || (i == 0 && j != 0)) && !(i == 0 && j == 0))
                {
                    //進行方向の確定
                    Vector3Int posInt = new Vector3Int(centerPos.x + i, centerPos.y + j, centerPos.z);

                    //進行方向にタイルがセットされており、かならず上下左右のどちらかに進んでいるとき
                    if (!(i == 0 && j == 0))
                    {
                        // リストに存在しないか探す
                        Vector3 pos = map.CellToWorld(posInt);
                        pos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);
                        if (cellInfoList.Where(x => x.pos == pos).Select(x => x).Count() == 0)
                        {
                            // リストに追加
                            cellInfo cell = new cellInfo();
                            cell.pos = pos;
                            cell.cost = center.cost + 1;// 実コスト(今まで何歩歩いたか)
                            cell.heuristic = Vector2.Distance(pos, goal);// 推定コスト(ゴールまでの距離)
                            cell.sumConst = cell.cost + cell.heuristic;//到着までの実際のコスト
                            cell.parent = center.pos;
                            cell.isOpen = true;
                            cellInfoList.Add(cell);

                            // ゴールの位置と一致したら終了
                            if (map.WorldToCell(goal) == map.WorldToCell(pos))
                            {
                                cellInfo preCell = cell;
                                while (preCell.parent != new Vector3(-9999, -9999, 0))
                                {
                                    map.SetTile(map.WorldToCell(preCell.pos), replaceTile);
                                    preCell = cellInfoList.Where(x => x.pos == preCell.parent).Select(x => x).First();
                                }

                                exitFlg = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

}