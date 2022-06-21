using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Player : MonoBehaviour,IDamagable//操作に関する処理はここに書く
{
    public AudioSource AS;
    public AudioClip AC;

    //変数
    [SerializeField] public int HP = 1;
    public int Hover_Count = 3;//デフォルトは４回まで。それ以上のホバリングは出力が弱くなってしまう。
    [SerializeField] public int[] Rocket_VelocityPower = new int[4];//ホバリングするときの飛ぶ力を個々に調節する。

    //真偽変数
    [SerializeField] private bool Jump;
    
    //コンポーネント
    private Rigidbody2D rbody;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (rbody.velocity.y > 20) rbody.velocity = new Vector2(rbody.velocity.x, 20);
        if (rbody.velocity.y < -25) rbody.velocity = new Vector2(rbody.velocity.x, -25);


    }

    public void TakeDamage(int Damage)
    {
        //Debug.Log("プレイヤーが何かにあたった");
        HP -= Damage;
        if (HP == 0) StartCoroutine(Miss_Move());//HPが０になったら１ミス
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        string layername = LayerMask.LayerToName(col.gameObject.layer);

        if (layername == "Floor") Jump = true;
        Hovering_Power = 3;
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        string layername = LayerMask.LayerToName(col.gameObject.layer);

        if (layername == "Floor") Jump = false;
    }

    public IEnumerator Miss_Move()
    {
        AS.PlayOneShot(AC);//効果音を鳴らす




        GetComponent<PlayerController>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        rbody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.3f);
        rbody.constraints = RigidbodyConstraints2D.None;
        
        rbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        rbody.velocity = new Vector2(0, 8);

        yield return new WaitForSeconds(3f);
        HP = 1;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        rbody.velocity = new Vector2(0, 0);
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        var gamemaster = GameObject.FindWithTag("GameMaster");//ゲームマスターを探す
        //if (gamemaster != null) StartCoroutine(gamemaster.GetComponent<GameMaster>().Player_Restart());
    }

    public bool Jump_Hoge//は？これだけでゲッターとセッターの役割果たすんか？便利すぎ楽すぎ
    {
        get { return Jump; }
        set { Jump = value; }
    }

    public int Hovering_Power
    {
        set { Hover_Count = value; }
        get
        {
            int Power;
            if (Hover_Count == 3) Power = Rocket_VelocityPower[3];
            else if (Hover_Count == 2) Power = Rocket_VelocityPower[2];
            else if (Hover_Count == 1) Power = Rocket_VelocityPower[1];
            else if (Hover_Count == 0) Power = Rocket_VelocityPower[0];
            else Power = 0;

            Hover_Count--; ;//こいつに１小さい数をセットさせる

            return Power;
        }
    }

    public int getCount()
    {
        return Hover_Count;
    }
}