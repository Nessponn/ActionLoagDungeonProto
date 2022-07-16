using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DigMaze;

public class CreateDigTilemap : MonoBehaviour
{
    [Range(5, 121)] public int Width;
    [Range(5, 121)] public int Height;
    [Range(1, 3)] public int thickness;

    public GameObject StageGrid;

    public TileBase Basetile;//壁や床の元となるタイル
    public TileBase Chaintile;//連結部の生成位置（デバッグ用）

    private Tilemap tilemap;

    private DigTilemap.MazeCreateor_Dig DT;

    private int[,] Maze;//迷路配列

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

        //タイルマップのサイズ調整
        tilemap.size = new Vector3Int(DT.Width * DT.Thickness, DT.Height * DT.Thickness, tilemap.size.z);

        //マップに迷路データの適用
        CellMap();

        //タイルマップの位置調整
        tilemap.CompressBounds();
        StageGrid.transform.position = new Vector3(-tilemap.size.x / 2, -tilemap.size.y / 2, 0);
    }

    //迷路配列をセット
    void CellMap()
    {
        var Celltilemap = StageGrid.GetComponent<Tilemap>();
        var Cellbound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //迷路の位置参照用
        int mx,my;
        mx = my = 0;

        for (int y = Cellbound.max.y - 1; y >= Cellbound.min.y; --y)
        {//左上から右下にかけてタイルを監査する

            for (int x = Cellbound.min.x; x < Cellbound.max.x; ++x)
            {
                //参照するブロック
                var position = new Vector3Int(x, y, 0);

                //壁（１）であれば、埋める
                if(Maze[mx,my] == 1) tilemap.SetTile(position, Basetile);

                mx++;
            }

            mx = 0;
            my++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
