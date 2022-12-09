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

public class AStarPath : SingletonMonoBehaviourFast<AStarPath>
{
    //public Tilemap map;                     // 移動範囲
    public TileBase replaceTile;            // 移動線上に位置するタイルの色を代える
    //public TileBase player;               // プレイヤーのゲームオブジェクト
    //public TileBase enemy;                // 敵のゲームオブジェクト
    //private List<cellInfo> cellInfoList = new List<cellInfo>();    // 調査セルを記憶しておくリスト
    private Vector3 goal;                   // ゴールの位置情報
    private bool exitFlg;                   // 処理が終了したかどうか

    // Start is called before the first frame update
    void Start()
    {
        //cellInfoList = new List<cellInfo>();

        //astarSearchPathFinding();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// AStarアルゴリズムです。
    /// </summary>
    /*public void astarSearchPathFinding()
    {
        var maptile = map.GetComponent<Tilemap>();

        //幅情報を取る
        var Cellbound = map.GetComponent<Tilemap>().cellBounds;

        // スタートの情報を設定する(スタートは敵)
        cellInfo start = new cellInfo();
        //start.pos = map.WorldToCell(enemy.transform.position);
        start.cost = 0;
        //start.heuristic = Vector2.Distance(enemy.transform.position, goal);
        start.sumConst = start.cost + start.heuristic;
        start.parent = new Vector3(-9999, -9999, 0);    // スタート時の親の位置はありえない値にしておきます
        start.isOpen = true;
        cellInfoList.Add(start);

        exitFlg = false;

        for (int y = Cellbound.max.y - 1; y >= Cellbound.min.y; y--)
        {//左下から右上にかけてタイルを監査する

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //参照するブロック
                var position = new Vector3Int(x, y, 0);

                // ゴールはプレイヤーの位置情報
                if (map.GetTile(position) == player) goal = map.WorldToCell(position);
                if (map.GetTile(position) == enemy) start.pos = map.WorldToCell(position);
            }
        }

        start.heuristic = Vector2.Distance(start.pos, goal);

        //goal = player.transform.position;



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
    }*/

    //MapAutomizerから得られたデータから、壁生成を行う
    public void astarSearchPathFinding(MapInfo mapInfo, Vector3Int Startpos, Vector3Int Goalpos , int dir)
    {
        List<cellInfo> cellInfoList = new List<cellInfo>();

        // スタートの情報を設定する(スタートは敵)
        cellInfo start = new cellInfo();
        //start.pos = map.WorldToCell(enemy.transform.position);
        start.cost = 0;
        //start.heuristic = Vector2.Distance(enemy.transform.position, goal);
        start.sumConst = start.cost + start.heuristic;
        start.parent = new Vector3(-9999, -9999, 0);    // スタート時の親の位置はありえない値にしておきます
        start.isOpen = true;

        //Debug.Log("map = " + map);
        //Debug.Log("posLeft = " + posLeft);

        cellInfoList.Add(start);

        exitFlg = false;

        goal = mapInfo.map.WorldToCell(Startpos);
        start.pos = mapInfo.map.WorldToCell(Goalpos);

        start.heuristic = Vector2.Distance(start.pos, goal);

        //ギミック生成用変数
        int Count = 0;
        List<Vector3Int> GimmickStartPos = new List<Vector3Int>() { new Vector3Int(-9999, -9999, 0) };
        List<Vector3Int> GimmickGoalPos = new List<Vector3Int>() { new Vector3Int(-9999, -9999, 0) };

        //オープンが存在する限りループ
        //調査対象として、含まれているマス目がある限り、それが終わりのマス目でなければ続く
        //なお、cellInfoはwhile内でも増え続ける
        while (cellInfoList.Where(x => x.isOpen == true).Select(x => x).Count() > 0 && exitFlg == false)
        {
            //最小コストのノードを探す
            //経路探索の前に、調査対象のみを条件とし、探索済みの進行方向の推定コストが、最も少ないものを選出。
            cellInfo minCell = cellInfoList.Where(x => x.isOpen == true).OrderBy(x => x.sumConst).Select(x => x).First();

            //調査対象の開拓を行う
            //openSurround(minCell, map,cellInfoList , Count, GimmickStartPos, GimmickGoalPos);
            openSurround(minCell, cellInfoList,mapInfo);

            // 中心のノードを閉じる
            minCell.isOpen = false;
        }

        //ギミックの生成処理に移る
        //MapAutomizer.Instance.Gimmick_Init(map,GimmickStartPos, GimmickGoalPos, dir);
    }
    /// <summary>
    /// 周辺のセルを開きます
    /// </summary>
    private void openSurround(cellInfo center, List<cellInfo> cellInfoList ,MapInfo mapInfo)
    {
        // ポジションをVector3Intへ変換
        Vector3Int centerPos = mapInfo.map.WorldToCell(center.pos);

        //-1～1の範囲で探索
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // 上下左右のみ可とする、かつ、中心は除外
                if (((i != 0 && j == 0) || (i == 0 && j != 0)) && !(i == 0 && j == 0))
                {
                    //進行方向の確定
                    //かならず上下左右のどちらかに進んでいる
                    Vector3Int posInt = new Vector3Int(centerPos.x + i, centerPos.y + j, centerPos.z);

                    // リストに存在しないか探す
                    Vector3 pos = mapInfo.map.CellToWorld(posInt);
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

                        /*//一定以上、下に連続で進んでいれば、その中間の位置にギミックをつける
                        //始点と終点を決める
                        if(i == 0 && j == -1)
                        {
                            if(GimmickStartPos[Count] == new Vector3Int(-9999, -9999, 0))
                            {
                                GimmickStartPos[Count] = map.WorldToCell(pos);
                            }
                        }
                        //下以外に進んだら、カウントは
                        else
                        {
                            if (GimmickStartPos[Count] != new Vector3Int(-9999, -9999, 0))
                            {
                                //下が途切れた地点をゴールとして登録する
                                GimmickGoalPos[Count] = map.WorldToCell(pos);

                                Count++;
                                GimmickStartPos.Add(new Vector3Int(-9999, -9999, 0));
                                GimmickGoalPos.Add(new Vector3Int(-9999, -9999, 0));
                            }
                            
                        }*/

                        // ゴールの位置と一致したら終了
                        if (mapInfo.map.WorldToCell(goal) == mapInfo.map.WorldToCell(pos))
                        {
                            cellInfo preCell = cell;


                            //片側の方だけの実装を想定
                            if(mapInfo.GimmickStartPosx[0] == new Vector3Int(-9999, -9999, 0))
                            {
                                mapInfo.GimmickStartPosx[0] = mapInfo.map.WorldToCell(preCell.pos);
                            }

                            if (mapInfo.GimmickStartPosy[0] == new Vector3Int(-9999, -9999, 0))
                            {
                                mapInfo.GimmickStartPosy[0] = mapInfo.map.WorldToCell(preCell.pos);
                            }

                            while (preCell.parent != new Vector3(-9999, -9999, 0))
                            {
                                //Debug.Log(mapInfo.map.WorldToCell(preCell.pos));

                                //マスの設置
                                mapInfo.map.SetTile(mapInfo.map.WorldToCell(preCell.pos), replaceTile);

                                //設置したタイルの情報保持
                                var prevCell = preCell;

                                //次に設置するタイルの情報
                                preCell = cellInfoList.Where(x => x.pos == preCell.parent).Select(x => x).First();

                                //ギミック生成処理
                                //前のマスとx軸が違ったら、そこでいったん区切る
                                if(prevCell.pos.x != preCell.pos.x)
                                {
                                    //Debug.Log("曲がった");

                                    //現在のマス目で登録
                                    mapInfo.GimmickGoalPosy.Add(mapInfo.map.WorldToCell(prevCell.pos));

                                    //次のマス目を、新たなスタート地点として登録
                                    mapInfo.GimmickStartPosy.Add(mapInfo.map.WorldToCell(preCell.pos));
                                }

                                if (prevCell.pos.y != preCell.pos.y)
                                {
                                    //Debug.Log("曲がった");

                                    //現在のマス目で登録
                                    mapInfo.GimmickGoalPosx.Add(mapInfo.map.WorldToCell(prevCell.pos));

                                    //次のマス目を、新たなスタート地点として登録
                                    mapInfo.GimmickStartPosx.Add(mapInfo.map.WorldToCell(preCell.pos));
                                }
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