using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BreakableObject : MonoBehaviour,TileDamagable
{
    //private int Hp = 1;

    [SerializeField] public TileBase[] TileSprites = new TileBase[0];//タイルの数だけ、そのタイルの命となる
    private List<Nullable<int>> _SpriteNumbers = new List<Nullable<int>>();//Nulllableでnullを実装可能なint型の配列を作ることが出来る！！... だけど今回はやめておく。

    //public Tilemap Stage;//表示するステージ

    // Start is called before the first frame update
    void Start()
    {
        Sprite();
        /*
        var builder = new StringBuilder();
        var bound = GetComponent<Tilemap>().cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                builder.Append(GetComponent<Tilemap>().HasTile(new Vector3Int(x, y, 0)) ? "■" : "□");
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
        */

        //スタートの時点で、すべてのタイルの情報を保存しておく

        //タイルの範囲を求める
        //

        //Debug.Log(builder.ToString());
    }

    void Sprite()
    {

        var bound = GetComponent<Tilemap>().cellBounds;
        var spriteList = new List<Sprite>();//この中に、GetSpriteで
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));//各タイルのスプライトを求める
                if (tile != null && !spriteList.Contains(tile.sprite))//上で宣言したtileのspliteをsplitelistにぶち込む。もしも既に追加したspriteが存在する場合、falseとして追加をしない
                {
                    spriteList.Add(tile.sprite);//ここでタイルに使われているスプライトの登録を行う。既に追加されているスプライトはif文の時点で除去される
                }
            }
        }
        // どの場所でそのSpriteが使われているかを出力
        //var builder = new StringBuilder();
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//もう一度tilemapの範囲を求める
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tile == null)
                {
                    //builder.Append("_");
                    _SpriteNumbers.Add(null);
                }
                else
                {
                    var index = spriteList.IndexOf(tile.sprite);
                    //builder.Append(spriteList.Count - index-1);//ここではナンバーの出力のみをしており、保存はしていない。
                    _SpriteNumbers.Add(spriteList.Count - index - 1);
                }
            }
            //builder.Append("\n");
        }
    }

    private float t = 0;
    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        if(t <= -1f)
        {
            t = 0;
            PassTile();
           
        }
        this.transform.position = new Vector3(t, this.transform.position.y, this.transform.position.z);
    }

    public void TakeDamage(Vector3 vec,int num)//爆風を受けた部分のスプライトを変更。またはnullにして消す
    {
        //Hp -= num;
        //if (Hp == 0) Destroy(this.gameObject);
        var tile = GetComponent<Tilemap>();
        Debug.Log("通っている");
        for(int i = 0; i < TileSprites.Length; i++)
        {
            if (tile.GetTile(tile.WorldToCell(vec)) == TileSprites[i])//いけるわ…
            {
                if (i - 1 >= 0) tile.SetTile(tile.WorldToCell(vec), TileSprites[i - 1]);//置き換え
                else tile.SetTile(tile.WorldToCell(vec), null);//OutRangeArrayになったらそのタイルは終了
            }
        }
        
    }

    private int distance = 0;
    public void PassTile()
    {
        //タイルの情報をずらす
        //0から-1担った瞬間、座標を0基準でタイル情報を左にずらす

        //タイルから何列目にいるか（y軸の値）の情報を引き出す
        int num = 0;
        Vector3Int vec;
        var bound = GetComponent<Tilemap>().cellBounds;

        _SpriteNumbers.Clear();
        Sprite();
        //GetComponent<Tilemap>().ClearAllTiles();
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
        {
            for (int x = bound.min.x ; x < bound.max.x; ++x)
            {
                //Debug.Log("x = " + x);
                //タイルの情報を下記の１行のコードに格納する
                //タイルの座標、メソッドだと思っていた部分で取れるのビビったんやけど…
                var tile = GetComponent<Tilemap>().GetTile<Tile>(vec = new Vector3Int(x, y, 0));
                vec.x -= 1;
              
                if(_SpriteNumbers[num] >= 0 && vec.x >= bound.min.x)
                {
                    //ここでタイルセット
                   
                    GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x, y, 0), TileSprites[(int)_SpriteNumbers[num]]);
                    GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x + 1, y, 0), null);
                }
                else
                {
                    GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x + 1, y, 0), null);
                }
                num++;
            }
        }
        GetComponent<Tilemap>().RefreshAllTiles();
        bool insert = false;
        //動かした後は毎回、最後列五行に何も入っていないかチェック
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
        {
            for (int x = 15; x <= 20; ++x)//カメラから見えないくらいの範囲を調査
            {
                var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tile)
                {
                    //あれば挿入しない
                    insert = true;
                }
            }
        }

        //確認後、タイルが何かしらあったら挿入処理開始
        if (insert)
        {//一応ここまで、サイズはカメラのサイズ分のステージの追加の想定
         //インスペクターで登録したステージを付け足す処理

            num = 0;
            //var Stagebound = Stage.GetComponent<Tilemap>().cellBounds;
            for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
            {
                //Stageboundsのy軸はboundsと同じ値を扱うので省略


                //新たにステージタイルにタイル情報の書き込み(_SpriteNumbersに追加でステージ登録　を行う)


                for (int x = 15; x <= bound.max.x; ++x)//カメラから見えないくらいの範囲を調査
                     {
                           var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));

                           if (_SpriteNumbers[num] == null)
                           {
                               GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), null);
                           }
                           else
                           {
                              //ここでタイルセット
                              GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), TileSprites[(int)_SpriteNumbers[num]]);
                           }
                            
                    }
            }
        }



        distance ++;
    }

    public void RefreshTileState()//リフレッシュ
    {
        //計算式「spriteList - index」これで数字の逆数を求められる
        //それを用いて、SetTileであとは設置するだけ
        Debug.Log("修正実行開始");
        distance = 0;
        int num = 0;
        var bound = GetComponent<Tilemap>().cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));
                if(_SpriteNumbers[num] == null)
                {
                    GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), null);
                }
                else
                {
                    //ここでタイルセット
                    GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), TileSprites[(int)_SpriteNumbers[num]]);
                }
                num++;
            }
        }
    }
}
