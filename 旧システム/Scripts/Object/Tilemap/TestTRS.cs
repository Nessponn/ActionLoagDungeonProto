using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestTRS : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var bound = GetComponent<Tilemap>().cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));//各タイルのスプライトを求める
                if (tile != null)//上で宣言したtileのspliteをsplitelistにぶち込む。もしも既に追加したspriteが存在する場合、falseとして追加をしない
                {

                    //GetTileData(ref tile);

                    //spriteList.Add(tile.sprite);//ここでタイルに使われているスプライトの登録を行う。既に追加されているスプライトはif文の時点で除去される
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetTileData(ref TileData tileData)
    {
        var m = tileData.transform;
        m.SetTRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 0f), Vector3.one);
        tileData.transform = m;

        //Quaternion rot = Quaternion.Euler(0.0f, 0.0f, rotation);
        //tileData.transform = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);

        //var m = tileData.transform;
        // m.SetTRS(Vector3.zero, GetRotation((byte)mask), Vector3.one);
        //m = tileData.transform;
    }

}
