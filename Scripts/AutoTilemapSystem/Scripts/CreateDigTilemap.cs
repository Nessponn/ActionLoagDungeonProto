using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DigMaze;

public class CreateDigTilemap : MonoBehaviour
{
    [Range(5, 121)] public int Width;
    [Range(5, 121)] public int Height;
    [Range(1, 9)] public int thickness;

    public GameObject StageGrid;

    public List<GameObject> PresetMaps = new List<GameObject>();

    public TileBase Basetile;//壁や床の元となるタイル
    public TileBase Chaintile;//連結部の生成位置（デバッグ用）
    public TileBase AddMaptile;//プリマップの生成位置（デバッグ用）

    private Tilemap tilemap;

    private DigTilemap.MazeCreateor_Dig DT;

    private int[,] Maze;//迷路配列
    private int[,] EndPosition; //終端位置の情報

    private List<Vector3Int> SetMapPosition = new List<Vector3Int>();//プリセットマップをセットする位置候補

    // Start is called before the first frame update
    void Start()
    {
        //タイルマップのコンポーネント取得
        tilemap = StageGrid.GetComponent<Tilemap>();
        tilemap.CompressBounds();//マップがどの環境でも必ず中央にセットされるようにセットアップ

        //迷路のサイズをセット
        DT = new DigTilemap.MazeCreateor_Dig(Width, Height,thickness);

        //迷路データの生成
        Maze = DT.CreateMaze();

        EndPosition = DT.EndPosition;

        //タイルマップのサイズ調整
        tilemap.size = new Vector3Int(DT.Width * DT.Thickness, DT.Height * DT.Thickness, tilemap.size.z);

        //マップに迷路データの適用
        CellMap();

        //タイルマップの位置調整
        //tilemap.CompressBounds();
        StageGrid.transform.position = new Vector3(-tilemap.size.x / 2, -tilemap.size.y / 2, 0);
    }

    //迷路配列をセット
    //タイルセットの処理は後で消す
    void CellMap()
    {
        var Cellbound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //迷路の位置参照用
        int mx,my;
        mx = my = 0;
       
        //タイルの設置、太さがあれば、太さ分データのタイルを引き離し、引き離した分タイルを大きくする。
        for (int y = Cellbound.max.y - 1 - ((DT.Thickness - 1) / 2); y >= Cellbound.min.y; y -= DT.Thickness)
        {//左上から右下にかけてタイルを監査する

            for (int x = Cellbound.min.x + ((DT.Thickness - 1) / 2); x < Cellbound.max.x; x += DT.Thickness)
            {
                //壁（１）であれば、埋める
                if (Maze[mx, my] == 1)
                {
                    //指定幅を１以上指定していれば、周りも埋める
                    for (int ey = y + ((DT.Thickness - 1) / 2); ey >= y - ((DT.Thickness - 1) / 2); ey--)
                    {
                        for (int ex = x - ((DT.Thickness - 1) / 2); ex <= x + ((DT.Thickness - 1) / 2); ex++)
                        {
                            //参照するブロック
                            var position = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(position, Basetile);
                        }
                    }
                }

                if(EndPosition[mx,my] == 2)
                {
                    //参照するブロック
                    var position = new Vector3Int(x, y, 0);
                    tilemap.SetTile(position, Chaintile);//デバッグ用 

                    //指定された位置を中心にプリセットマップを設置する候補に入れる
                    SetMapPosition.Add(position);
                }

                mx++;
            }

            mx = 0;
            my++;
        }
        
        //マップにプリセットマップを設置する
        TranscriptionMaps(Cellbound);
    }

