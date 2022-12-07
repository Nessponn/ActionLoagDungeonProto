using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    public GameObject mapobj { get; set; }

    public Tilemap map { get; set; }

    public Vector2Int size { get; set; }

    public List<Vector3Int> GimmickStartPos { get; set; }

    public List<Vector3Int> GimmickGoalPos { get; set; }
}
//マップオートマイザー
public class MapAutomizer : SingletonMonoBehaviourFast<MapAutomizer>
{
    /// <summary>
    /// 概要
    /// １．タイルマップオブジェクトを作成し、情報を入れる準備を行う。（縦横比、など）
    /// 
    /// 
    /// </summary>
    /// 

    public Vector2Int size;
    public GameObject TileGrid;
    public TileBase Basetile;
    public TileBase Itemtile;

    public int Thickness;//間隔

    private List<MapInfo> mapInfo = new List<MapInfo>();
    
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> mapobj = new List<GameObject>();

        for(int x = 0;x < 10; x++) mapobj.Add(TileMapDataCreate(size.x * Thickness, size.y * Thickness));

        for (int x = 0; x < 10; x++) 
        { 
            if(x % 2 == 0)mapobj[x].transform.position = new Vector2(size.x * x * 2 - 75, size.y * 2);
            else mapobj[x].transform.position = new Vector2(size.x * x * 2 - 75, -size.y * 2);
        }
    }

    

    GameObject TileMapDataCreate(int Width,int Height)
    {
        MapInfo mapInfo = new MapInfo();

        //マップデータの作成
        GameObject mapobj = new GameObject();//この時実は生成処理も行われている
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        mapobj.transform.parent = TileGrid.transform;//壁生成処理のための座用を正しく登録するため、オブジェクトの子登録をしておく

        //タイルマップコンポーネントの取得
        var mapdata = mapobj.GetComponent<Tilemap>();
        mapInfo.map = mapdata;

        //縦横比の設定
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);

        //マップ状態を保存
        mapInfo.mapobj = mapobj;
        mapInfo.size = new Vector2Int(Width, Height);

        //マップ配列データの作成
        MapBoundCreate(mapInfo);

        //回転処理
        //mapobj = Rotate(mapobj.GetComponent<Tilemap>());

        TileDataDone(mapobj, Width, Height);

        this.mapInfo.Add(mapInfo);



        return mapobj;
    }

    //元となるマップデータを作成
    void MapBoundCreate(MapInfo mapInfo)
    {
        //マップ配列の作成
        int[,] mapdata = new int[mapInfo.size.x, mapInfo.size.y];
        int[,] mappoint = new int[mapInfo.size.x, mapInfo.size.y];

        //マップの位置保存用
        int Count;

        //タイルの設置直後の制限カウント用
        int TiledCount;

        //マップの中心点（絶対に埋まらない部分。まずは０で設定）
        int middlepoint = 0;

        //Debug.Log("Width =" + Width);
        for (int y = 0; y < mapInfo.size.y; y++)
        {//左下から右上にかけてタイルを監査する

            //カウントのリセット
            Count = 2;
            TiledCount = 0;
            //Debug.Log("x[" + y + "] =" + ((int)((-Mathf.Pow(y, 2) + (Height * y)) / Height)));

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //マップの中心点なら飛ばす、残りカウントが０以下でも飛ばす
                if (x != middlepoint && Count > 0 && TiledCount <= 0)
                {
                    //乱数設定
                    int rad = Random.Range(1, 10);


                    //乱数で当たったらタイルを埋める(1/5程度)
                    if (rad > 8)
                    {
                        if (Count == 2)
                        {
                            //マップの真ん中あたりが膨らむ様に、調整

                            mapdata[x, y] = 1;
                            mappoint[x, y] = Count;
                        }
                        else if (Count == 1)
                        {
                            int X = x + ((int)((-Mathf.Pow(y, 2) + (mapInfo.size.y * y)) / mapInfo.size.y)) * 2;
                            if (X > mapInfo.size.x - 1) X = mapInfo.size.x - 1;

                            //マップの真ん中あたりが膨らむ様に、調整
                            mapdata[X, y] = 1;
                            mappoint[X, y] = Count;
                        }


                        /*mapdata[x, y] = 1;
                        mappoint[x, y] = Count;*/

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                    //ただしx軸の末端時点でまだ回数分設定していな場合、例外的に自動で埋める
                    else if (Count > 0 && x == mapInfo.size.x - 1)
                    {
                        mapdata[x, y] = 1;
                        mappoint[x, y] = Count;

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                    //一つ目のタイルがx軸の末尾から３つ目の時点で設置されていない場合、例外的に自動で埋める
                    else if (Count > 1 && x == mapInfo.size.x - 3)
                    {
                        mapdata[x, y] = 1;
                        mappoint[x, y] = Count;

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                }

                TiledCount--;
            }

            if (Count > 0) Debug.Log("カウントまだ残ってる");
        }

        MapAutoCreate(mapInfo, mapdata, mappoint);
    }

    void MapAutoCreate(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        //タイルマップコンポーネントの取得
        var maptile = mapInfo.mapobj.GetComponent<Tilemap>();

        //幅情報を取る
        var Cellbound = mapInfo.mapobj.GetComponent<Tilemap>().cellBounds;

        List<Vector3Int> posLeft = new List<Vector3Int>();//左側のタイルマップ
        List<Vector3Int> posRight = new List<Vector3Int>();//右側のタイルマップ

        //マップデータのの位置参照用
        int mx, my;
        mx = my = 0;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//左下から右上にかけてタイルを監査する

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //壁（１）であれば、埋める
                if (mapdata[mx, my] == 1)
                {
                    //指定幅を１以上指定していれば、周りも埋める
                    /*for (int ey = y + ((Thickness - 1) / 2); ey >= y - ((Thickness - 1) / 2); ey--)
                    {
                        //参照するブロック
                        var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                        maptile.SetTile(position, Basetile);
                    }*/

                    //参照するブロック
                    var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                    maptile.SetTile(position, Basetile);

                    //タイルを左右に分ける
                    if (mappoint[mx, my] == 2)//左であれば
                    {
                        posLeft.Add(position);
                    }
                    else//右であれば
                    {
                        posRight.Add(position);
                    }
                }
                mx++;
            }
            mx = 0;
            my++;
        }

        //入口と出口となる部分が正常に生成されていれば、処理を続行する
        if (!(posLeft[0].y == posRight[0].y && posLeft[posLeft.Count - 1].y == posRight[posRight.Count - 1].y))
        {
            //きちんと点が生成されていなければ、再生成処理を行う
            Debug.LogError("正常に生成されませんでした。再生成を行います");

            //Destroy(mapobj);

            //TileMapDataCreate(size.x * Thickness, size.y * Thickness);

            return;
        }

        mapInfo.GimmickStartPos = new List<Vector3Int>();
        mapInfo.GimmickGoalPos = new List<Vector3Int>();
        mapInfo.GimmickStartPos.Add(new Vector3Int(-9999, -9999, 0));

        //壁を作る処理
        for (int x = 0; x < posLeft.Count - 1; x++)
        {
            float X = posLeft[x].y / Thickness;

            for (int ex = x + 1; ex < posLeft.Count - 1; ex++)
            {
                //次の点として、適切かどうかを決める
                if (Mathf.Abs(posLeft[x].x - posLeft[ex].x) < size.x - ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) / Thickness || posLeft[ex] == posLeft[posLeft.Count - 1])
                {
                    //必要以上に離れていなければ、中継点として扱う
                    //または、次の点が最後の点であれば、それは中継点として扱う

                    break;
                }
                else
                {
                    //不適合であれば、候補として消す
                    maptile.SetTile(posLeft[ex], null);
                    posLeft.RemoveAt(ex);
                    //減らした分だけ、exも調整する
                    ex--;
                }
            }

            AStarPath.Instance.astarSearchPathFinding(mapInfo, posLeft[x], posLeft[x + 1], 0);//左
        }

        //MapAutomizer.Instance.Gimmick_Init(map,GimmickStartPos, GimmickGoalPos, dir);

        Gimmick_Init(mapInfo, 0);

        //AStarPath.Instance.astarSearchPathFinding(maptile, posLeft[0], posLeft[1], 0);//左

        /*for (int x = 0; x < posRight.Count - 1; x++)
        {
            float X = posRight[x].y / Thickness;

            for (int ex = x + 1; ex < posRight.Count - 1; ex++)
            {
                //次の点として、適切かどうかを決める
                if (Mathf.Abs(posRight[x].x - posRight[ex].x) < size.x - ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) / Thickness || posRight[ex] == posRight[posRight.Count - 1])
                {
                    //必要以上に離れていなければ、中継点として扱う
                    //または、次の点が最後の点であれば、それは中継点として扱う

                    break;
                }
                else
                {
                    //不適合であれば、候補として消す
                    maptile.SetTile(posRight[ex], null);
                    posRight.RemoveAt(ex);
                    //減らした分だけ、exも調整する
                    ex--;
                }
            }

            AStarPath.Instance.astarSearchPathFinding(maptile, posRight[x], posRight[x + 1], 1);//右
        }*/
        /*
                //上下の枠を閉じる
                for (int x = posLeft[0].x; x <= posRight[0].x; x++)
                {
                    var position = posLeft[0];
                    position.x = x;

                    maptile.SetTile(position, Basetile);
                }

                for (int x = posLeft[posLeft.Count - 1].x; x <= posRight[posRight.Count - 1].x; x++)
                {
                    var position = posLeft[posLeft.Count - 1];
                    position.x = x;

                    if (maptile.HasTile(position)) maptile.SetTile(position, null);
                    else maptile.SetTile(position, Basetile);
                }*/
    }
    //ギミックを設置する処理
    public void Gimmick_Init(MapInfo mapInfo,int dir)
    {
        //何も入っていなければ、処理を飛ばす
        if (mapInfo.GimmickStartPos[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPos.Count - 1; x++)//一番最後はありえない値なので、除外する意味で-1する
            {
                //Debug.Log("x[" + x + "]" + (mapInfo.GimmickStartPos[x].y - mapInfo.GimmickGoalPos[x].y));

                int Ypos = mapInfo.GimmickStartPos[x].y - mapInfo.GimmickGoalPos[x].y;

                //一定以上幅があれば、ギミックを設置する
                if (Ypos >= 3)
                {
                    if (dir == 0)//右側に生成
                    {
                        Debug.Log("トレジャーッ！！！");

                        var position = new Vector3Int(mapInfo.GimmickStartPos[x].x + 1, mapInfo.GimmickGoalPos[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile);
                    }
                    else//左側に生成
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPos[x].x - 1, mapInfo.GimmickGoalPos[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile);
                    }
                }
            }
        } 
    }

    //生成マップの回転処理(左に９０度回転)
    GameObject Rotate(Tilemap map)
    {
        //マップデータの作成
        GameObject mapobj = new GameObject();//この時実は生成処理も行われている
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        mapobj.transform.parent = TileGrid.transform;//壁生成処理のための座用を正しく登録するため、オブジェクトの子登録をしておく

        //タイルマップコンポーネントの取得
        var mapdata = mapobj.GetComponent<Tilemap>();

        //縦横比の設定
        mapdata.size = new Vector3Int(map.size.x, map.size.y, mapdata.size.z);

        //幅情報を取る
        var Cellbound = map.cellBounds;

        //GameObject obj = Instantiate(mapdata.transform.gameObject, TileGrid.transform.position, Quaternion.identity);
        for (int y = Cellbound.max.y - 1 ; y >= Cellbound.min.y; y --)
        {//左下から右上にかけてタイルを監査する

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                if(map.GetTile(new Vector3Int(x, y, 0)))
                {
                    //参照するブロック
                    var position = new Vector3Int(y, x, 0);
                    mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                }

            }
        }
        //mapobj.transform.position = new Vector3(-map.size.x / 2, -map.size.y / 2, 0);

        Destroy(map.transform.gameObject);

        return mapobj;
    }

    //元となるマップデータを作成（後の MapBoundCreate）
    void MapAutoCreate(GameObject mapobj)
    {
        ///
        /// まず、横１列に付き、２つのタイルを設置
        ///

        //タイルマップコンポーネントの取得
        var mapdata = mapobj.GetComponent<Tilemap>();

        //幅情報を取る
        var Cellbound = mapobj.GetComponent<Tilemap>().cellBounds;

        //マップの位置保存用
        int Count;


        //マップの中心点（絶対に埋まらない部分。まずは０で設定）
        int middlepoint = 0;

        for (int y = Cellbound.max.y - 1; y >= Cellbound.min.y; y --)
        {//左下から右上にかけてタイルを監査する

            //カウントのリセット
            Count = 2;

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //マップの中心点なら飛ばす、残りカウントが０以下でも飛ばす
                if(x != middlepoint && Count > 0)
                {
                    //乱数設定
                    int rad = Random.Range(1, 10);


                    //乱数で当たったらタイルを埋める(1/5程度)
                    if(rad <= 2)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //カウントをマイナス
                        Count--;

                    }
                    //ただしx軸の末端時点でまだ回数分設定していな場合、例外的に自動で埋める
                    else if (Count > 0 && x == Cellbound.max.x - 1)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //カウントをマイナス
                        Count--;
                    }
                    //一つ目のタイルがx軸の末尾から３つ目の時点で設置されていない場合、例外的に自動で埋める
                    else if (Count > 1 && x == Cellbound.max.x - 3)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //カウントをマイナス
                        Count--;

                    }
                }

            }

        }

        Init_MapData(mapobj);
    }

    //マップの生成（テスト機能）
    void Init_MapData(GameObject mapobj)
    {
        
    }

    void TileDataDone(GameObject mapobj, int Width, int Height)
    {

        //タイルマップの位置調整
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

    }
}
