using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    //ゲームオブジェクト（private組）
    private GameObject[] StageGrid;
    //変数
    private float t = 0;//時間。

    private int distance = 1;
    private int total_distance = 0;

    private static int result_distance = 0;
    public static int result_enemydisturb = 0;

    private int rest = 2;//プレイヤー残機。どうせマイナス台に突入する。

    private Vector3 RespaenPosition;//リスタートポジションを記憶しておくとこ。Hogeで入出力をする

    public GameObject WhiteScreen;

    public string ResultScene;

    public Text distance_text;

    private bool StageCrach = false;//クラッチみたいに進行か停止かを変える
    public GameObject GameClear_text;

    // Start is called before the first frame update
    void Start()
    {
        StageGrid = GameObject.FindGameObjectsWithTag("Tile");
        UpdateRecord();

        result_distance = 0;
        result_enemydisturb = 0;
    }
    public int Result_distance
    {
        get { return result_distance; }
        set { result_distance = value; }
    }

    public int Result_enemy
    {
        get { return result_enemydisturb; }
        set { result_enemydisturb = value; }
    }
    public void enemyresultAdd()
    {
        result_enemydisturb++;
    } 

    void UpdateRecord()
    {
        distance_text.text = " "+total_distance+"m";
    }

    public bool StageCrach_prop
    {
        get { return StageCrach; }
        set { StageCrach = value; }
    }

    // Update is called once per frame
    void Update()
    {
        //Playerの位置は毎回変わるので、ここで。
        var Player = GameObject.FindWithTag("Player");
        //if (!StageCrach)
        //{
            t -= Time.deltaTime * distance;
        //}
        if (t <= -distance)
        {
            //Debug.Log("value = " + (Camera.main.transform.localPosition.x - Player.transform.localPosition.x));
            PassTile();
            total_distance += distance;
            UpdateRecord();
            if (Camera.main.transform.localPosition.x - Player.transform.localPosition.x <= -4)
            {
                distance = 7;
            }
            else if (Camera.main.transform.localPosition.x - Player.transform.localPosition.x <= -2)
            {
                distance = 5;
            }
            else if (Camera.main.transform.localPosition.x - Player.transform.localPosition.x <= 4)
            {
                distance = 3;
            }
            else
            {
                distance = 1;
            }

            if(total_distance >= 450)
            {
                GameClear();
            }

            t = 0;
        }
        for(int i = 0;i< StageGrid.Length; i++)
        {
            if (StageGrid[i].GetComponent<Tilemap>())
            {
                StageGrid[i].transform.position = new Vector3(t, StageGrid[i].transform.position.y, StageGrid[i].transform.position.z);
            }
            else
            {
                StageGrid[i].transform.position -= new Vector3((float)distance/40 ,0, 0);
            }
        }
    }
    private class TileInfo
    {
        public readonly Vector3Int m_position;
        public readonly Vector3 m_rotation;
        public readonly TileBase m_tile;

        public TileInfo(Vector3Int position, Vector3 rotation, TileBase tile,int distance)
        {
            position.x -= distance;
            m_position = position;
            m_rotation = rotation;
            m_tile = tile;
        }
    }

    public void PassTile()
    {
        //タイルの情報をずらす
        //0から-1担った瞬間、座標を0基準でタイル情報を左にずらす

        for(int gridnum = 0;gridnum < StageGrid.Length; gridnum++)
        {
            //タイルから何列目にいるか（y軸の値）の情報を引き出す
            int num = 0;
            Vector3Int vec;
            if (StageGrid[gridnum].GetComponent<Tilemap>())
            {

                var tilemap = StageGrid[gridnum].GetComponent<Tilemap>();
                var bound = StageGrid[gridnum].GetComponent<Tilemap>().cellBounds;

                var list = new List<TileInfo>();

                //GetComponent<Tilemap>().ClearAllTiles();
                for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
                {
                    for (int x = bound.min.x; x < bound.max.x; ++x)
                    {
                        //Debug.Log("x = " + x);
                        //タイルの情報を下記の１行のコードに格納する
                        //タイルの座標、メソッドだと思っていた部分で取れるのビビったんやけど…
                        var tile = StageGrid[gridnum].GetComponent<Tilemap>().GetTile<Tile>(vec = new Vector3Int(x, y, 0));

                        vec.x -= 1;

                        var position = new Vector3Int(x, y, 0);
                        Vector3 rotation = tilemap.GetTransformMatrix(position).rotation.eulerAngles;
                        if (position.x >= bound.min.x)
                        {
                            var tile2 = tilemap.GetTile(position);

                            var info = new TileInfo(position, rotation, tile, distance);
                            list.Add(info);
                        }


                        if (list.Count <= 0) return;

                        /*
                        if (_SpriteNumbers[num] >= 0 && vec.x >= bound.min.x)
                        {
                            //ここでタイルセット
                            GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x, y, 0), TileSprites[(int)_SpriteNumbers[num]]);
                            GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x + 1, y, 0), null);
                        }
                        else
                        {
                            GetComponent<Tilemap>().SetTile(new Vector3Int(vec.x + 1, y, 0), null);
                        }
                        */
                        num++;
                    }

                    foreach (var data in list)
                    {

                        var position = data.m_position;
                        var rotation = data.m_rotation;

                        if (position.x >= bound.min.x)
                        {
                            tilemap.SetTile(position, data.m_tile);
                            Matrix4x4 matrix = Matrix4x4.TRS(Vector3Int.zero, Quaternion.Euler(rotation), Vector3.one);
                            tilemap.SetTransformMatrix(position, matrix);
                        }
                        else
                        {
                            tilemap.SetTile(position, null);
                        }


                        position.x += 1;
                        tilemap.SetTile(position, null);
                    }
                }
            }
            /*
            //GetComponent<Tilemap>().RefreshAllTiles();
            bool insert = false;
            //動かした後は毎回、最後列五行に何も入っていないかチェック
            for (int y = bound.max.y - 1; y >= bound.min.y; --y)//左上から右下にかけてタイルを代入していく
            {
                for (int x = 15; x <= 20; ++x)//カメラから見えないくらいの範囲を調査
                {
                    var tile = StageGrid.GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));
                    if (tile)
                    {
                        //あれば挿入しない
                        insert = true;
                    }
                }
            }
            */
        }
    }
    public void Player_MissCount()
    {
        rest--;//残機が１減る
    }


    public void GameOver()
    {
        WhiteScreen.SetActive(true);
        result_distance = total_distance;

        var Data = GameObject.FindWithTag("Data").GetComponent<DataManager>();

        if(Data != null)
        {
            Data.Get_GameData(result_distance, result_enemydisturb);
        }

        StartCoroutine(GameOverScene());
    }


    public void GameClear()
    {
        GameClear_text.SetActive(true);

        StartCoroutine(GameOverScene());
    }

    private IEnumerator GameOverScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(ResultScene);
    }

    
    /*
    public Vector3 RespawnPoint_Hoge//リスポーン地点の更新、設定
    {
        get{ return RespaenPosition; }

        // 変更
        set { RespaenPosition = value; }
    }

    public IEnumerator Player_Restart()//特定のボタンを押したり、ミスして一定時間経過によりこのメソッドが実行される
    {
        Screen_Canvas.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        Screen_Canvas.SetActive(false);

        GameObject[] Tiles = GameObject.FindGameObjectsWithTag("BreakableTile");

        foreach(var T in Tiles)
        {
            Debug.Log("状態補正完了");
            //T.GetComponent<BreakableObject>().RefreshTileState();
        }
        Player.transform.position = RespawnPoint_Hoge; //プロパティを設定することで勝手にポジションを取ってきてくれる
    }
    */
}

[Serializable]
public class Boss
{
    
}
