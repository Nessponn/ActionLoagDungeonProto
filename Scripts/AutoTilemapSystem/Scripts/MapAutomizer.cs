using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    public GameObject mapobj { get; set; }

    public Tilemap map { get; set; }

    public Vector2Int size { get; set; }

    public List<Vector3Int> GimmickStartPosx { get; set; }

    public List<Vector3Int> GimmickGoalPosx { get; set; }

    public List<Vector3Int> GimmickStartPosy { get; set; }

    public List<Vector3Int> GimmickGoalPosy { get; set; }

    public List<Vector3Int> GimmickposLeft { get; set; }

    public List<Vector3Int> GimmickposRight { get; set; }
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
    public TileBase Itemtile_X;
    public TileBase Itemtile_Y;

    public int Thickness;//間隔

    [Range(0,2)]public float Reaction;

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
        //マップデータの作成
        GameObject mapobj = new GameObject();//この時実は生成処理も行われている
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        

        mapobj.transform.parent = TileGrid.transform;//壁生成処理のための座用を正しく登録するため、オブジェクトの子登録をしておく

        //タイルマップコンポーネントの取得
        var mapdata = mapobj.GetComponent<Tilemap>();

        //縦横比の設定
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);



        //mapInfoの作成＆初期化
        MapInfo mapInfo = new MapInfo();
        mapInfo.map = mapdata;
        mapInfo.mapobj = mapobj;//マップ状態を保存
        mapInfo.size = new Vector2Int(Width, Height);
        mapInfo.GimmickposLeft = new List<Vector3Int>();
        mapInfo.GimmickposRight = new List<Vector3Int>();

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
        List<Vector3Int> posLeft = LeftPointerFromMiddle(mapInfo);//左側のタイルマップ
        List<Vector3Int> posRight = RightPointerFromMiddle(mapInfo);//右側のタイルマップ

        MapAutoCreate(mapInfo, posLeft, posRight);
    }

    //中央から左端へ点を打つ
    List<Vector3Int> LeftPointerFromMiddle(MapInfo mapInfo)
    {
        List<Vector3Int> posLeft = new List<Vector3Int>();//左側のタイルマップ

        //幅情報を取る
        var Cellbound = mapInfo.map.cellBounds;


        //Debug.Log(Cellbound.max.x);

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//左下から右上にかけてタイルを監査する

            //中央までで打ち止め
            for (int x = Cellbound.max.x / 2 - 1; x >= 0; x--)
            {

                //乱数設定
                int rad = Random.Range(1, 10);


                //乱数で当たったらタイルを埋める(1/5程度)
                if (rad > 8)
                {
                    var position = new Vector3Int(x, y, 0);
                    posLeft.Add(position);

                    break;///点が打たれた時点で、そのx軸での生成処理は終了
                }

                if(x == 0)
                {
                    var position = new Vector3Int(x, y, 0);
                    posLeft.Add(position);

                    break;///点が打たれた時点で、そのx軸での生成処理は終了
                }
            }
        }

        return posLeft;
    }

    //中央から右端へ点を打つ
    List<Vector3Int> RightPointerFromMiddle(MapInfo mapInfo)
    {
        List<Vector3Int> posRight = new List<Vector3Int>();//右側のタイルマップ

        //幅情報を取る
        var Cellbound = mapInfo.map.cellBounds;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//左下から右上にかけてタイルを監査する

            //中央までで打ち止め
            for (int x = Cellbound.max.x / 2 + 1; x <= Cellbound.max.x; x++)
            {
                //乱数設定
                int rad = Random.Range(1, 10);

                //乱数で当たったらタイルを埋める(1/5程度)
                if (rad > 8)
                {
                    var position = new Vector3Int(x, y, 0);
                    posRight.Add(position);

                    break;///点が打たれた時点で、そのx軸での生成処理は終了
                }

                //この時点でどこも打たれていなければ、右端で打つ
                if(x == Cellbound.max.x)
                {
                    var position = new Vector3Int(x, y, 0);
                    posRight.Add(position);

                    break;///点が打たれた時点で、そのx軸での生成処理は終了
                }
            }
        }

        return posRight;
    }
    void LeftPointerToMiddle(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        for (int y = 0; y < mapInfo.size.y; y++)
        {//左下から右上にかけてタイルを監査する

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //乱数設定
                int rad = Random.Range(1, 10);

                //乱数で当たったらタイルを埋める(1/5程度)
                if (rad > 8)
                {

                }
            }
        }
    }

    //右端から中央へ点を打つ
    void RightPointerToMiddle(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        for (int y = 0; y < mapInfo.size.y; y++)
        {//左下から右上にかけてタイルを監査する

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //乱数設定
                int rad = Random.Range(1, 10);

                //乱数で当たったらタイルを埋める(1/5程度)
                if (rad > 8)
                {

                }
            }
        }
    }


    void MapAutoCreate(MapInfo mapInfo, List<Vector3Int> posLeft, List<Vector3Int> posRight)
    {
        //タイルマップコンポーネントの取得
        var maptile = mapInfo.mapobj.GetComponent<Tilemap>();

        //入口と出口となる部分が正常に生成されていれば、処理を続行する
        if (!(posLeft[0].y == posRight[0].y && posLeft[posLeft.Count - 1].y == posRight[posRight.Count - 1].y))
        {
            //きちんと点が生成されていなければ、再生成処理を行う
            Debug.LogError("正常に生成されませんでした。再生成を行います");

            return;
        }

        mapInfo.GimmickStartPosx = new List<Vector3Int>();
        mapInfo.GimmickGoalPosx = new List<Vector3Int>();
        mapInfo.GimmickStartPosy = new List<Vector3Int>();
        mapInfo.GimmickGoalPosy = new List<Vector3Int>();
        mapInfo.GimmickStartPosx.Add(new Vector3Int(-9999, -9999, 0));
        mapInfo.GimmickStartPosy.Add(new Vector3Int(-9999, -9999, 0));

        //壁を作る処理
        //このfor文では、必要以上に凸凹しないように調整するためのもの
        for (int x = 0; x < posLeft.Count - 1; x++)
        {
            for (int ex = x + 1; ex < posLeft.Count - 1; ex++)
            {
                //次の点として、適切かどうかを決める
                if (Mathf.Abs(posLeft[x].x - posLeft[ex].x) < size.x + ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) * (Reaction - 1) / Thickness || posLeft[ex] == posLeft[posLeft.Count - 1])
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
        //Gimmick_posset(mapInfo, 0);


        for (int x = 0; x < posRight.Count - 1; x++)
        {
            for (int ex = x + 1; ex < posRight.Count - 1; ex++)
            {
                //次の点として、適切かどうかを決める
                if (Mathf.Abs(posRight[x].x - posRight[ex].x) < size.x + ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) * (Reaction - 1) / Thickness || posRight[ex] == posRight[posRight.Count - 1])
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

            AStarPath.Instance.astarSearchPathFinding(mapInfo, posRight[x], posRight[x + 1], 1);//右
        }

        //Gimmick_posset(mapInfo, 1);
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

    public void Gimmickpos_Add()
    {

    }

    //ギミックを設置する処理
    public void Gimmick_posset(MapInfo mapInfo,int dir,List<Vector3Int> GimmickStartPosx, List<Vector3Int> GimmickGoalPosx, List<Vector3Int> GimmickStartPosy, List<Vector3Int> GimmickGoalPosy)
    {
        //何も入っていなければ、処理を飛ばす
        if (mapInfo.GimmickStartPosy[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPosy.Count - 1; x++)//一番最後はありえない値なので、除外する意味で-1する
            {
                int Ypos = mapInfo.GimmickStartPosy[x].y - mapInfo.GimmickGoalPosy[x].y;

                //一定以上幅があれば、ギミックを設置する
                if (Ypos >= 3)
                {
                    if (dir == 0)//右側に生成
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPosy[x].x + 1, mapInfo.GimmickGoalPosy[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_Y);
                        mapInfo.GimmickposLeft.Add(mapInfo.map.WorldToCell(position));
                    }
                    else//左側に生成
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPosy[x].x - 1, mapInfo.GimmickGoalPosy[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_Y);
                        mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
                    }
                }
            }
        }

        if (mapInfo.GimmickStartPosx[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPosx.Count - 1; x++)//一番最後はありえない値なので、除外する意味で-1する
            {
                int Ypos = mapInfo.GimmickStartPosx[x].y - mapInfo.GimmickGoalPosx[x].y;
                int Xpos = mapInfo.GimmickStartPosx[x].x - mapInfo.GimmickGoalPosx[x].x;

                //一定以上幅があれば、ギミックを設置する
                if (Xpos >= 3 * Thickness)
                {
                    var position = new Vector3Int(mapInfo.GimmickStartPosx[x].x - Xpos / 2, mapInfo.GimmickGoalPosx[x].y + Ypos / 2, 0);

                    mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_X);//設置

                    //下方向に生成するための情報をz軸に入れる(使うときは　0　にするのを忘れずに)
                    position.z = -1;//下
                    mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
                }
                else if (Xpos <= -3 * Thickness)
                {
                    var position = new Vector3Int(mapInfo.GimmickStartPosx[x].x - Xpos / 2, mapInfo.GimmickGoalPosx[x].y + Ypos / 2, 0);

                    mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_X);//設置

                    //上方向に生成するための情報をz軸に入れる(使うときは　0　にするのを忘れずに)
                    position.z = 1;//上
                    mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
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

        for (int y = Cellbound.max.y - 1 ; y >= Cellbound.min.y; y --)
        {//左下から右上にかけてタイルを監査する

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                var mapcell = map.GetTile(new Vector3Int(x, y, 0));

                if (mapcell)
                {
                    //参照するブロック
                    var position = new Vector3Int(y, x, 0);

                    //赤い点と同じところにあたったら
                    if (mapcell == Itemtile_X)
                    {
                        mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                    }
                    //緑色の点と同じところにあたったら
                    else if (mapcell == Itemtile_Y)
                    {
                        mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                    }
                    else mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                }
            }
        }

        Destroy(map.transform.gameObject);

        return mapobj;
    }

    //元となるマップデータを作成（後の MapBoundCreate）
    
    void TileDataDone(GameObject mapobj, int Width, int Height)
    {

        //タイルマップの位置調整
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

    }
}
