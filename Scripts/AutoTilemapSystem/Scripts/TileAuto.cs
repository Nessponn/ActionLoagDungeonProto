using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAuto : MonoBehaviour
{
    #region//変数一覧

    public TileStageData StageData;
    public GameObject StageGrid;//レンダリング基盤となるタイルフィールド

    public GameObject PreStages;//マップ生成の基盤となるもの

    private Tilemap tilemap;

    public TileBase Basetile;//壁や床の元となるタイル
    public TileBase Chaintile;//連結部の生成位置（デバッグ用）

    //ビルド回数(発生させる箱の数)
    private int BuildCount;

    //タイルマップの全体サイズ
    private int x_GridRange;
    private int y_GridRange;

    //どのくらいのサイズの枠を作るか
    private int Outline_scale_x;
    private int Outline_scale_y;

    //製作するマップの空洞のサイズ
    private int mapbox_scale_x;
    private int mapbox_scale_y;


    //連結情報保存用クラス変数
    private List<PrePoints_Info> Preinfo = new List<PrePoints_Info>();

    #endregion

    private void Start()
    {
        TileSetup();    //マップの設定のセットアップ

        //Debug.Log(tilemap.size);

        Premap_Init();   //プリセットのマスを生成する


        PrePoints_Detection();  //プリセットのマスから中継地点を定義する


        //(中継地点を経由しながら)タイルを埋める
        //TileCellar();

        //タイルの状態を確認
        CheckTileCell();
    }

    //プリセットのタイルを認識し、部屋を追加する中継位置を定義する

    void TileSetup()
    {
        //タイルマップのコンポーネント取得
        tilemap = StageGrid.GetComponent<Tilemap>();

        //タイルマップの全体サイズ
        x_GridRange = StageData.x_GridRange;
        y_GridRange = StageData.y_GridRange;

        //単純に拡大すると右上にずれていくので、ここで拡大した分だけずらす
        StageGrid.transform.position = new Vector3(-x_GridRange / 2, -y_GridRange / 2, 0);
        //gameObject.transform.position = new Vector3(-x_GridRange / 2, -y_GridRange / 2, 0);

        tilemap.size = new Vector3Int(x_GridRange, y_GridRange, tilemap.size.z);
        //tilemap.CompressBounds();
    }

    //セットされたプリセットステージをセットする
    void Premap_Init()
    {
        if (!PreStages) return;
        
        var Pretilemap = PreStages.GetComponent<Tilemap>();
        var Prebound = PreStages.GetComponent<Tilemap>().cellBounds;

        Pretilemap.CompressBounds();//リサイズ


        //セットされたプリステージがタイルマップの全体サイズより大きい場合、大きさを上書きする
        if (Pretilemap.size.x > x_GridRange) x_GridRange = Pretilemap.size.x;
        if (Pretilemap.size.y > y_GridRange) y_GridRange = Pretilemap.size.y;

        //サイズの更新
        tilemap.size = new Vector3Int(x_GridRange, y_GridRange, tilemap.size.z);


        for (int y = Prebound.max.y - 1; y >= Prebound.min.y; --y)
        {//左上から右下にかけてタイルを監査する

            for (int x = Prebound.min.x; x < Prebound.max.x; ++x)
            {
                //参照するブロック
                var position = new Vector3Int(x + (x_GridRange - Pretilemap.size.x) / 2, y + (y_GridRange - Pretilemap.size.y) / 2, 0);

                //タイルの取得
                TileBase tile = Pretilemap.GetTile(new Vector3Int(x, y, 0));

                //基盤のタイルマップにプリセット地形を転写
                tilemap.SetTile(position, tile);
                
            }
        }

    }

    //プリセットのタイルマップから中継地点を捜索する
    void PrePoints_Detection()
    {
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //与えられたマップに重みを付ける
        int num = 0;
        Vector3Int StartPos = new Vector3Int();
        Vector3Int EndPos = new Vector3Int(); ;

        //X軸の検査
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {//左上から右下にかけてタイルを監査する

            //工程が変わるごとにnumの値をリセット
            num = 0;
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var position = new Vector3Int(x, y, 0);//参照するブロック

                //タイルの取得
                TileBase tile = tilemap.GetTile(position);

                if(tile != null)
                {
                    //numが0の時、現在の参照位置を始点を設定
                    if(num == 0)StartPos = position;

                    //タイルのカウント
                    num++;
                }
                else
                {
                    //現在の参照位置を終点を設定
                    EndPos = position;
                    EndPos.x = EndPos.x - 1;//一個前の位置を設定

                    //切れた時点で、中継地点として使用するかどうかの決定判断を処理する
                    PrePoints_Init(num,StartPos,EndPos,true);

                    //numの値をリセット
                    num = 0;
                }
            }
        }

        //y軸の検査
        for (int x = bound.min.x; x < bound.max.x; ++x)
        {//左上から右下にかけてタイルを監査する

            //工程が変わるごとにnumの値をリセット
            num = 0;
            for (int y = bound.max.y - 1; y >= bound.min.y; --y)
            {
                var position = new Vector3Int(x, y, 0);//参照するブロック

                //タイルの取得
                TileBase tile = tilemap.GetTile(position);

                if (tile != null)
                {
                    //numが0の時、現在の参照位置を始点を設定
                    if (num == 0) StartPos = position;

                    //タイルのカウント
                    num++;
                }
                else
                {
                    //現在の参照位置を終点を設定
                    EndPos = position;
                    EndPos.y = EndPos.y + 1;//一個前の位置を設定

                    //切れた時点で、中継地点として使用するかどうかの決定判断を処理する
                    PrePoints_Init(num, StartPos, EndPos, false);

                    //numの値をリセット
                    num = 0;
                }
            }
        }
    }

    //中継地点の生成
    void PrePoints_Init(int num, Vector3Int StartPos, Vector3Int EndPos ,bool Xmark)
    {
        //一区画の長さが　一定数以下　であれば、それを部屋として見なさない
        if (num <= 8) return;

        var position = new Vector3Int();

        //検査対象をx軸かy軸かで区別
        if(Xmark) position = new Vector3Int(EndPos.x - ((EndPos.x - StartPos.x) / 2), StartPos.y, StartPos.z);
        else position = new Vector3Int(StartPos.x, EndPos.y - ((EndPos.y - StartPos.y) / 2), StartPos.z);

        //デバッグ用の連結位置保存
        tilemap.SetTile(position, Chaintile);

        //連結部情報の保存
        var info = new PrePoints_Info(position, true);
        Preinfo.Add(info);

    }

    class PrePoints_Info
    {
        Vector3Int position;//位置
        bool Xmark;//X軸の連結部か

        public PrePoints_Info(Vector3Int position,bool Xmark)
        {
            this.position = position;
            this.Xmark = Xmark;
        }
    }






    //タイルを埋めるためのメソッド。アルゴリズムに必要な部屋の作成
    void TileCellar()
    {
        //与えられたマップに重みを付ける
        int num = 0;
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //左上からタイルを代入するにかけ、始点からの　'offset'　と　枠の大きさ'　と　'枠を作る際の右からの変数'　を決める(最初はクラスとか作んないでおく)

        int offsetX,offsetY;//枠の位置をずらす値

        for (int i = 0; i < StageData.Stage.Count; i++)
        {
            //ビルド回数(発生させる箱の数)
            BuildCount = StageData.Stage.Count;

            //どのくらいのサイズの枠を作るか
            Outline_scale_x = StageData.Stage[i].OutlineNum;
            Outline_scale_y = StageData.Stage[i].OutlineNum;

            //製作するマップの空洞のサイズ
            mapbox_scale_x = (int)StageData.Stage[i].StageSize.x;
            mapbox_scale_y = (int)StageData.Stage[i].StageSize.y;

            //生成する枠のoffsetをビルド回数ごとに決定する
            offsetX = Random.Range(0, x_GridRange - mapbox_scale_x);
            offsetY = Random.Range(0, y_GridRange - mapbox_scale_y);

            for (int y = mapbox_scale_y - 1; y >= bound.min.y; --y){//左下から右上にかけてタイルを代入していく

                for (int x = bound.min.x; x < mapbox_scale_x; ++x)
                {
                    var position = new Vector3Int(x + offsetX, y + offsetY, 0);//ブロックを配置する位置の決定
                    //Vector3 rotation = tilemap.GetTransformMatrix(position).rotation.eulerAngles;//回転を取る

                    //タイル情報の保存
                    //var info = new TileInfo(position, 0, rotation, tile, MAP_SPEED);
                    //ist.Add(info);

                    if((x < bound.min.x + Outline_scale_x || x > mapbox_scale_x - 1 - Outline_scale_x) || (y >= mapbox_scale_y - Outline_scale_y) || y <= bound.min.y + Outline_scale_y - 1)
                    {
                        tilemap.SetTile(position, Basetile);
                    }

                    num++;
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

