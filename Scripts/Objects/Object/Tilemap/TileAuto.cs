using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAuto : MonoBehaviour
{
    public GameObject StageGrid;
    private Tilemap tilemap;

    public TileBase Basetile;

    //ビルド回数(発生させる箱の数)
    public int BuildCount;

    //タイルマップの全体サイズ
    public int x_GridRange;
    public int y_GridRange;


    //軸からいくらずらすか
    private int Zurashi_x;
    private int Zurashi_y;


    //最大の枠からいくら縮小させるか
    //どのくらいのサイズの枠を作るか
    public int Outline_scale_x;
    public int Outline_scale_y;

    //製作するマップの空洞のサイズ
    public int mapbox_scale_x;
    public int mapbox_scale_y;

    private void Start()
    {
        tilemap = StageGrid.GetComponent<Tilemap>();

        //単純に拡大すると右上にずれていくので、ここで拡大した分だけずらす
        //StageGrid.transform.position = new Vector3(-x_GridRange / 2, -y_GridRange / 2, 0);

        tilemap.size = new Vector3Int(x_GridRange,y_GridRange, tilemap.size.z);
        //tilemap.CompressBounds();


        //タイルを埋める
        TileCellar();

        //タイルの状態を確認
        CheckTileCell();
    }

    //タイルを埋めるためのメソッド。アルゴリズムに必要な部屋の作成
    void TileCellar()
    {
        //与えられたマップに重みを付ける

        //for()



        for (int i = 0; i < BuildCount; i++)
        {
            //生成位置のスタート地点をオフセットで決める
            Zurashi_x = Random.Range(-x_GridRange / 2, x_GridRange / 2);
            Zurashi_y = Random.Range(-y_GridRange / 2, y_GridRange / 2);

            //まず、マップ + 空洞　の四角い枠を生成する（開発段階での工程。きちんと□が生成される段階になったら、重みをつける処理に変更する）
            for (int y = -Outline_scale_y - mapbox_scale_y + tilemap.size.y / 2 + Zurashi_y; y <= Outline_scale_y + mapbox_scale_y - 1 + tilemap.size.y / 2 + Zurashi_y; ++y)
            {
                for (int x = -Outline_scale_x - mapbox_scale_x + tilemap.size.x / 2 + Zurashi_x; x < Outline_scale_x + mapbox_scale_x + tilemap.size.x / 2 + Zurashi_x; ++x)
                {
                    /*
                    if((x <= x + mapbox_scale_x && x > x - mapbox_scale_x))
                    {
                        tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), Basetile);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), null);
                    }
                    */

                    Vector3Int grid = tilemap.WorldToCell(new Vector3Int(x- tilemap.size.x / 2, y - tilemap.size.y / 2, tilemap.size.z));

                    if (tilemap.HasTile(grid))
                    {
                        Debug.Log("通った");
                        break;
                    }

                    if (x < tilemap.size.x / 2 + Zurashi_x + mapbox_scale_x && x >= tilemap.size.x / 2 + Zurashi_x - mapbox_scale_x && y < tilemap.size.y / 2 + Zurashi_y + mapbox_scale_y && y >= tilemap.size.y / 2 + Zurashi_y - mapbox_scale_y)
                    {
                        tilemap.SetTile(grid, null);
                    }
                    else
                    {
                        
                        tilemap.SetTile(grid, Basetile);
                    }

                    //

                }
            }
        }
    }

    /*
     
    void TileCellar()
    {
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y - 1 - scale_y; y >= bound.min.y + scale_y; --y)
        {
            for (int x = bound.min.x + scale_x + Zurashi_x; x < bound.max.x - scale_x + Zurashi_x; ++x)
            {
                if((x >= x + 2 && x < x - 2) && (y >= y + 2 && y < y - 2))
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), null);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), Basetile);
                }
            }
        }
    }
     */

    /*
     Gridの中に空き枠２マスの枠で囲った中に、２マス枠のボックス１つを作るプログラムの例
      
    void TileCellar()
    {
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                if((x >= bound.min.x + 2 && x < bound.max.x - 2) && (y >= bound.min.y + 2 && y < bound.max.y - 2))
                {
                    if (!((x >= bound.min.x + 4 && x < bound.max.x - 4) && (y >= bound.min.y + 4 && y < bound.max.y - 4)))
                    {
                        tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), Basetile);
                    }
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), null);
                }
            }
        }
    }
     */

    //埋まっているタイルを確認するためのメソッド
    void CheckTileCell()
    {
        var builder = new StringBuilder();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                builder.Append(StageGrid.GetComponent<Tilemap>().HasTile(new Vector3Int(x, y, 0)) ? "２" : "１");
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
    }

    void CreateMap()
    {
        //タイル基盤ボードの製作
        //グリッドのサイズを基盤のサイズに合わせる
    }


    public class TileInfo
    {
        public readonly Vector3Int m_position;
        public readonly Quaternion m_rotation;
        public readonly TileBase m_tile;
        public readonly bool cell;

        //タイルの位置、タイルの回転、タイルを設置するかどうか（0…しない、１…する）
        public TileInfo(Vector3Int position,Quaternion rotation,TileBase tile,int num)
        {
            m_position = position;
            m_rotation = rotation;
            m_tile = tile;
            cell = num == 1 ? true : false;
        }
    }
    public void PassTile()
    {
        //タイルの情報をずらす
        //0から-1担った瞬間、座標を0基準でタイル情報を左にずらす


        //タイルから何列目にいるか（y軸の値）の情報を引き出す
        int num = 0;
        //Vector3Int vec;
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        int gridnum = bound.max.x;

        //bound.max = new Vector3Int(bound.max.x - 1, bound.max.y, bound.max.z);

        var list = new List<TileInfo>();


        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
        {
            for (int x = bound.min.x; x < gridnum; ++x)
            {
                //Debug.Log("x = " + x);
                //タイルの情報を下記の１行のコードに格納する
                //タイルの座標、メソッドだと思っていた部分で取れるのビビったんやけど…
                //var tile = StageGrid.GetComponent<Tilemap>().GetTile<Tile>(vec = new Vector3Int(x, y, 0));

                //vec.x -= 1;

                var tile = StageGrid.GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));

                var position = new Vector3Int(x, y, 0);
                Vector3 rotation = tilemap.GetTransformMatrix(position).rotation.eulerAngles;//回転を取る

                Quaternion rota = tilemap.GetTransformMatrix(position).rotation;

                Debug.Log("rotation = " + rota);

                //タイルのxの位置がタイルの最小の値より大きい（左側に位置している）なら
                if (position.x >= bound.min.x)
                {
                    //
                    var info = new TileInfo(position,rota, tile, 1);
                    list.Add(info);

                    //本マップの幅が17以下になったらマップを補充する
                }


                if (list.Count <= 0) return;

                num++;
            }

            foreach (var data in list)
            {
                var position = data.m_position;
                var rotation = data.m_rotation;

                if (position.x >= bound.min.x)
                {
                    tilemap.SetTile(position, data.m_tile);
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3Int.zero, rotation, Vector3.one);
                    tilemap.SetTransformMatrix(position, matrix);
                }
                else
                {
                    //ここで、本マップ以降のマスはすべて削除している
                    tilemap.SetTile(position, null);
                }


                position.x += 1;
                tilemap.SetTile(position, null);
            }
        }

        //
        /*
        if (distance > 1)
        {
            tilemap.size = new Vector3Int(tilemap.size.x - distance, 10, tilemap.size.z);
            tilemap.ResizeBounds();
        }
        */
        StageGrid.GetComponent<Tilemap>().size = tilemap.size;
        StageGrid.GetComponent<Tilemap>().CompressBounds();

        //Create_Outline_Maps(list);
    }

    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }
    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        //マップをクリアする（重複しないようにする）
        tilemap.ClearAllTiles();
        //マップの幅の分、周回する
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            //マップの高さの分、周回する
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                // 1 = タイルあり、0 = タイルなし
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
    public static void UpdateMap(int[,] map, Tilemap tilemap) //マップとタイルマップを取得し、null タイルを必要箇所に設定する
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                //再レンダリングではなく、マップの更新のみを行う
                //これは、それぞれのタイル（および衝突データ）を再描画するのに比べて
                //タイルを null に更新するほうが使用リソースが少なくて済むためです。
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
    public static int[,] PerlinNoise(int[,] map, float seed)
    {
        int newPoint;
        //パーリンノイズのポイントの位置を下げるために使用される
        float reduction = 0.5f;
        //パーリンノイズを生成する
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

            //高さの半分の位置付近からノイズが始まるようにする
            newPoint += (map.GetUpperBound(1) / 2);
            for (int y = newPoint; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
        return map;
    }

    public static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
    {
        //ノイズを平滑化して整数の配列内に保存する
        if (interval > 1)
        {
            int newPoint, points;
            //パーリンノイズのポイントの位置を下げるために使用される
            float reduction = 0.5f;

            //平滑化のプロセスで使用される
            Vector2Int currentPos, lastPos;
            //平滑化の際に対応するポイント（x のリストと y のリストが 1 つずつ）
            List<int> noiseX = new List<int>();
            List<int> noiseY = new List<int>();

            //ノイズを生成する
            for (int x = 0; x < map.GetUpperBound(0); x += interval)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            points = noiseY.Count;// 1 で開始するので既に直前の位置があることになる
            for (int i = 1; i < points; i++)
            {
                //現在の位置を取得する
                currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                //直前の位置も取得する
                lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                // 2 つの間の差異を特定する
                Vector2 diff = currentPos - lastPos;

                //高さ変更の値を設定する
                float heightChange = diff.y / interval;
                //現在の高さを特定する
                float currHeight = lastPos.y;

                //最後の x から現在の x までの処理を行う
                for (int x = lastPos.x; x < currentPos.x; x++)
                {
                    for (int y = Mathf.FloorToInt(currHeight); y > 0; y--)
                    {
                        map[x, y] = 1;
                    }
                    currHeight += heightChange;
                }
            }
        }
        else
        {
            //デフォルトでは通常のパーリンノイズ生成が使用される
            map = PerlinNoise(map, seed);
        }

        return map;

    }
    public static int[,] RandomWalkTop(int[,] map, float seed)
    {
        //乱数のシード値を与える
        System.Random rand = new System.Random(seed.GetHashCode());

        //高さの開始値を設定する
        int lastHeight = Random.Range(0, map.GetUpperBound(1));

        //幅の繰り返し処理
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            //コインを投げる
            int nextMove = rand.Next(2);

            //表で、最下部付近でない場合は、高さを減少する
            if (nextMove == 0 && lastHeight > 2)
            {
                lastHeight--;
            }
            //裏で、最上部付近でない場合は、高さを増加する
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) - 2)
            {
                lastHeight++;
            }

            //直前の高さから最下部まで繰り返し処理する
            for (int y = lastHeight; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
        //マップを戻す
        return map;
    }
    public static int[,] RandomWalkTopSmoothed(int[,] map, float seed, int minSectionWidth)
    {
        //乱数のシード値を与える
        System.Random rand = new System.Random(seed.GetHashCode());

        //開始位置を特定する
        int lastHeight = Random.Range(0, map.GetUpperBound(1));

        //どの方向に進むかの特定に使用される
        int nextMove = 0;
        //現在のセクション幅の把握に使用される
        int sectionWidth = 0;

        //配列の幅において処理を行う
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            //次の動きを特定する
            nextMove = rand.Next(2);

            //セクション幅の最小限の値より大きい現在の高さを使用した場合にのみ、高さを変更する
            if (nextMove == 0 && lastHeight > 0 && sectionWidth > minSectionWidth)
            {
                lastHeight--;
                sectionWidth = 0;
            }
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) && sectionWidth > minSectionWidth)
            {
                lastHeight++;
                sectionWidth = 0;
            }
            //セクション幅をインクリメントする
            sectionWidth++;

            //高さから 0 まで処理を繰り返す
            for (int y = lastHeight; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }

        //修正されたマップを戻す
        return map;
    }
}