    //セットしたプリセットマップをマップに転写する
    private void TranscriptionMaps(BoundsInt Cellbound)
    {
        while (true)
        {
            int index = Random.Range(0, PresetMaps.Count);
            var PreCell = PresetMaps[index].GetComponent<Tilemap>();
            PreCell.CompressBounds();
            var PreCellbound = PresetMaps[index].GetComponent<Tilemap>().cellBounds;



            //プリマップのブロックの位置参照
            int mx, my;
            my = PreCellbound.max.y - 1;
            mx = PreCellbound.min.x;

            int posindex = Random.Range(0, SetMapPosition.Count);
            var Setpos = SetMapPosition[posindex];//迷路の随所のマップ設置位置
            tilemap.SetTile(Setpos, AddMaptile);

            PresetMaps.RemoveAt(index);
            SetMapPosition.RemoveAt(index);

            //Vector3Int[,] PrePosition = new Vector3Int[PreCell.size.x, PreCell.size.y];
            TileBase[,] PreTile = new TileBase[PreCell.size.x, PreCell.size.y];

            //プリセットマップの中身を消す
            bool EraseTile = false;

            for (int y = Setpos.y + PreCellbound.max.y - 1; y >= Setpos.y + PreCellbound.min.y; y--)
            {
                var LeftWallposition = LeftWallPosition(PreCell, my);
                var RightWallposition = RightWallPosition(PreCell, my);

                for (int x = Setpos.x + PreCellbound.min.x; x < Setpos.x + PreCellbound.max.x; x++)
                {



                    //Debug.Log("LeftWallposition = " + LeftWallposition);
                    //Debug.Log("RightWallposition = " + RightWallposition);
                    if (EraseTile)
                    {
                        var position = new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);
                        tilemap.SetTile(position, null);

                        //LeftWallPosition 略してLWP
                        /*var LWP = new Vector3Int(LeftWallposition.x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);

                        var RWP = new Vector3Int(LeftWallposition.x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);
                        Debug.Log("LeftWallposition.x + 2 - 1 = " + (LeftWallposition.x + 2 - 1)
                        + "\nLeftWallposition.y + y - 2 - (PreCell.size.y % 2) + 1 = " + ( y - 2 - (PreCell.size.y % 2) + 1)
                        + "\nx + 2 + (PreCell.size.x % 2) - 1 = " + (x + 2 + (PreCell.size.x % 2) - 1)
                        + "\ny - 2 - (PreCell.size.y % 2) + 1 = " + (y - 2 - (PreCell.size.y % 2) + 1));
                        if (LWP == position)
                        {
                            Debug.Log("あってる");
                        }*/



                        //Debug.Log("position = " + position);
                        //くり抜く部分のマップを登録
                        //PrePosition[mx, my] = position;
                        //if(TileCheck(PreCellbound, PreCell, new Vector3Int(mx, my, 0))) PreTile[mx, my] = AddMaptile;
                    }

                    //参照するプリマップのブロックの位置にブロックがあるかチェック
                    if (TileCheck(PreCellbound, PreCell, new Vector3Int(mx, my, 0)))
                    {
                        var position = new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);

                        /*//設置する位置にタイルマップのブロックがすでにあれば、消す
                        if (!tilemap.GetTile(position))
                        {
                            tilemap.SetTile(position, null);
                        }
                        else
                        {
                            tilemap.SetTile(position, AddMaptile);
                        }*/

                        //ただマップを転写するだけならこの一行をアクティブ化、上のif文をコメントアウトする
                        tilemap.SetTile(position, AddMaptile);

                    }

                    //プリセットマップの中身を消すための真偽値を設定
                    /*if (LeftWallposition == new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0))
                    {
                        EraseTile = true;
                    }

                    if(RightWallposition == new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0))
                    {
                        EraseTile = false;
                    }
    */
                    if (!TileCheck(PreCellbound, PreCell, new Vector3Int(mx + 1, my, 0)))
                    {
                        EraseTile = true;
                    }
                    else
                    {
                        EraseTile = false;
                    }


                    mx++;
                }
                //設定をリセットする
                EraseTile = false;

                mx = PreCellbound.min.x;
                my--;
            }

            if (PresetMaps.Count <= 0 || SetMapPosition.Count <= 0) break;
        }

        /*for (int y = Cellbound.max.y - 1 - ((DT.Thickness - 1) / 2); y >= Cellbound.min.y; y -= DT.Thickness)
        {//左上から右下にかけてタイルを監査する

            for (int x = Cellbound.min.x + ((DT.Thickness - 1) / 2); x < Cellbound.max.x; x += DT.Thickness)
            {
                var position = new Vector3Int(x, y, 0);

                if (Setpos == position)
                {
                    for (int ey = PreCellbound.max.y - 1; y >= PreCellbound.min.y; y--)
                    {//左上から右下にかけてタイルを監査する

                        for (int ex = PreCellbound.min.x; x < PreCellbound.max.x; x++)
                        {
                            var Preposition = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(Preposition, Basetile);
                        }
                    }
                    *//*for (int ey =  PreCellbound.max.y - y; ey >= PreCellbound.min.y; ey--)
                    {
                        for (int ex = PreCellbound.min.x + x; ex <= PreCellbound.max.x; ex++)
                        {
                            //参照するブロック
                            //var position = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(position, Basetile);
                        }
                    }*//*
                }


                mx++;
            }

            mx = 0;
            my++;
        }*/
    }

    private bool TileCheck(BoundsInt Prebound, Tilemap Pretilemap,Vector3Int position)
    {
        TileBase tile = Pretilemap.GetTile(position);

        if (tile != null) return true;
        return false;
    }

    //参照する行の、左側の壁を検出する
    private Vector3Int LeftWallPosition(Tilemap PreTilemap,int y)
    {
        var bound = PreTilemap.cellBounds;

        for (int x = bound.min.x; x < bound.max.x; ++x)
        {
            //タイルの取得
            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
            TileBase tile2 = tilemap.GetTile(new Vector3Int(x + 1, y, 0));

            if (tile != null && tile2 == null)
            {
                return new Vector3Int(x, y, 0);
            }
        }

        return new Vector3Int(999, 999, 0);
    }

    //参照する行の、右側の壁を検出する
    private Vector3Int RightWallPosition(Tilemap PreTilemap, int y)
    {
        var bound = PreTilemap.cellBounds;

        for (int x = bound.max.x; x > bound.min.x; --x)
        {
            //タイルの取得
            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
            TileBase tile2 = tilemap.GetTile(new Vector3Int(x - 1, y, 0));

            if (tile != null && tile2 == null)
            {
                return new Vector3Int(x, y, 0);
            }
        }

        return new Vector3Int(999, 999, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
