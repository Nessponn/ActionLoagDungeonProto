using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//マップオートマイザー
public class MapAutomizer : MonoBehaviour
{
    /// <summary>
    /// 概要
    /// １．タイルマップオブジェクトを作成し、情報を入れる準備を行う。（縦横比、など）
    /// 
    /// 
    /// </summary>
    /// 

    public GameObject TileGrid;
    public TileBase Basetile;

    public int Thickness;//間隔
    //マップごとの情報格納庫
    class MapInfo
    {
        GameObject mapobj;
        MapInfo(int Width)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TileMapDataCreate(15 * Thickness, 15 * Thickness);
    }

    void TileMapDataCreate(int Width,int Height)
    {
        //マップデータの作成
        GameObject mapobj = new GameObject();//この時実は生成処理も行われている
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();
        
        //タイルマップコンポーネントの取得
        var mapdata = mapobj.GetComponent<Tilemap>();

        //縦横比の設定
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);

        //タイルマップの位置調整
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

        //マップの作成
        //MapAutoCreate(mapobj);

        //マップ配列データの作成
        MapBoundCreate(mapobj,Width, Height);
    }

    //元となるマップデータを作成
    void MapBoundCreate(GameObject mapobj,int Width,int Height)
    {
        //マップ配列の作成
        int[,] mapdata = new int[Width,Height];

        //マップの位置保存用
        int Count;

        //タイルの設置直後の制限カウント用
        int TiledCount;

        //マップの中心点（絶対に埋まらない部分。まずは０で設定）
        int middlepoint = 0;

        for (int y = 0;y < Height;y++)
        {//左下から右上にかけてタイルを監査する

            //カウントのリセット
            Count = 2;
            TiledCount = 0;

            for (int x = 0; x < Width; x++)
            {
                //マップの中心点なら飛ばす、残りカウントが０以下でも飛ばす
                if (x != middlepoint && Count > 0 && TiledCount <= 0)
                {
                    //乱数設定
                    int rad = Random.Range(1, 10);


                    //乱数で当たったらタイルを埋める(1/5程度)
                    if (rad <= 2)
                    {
                        mapdata[x, y] = 1;

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                    //ただしx軸の末端時点でまだ回数分設定していな場合、例外的に自動で埋める
                    else if (Count > 0 && x >= Width - 1)
                    {
                        mapdata[x, y] = 1;

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                    //一つ目のタイルがx軸の末尾から３つ目の時点で設置されていない場合、例外的に自動で埋める
                    else if (Count > 1 && x == Width - 3)
                    {
                        mapdata[x, y] = 1;

                        //カウントをマイナス
                        Count--;
                        TiledCount = 2;
                    }
                }

                TiledCount--;
            }

            if (Count > 0) Debug.Log("カウントまだ残ってる");
        }

        MapAutoCreate(mapobj, mapdata);
    }


    void MapAutoCreate(GameObject mapobj,int[,] mapdata)
    {
        //タイルマップコンポーネントの取得
        var maptile = mapobj.GetComponent<Tilemap>();

        //幅情報を取る
        var Cellbound = mapobj.GetComponent<Tilemap>().cellBounds;

        //マップデータのの位置参照用
        int mx, my;
        mx = my = 0;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y-= Thickness)
        {//左下から右上にかけてタイルを監査する

            for (int x = Cellbound.min.x ; x < Cellbound.max.x; x+= Thickness)
            {
                //壁（１）であれば、埋める
                if (mapdata[mx, my] == 1)
                {
                    //指定幅を１以上指定していれば、周りも埋める
                    //for (int ey = y + ((Thickness - 1) / 2); ey >= y - ((Thickness - 1) / 2); ey--)
                    //{
                        //参照するブロック
                        var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                        maptile.SetTile(position, Basetile);
                    //}
                }

                mx++;
            }
            Debug.Log(mx);

            mx = 0;
            my++;
        }

        Init_MapData(mapobj);
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
        //GameObject obj = Instantiate(mapdata.transform.gameObject, TileGrid.transform.position, Quaternion.identity);

        mapobj.transform.parent = TileGrid.transform;
    }
}
