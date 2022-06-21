using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]…実行したものが、エディターの再生を止めても残り続ける設定
public class EnemySpawner : MonoBehaviour
{
    //アクティブバランサーを考慮した設計にする（）

    //EnemyObjを発生させるためのもの。

    //スポナーにつけるもの
    //こいつ（EnemySpawner）、SpriteRenderer、エディター上にスプライトを表示させるための更新ボタン「スプライト更新」

    public EnemyData EnemyData;

    public GameObject EnemyBase;

    public RuntimeAnimatorController AnimatorController;

    private bool Respawn = true;

    private GameObject enemy;

    public bool DebugMode;

    public bool Boss;

    public AudioClip yarare;

    // Update is called once per frame
    void Update()
    {
        
        var Player = GameObject.FindWithTag("Player");
        //Debug.Log("this.gameObject.transform.position.x = " + this.gameObject.transform.position.x);
       // Debug.Log("this.gameObject.transform.localPosition.x = " + this.gameObject.transform.localPosition.x);
        Debug.Log("!enemy = " + enemy);
        Debug.Log("Respawn = " + Respawn);


        if (this.gameObject.transform.position.x < 15f)
        {//範囲内にプレイヤーがいるかどうか
            
            //敵が生成されていない、かつリスポーン設定がされていた場合
            if (!enemy && Respawn)
            {
                Respawn = false;
                EnemySpawn();
            }
            //生成後にRespawnの状態を上書きする（つまり一度はかならず生成されるようにする）
        }
        else
        {
            if (!enemy) Respawn = EnemyData.Respawn;
        }

    }

    void EnemySpawn()//敵を生成する
    {
        //あーガバガバ…
        //子オブジェクトに子オブジェクトはつけらんないので、仕方がないので敵の型のプレハブ作ります
        //プレハブの敵ベースとなるオブジェクトに様々なデータ情報を送り込むという形にします

        enemy = Instantiate(EnemyBase, this.transform.position, Quaternion.identity);

        enemy.transform.localPosition = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        enemy.GetComponent<CircleCollider2D>().isTrigger = true;
        //enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");

        //敵を形成する判定として必要なものを渡す
        //enemy.GetComponent<CircleCollider2D>().size = new Vector2(1, enemy.transform.localScale.y);
        enemy.GetComponent<CircleCollider2D>().offset = new Vector2(0, EnemyData.Circleoffset_y);
        enemy.GetComponent<CircleCollider2D>().radius = EnemyData.radius;
        


        if (EnemyData.BoxCol)
        {//着地を有効にする
            
            //敵の判定とは別に、着地を行う足の判定をつけるための判定
            enemy.GetComponentInChildren<BoxCollider2D>().size = new Vector2(EnemyData.Boxsize.x, EnemyData.Boxsize.y);
            enemy.GetComponentInChildren<BoxCollider2D>().offset = new Vector2(EnemyData.Boxoffset.x, EnemyData.Boxoffset.y);
            enemy.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        else
        {
            //着地を無効にし、床をすり抜けるようにする
            enemy.GetComponentInChildren<BoxCollider2D>().size = new Vector2(EnemyData.Boxsize.x, EnemyData.Boxsize.y);
            enemy.GetComponentInChildren<BoxCollider2D>().offset = new Vector2(EnemyData.Boxoffset.x, EnemyData.Boxoffset.y);
            enemy.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("EnemythroughFloor");
        }

        //リジッドボディーの設定
        enemy.GetComponent<Rigidbody2D>().mass = EnemyData.Mass;
        enemy.GetComponent<Rigidbody2D>().gravityScale = EnemyData.Gravity;

        if (EnemyData.constant_z)
        {
            if (EnemyData.constant_x && EnemyData.constant_y)
            {
                //ｘ、ｙ、ｚの全てが押されている
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else if (EnemyData.constant_y)
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }//この時点で、zは押されているが、ｙは押されていないということがわかる
            else if (EnemyData.constant_x)
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            //この時点で、zのみが押されているということが分かった
            else
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        //この時点でzが押されているということはなくなった
        else if (EnemyData.constant_y)
        {
            if (EnemyData.constant_x)
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            }
            //この時点で、ｙのみが押されているということが分かった
            else
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
            }
        }
        //この時点でｙが押されているということはなくなった
        else if (EnemyData.constant_x)
        {
            //ｘのみが押されている
            enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        //この時点で、zのみが押されているということが分かった
        else
        {
            enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }









        //SpriteRendererのアタッチ
        enemy.GetComponent<SpriteRenderer>().sprite = EnemyData.Image;


        //Animatorのアタッチ
        if(EnemyData.Animation)
        {
            enemy.AddComponent<Animator>();

            AnimatorOverrideController newAnime = new AnimatorOverrideController();

            newAnime.runtimeAnimatorController = AnimatorController;

            AnimationClipPair[] clipPairs = newAnime.clips;
            Debug.Log("newAnime.clips = " + newAnime.clips.Length);

            for (int i = 0; i < newAnime.clips.Length; i++)
            {
                // 今回はオリジナルクリップを名前で判別して差し替える
                if (newAnime.clips[i].originalClip.name.IndexOf("Waiting") >= 0 && EnemyData._move != null)
                {
                    clipPairs[i].overrideClip = EnemyData._move; // newClipはInspector側でアタッチしてる前提
                    Debug.Log("通った１");
                }

                if (newAnime.clips[i].originalClip.name.IndexOf("Jump") >= 0 && EnemyData._jump != null)
                {
                    clipPairs[i].overrideClip = EnemyData._jump; // newClipはInspector側でアタッチしてる前提
                    Debug.Log("通った２");
                }
            }
            newAnime.clips = clipPairs;

            // 差し替えたOverrideControllerをAnimatorに代入して完了
            enemy.GetComponent<Animator>().runtimeAnimatorController = newAnime;
        }






        if (Boss)
        {
            enemy.gameObject.tag = "Boss";

            var GM = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
            if(GM != null)
            {
                if (enemy)
                {
                    //ステージが進まなくなるようにする
                    GM.StageCrach_prop = true;
                }
            }
        }
        else
        {
            enemy.gameObject.tag = "Enemy";
        }
        //敵の行動パターンやデータを入れる.
        //データの送信は、メソッド開始時にStartメソッドで行われる




        



        enemy.AddComponent<EnemyObj>();
        enemy.GetComponent<EnemyObj>().Enemydata = EnemyData;
        enemy.GetComponent<EnemyObj>().yara = yarare;

        //エディターで表示しているスポナーのスプライトは消す
        this.GetComponent<SpriteRenderer>().enabled = false;
    }


    public void SetSprite()//エディター上でもスプライトの状態を確認できるもの
    {
        if(EnemyData != null)
        {
            GetComponent<SpriteRenderer>().sprite = EnemyData.Image;

            //スケールはスポナーのスケールと同じものにしても大丈夫
            //敵オブジェクトの方には、いいスケールデータの渡し方が今のところ思いつかないのでこのままで…
            //this.transform.localScale = new Vector3(EnemyData.EnemySpriteSetting.Scale, EnemyData.EnemySpriteSetting.Scale, 1);
        }
        else
        {
            Debug.LogError("そのスポナーには「EnemyData」がついてません！");
        }
    }

    public EnemyData GetEnemyData()
    {
        return EnemyData;
    }
}
