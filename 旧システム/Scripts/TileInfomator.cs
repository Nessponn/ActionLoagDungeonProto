using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileInfomator : MonoBehaviour
{

    public Tilemap TileStage;

    private class TileInfo
    {
        public readonly Vector3Int m_position;
        public readonly TileBase m_tile;

        public TileInfo(Vector3Int position, TileBase tile)
        {
            m_position = position;
            m_tile = tile;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private float t = 0;
    // Update is called once per frame
    void Update()
    {
         t -= Time.deltaTime;
        if (t <= -1f)
        {
            t = 0;
            Shift(Vector2Int.right);

        }
        this.transform.position = new Vector3(t, this.transform.position.y, this.transform.position.z);
    }
    private  void Shift(Vector2Int offset)
    {
        var tilemap = GetComponent<Tilemap>();
        var bound = TileStage.cellBounds;

        var list = new List<TileInfo>();

        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var position = new Vector3Int(x, y, 0);
                if(position.x >= bound.min.x)
                {
                    var tile = TileStage.GetTile(position);
                    var info = new TileInfo(position, tile);
                    list.Add(info);
                }
            }
        }

        if (list.Count <= 0) return;

        //Undo.RecordObject(tilemap, "Shift Tilemap");

        TileStage.ClearAllTiles();

        foreach (var data in list)
        {
            var position = data.m_position;
            TileStage.SetTile(position, data.m_tile);
        }

        TileStage.RefreshAllTiles();
    }
}
