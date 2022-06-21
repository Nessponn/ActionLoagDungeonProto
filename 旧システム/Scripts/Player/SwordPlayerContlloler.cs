using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerContlloler : MonoBehaviour
{
    //常駐効果以外の、操作に関する入力をいつでも発動、切断を切り替えるスクリプト

    //操作説明
    //WASDで移動、
    //何も操作しなければ自動で歩き続ける
    //マウスで空中ダッシュ、上異動でジャンプ、右に移動すると自動的にダッシュ攻撃が発動する

    //スマッシュスライダー…空中にいるときクリックで発動

    private enum _Move
    {
        Left,Right,Stop
    }

    private _Move _movedir = _Move.Stop;

    [SerializeField] private float JumpPower;
    [SerializeField] private float Speed = 80;
    [SerializeField] private float Speed_Air = 20;
    [SerializeField] private float MaxSpeed = 5;

    public GameObject Arrow;

    private Rigidbody2D rbody;
    private Animator anim;
    private ParticleSystem particle;
    public ParticleSystem Dashparticle;


    //状態管理
    private bool FloorTaken;
    private bool SmashAttacker;//空中で発動できる切り付け攻撃の状態管理。射程距離の概念もある
    private bool DashAttacker;//右おしっぱかつ地上にいるとき発動していられるダッシュアタックの状態


    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = Arrow.transform.localRotation;
        rot.z = -Degree();
        Arrow.transform.localRotation = Quaternion.Euler(0, 0, rot.z);

        //移動やジャンプに関する入力処理

        //攻撃入力を最優先
        if (Input.GetKey(KeyCode.D))
        {
            _movedir = _Move.Right;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _movedir = _Move.Stop;
        }
        //ジャンプ入力は二の次
        if (Input.GetKeyDown(KeyCode.W))
        {
            //着地しているとき、ジャンプ入力がなされればジャンプする
            if(FloorTaken) rbody.velocity = new Vector2(rbody.velocity.x, JumpPower);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _movedir = _Move.Left;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _movedir = _Move.Stop;
        }
        if (Input.GetKey(KeyCode.S))
        {
            
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            
        }

        //ジャンプアニメーション
        //if(!FloorTaken) anim.SetFloat("velocity_y", rbody.velocity.y);


        if (Camera.main.transform.localPosition.x - this.transform.localPosition.x <= -4)
        {
            switch (_movedir)
            {

                case _Move.Left:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(-Speed * 2, rbody.velocity.y));
                        Dashparticle.Play();

                        //アニメーション関連の処理
                        //アニメーションに関する処理
                        anim.SetBool("Standing", true);
                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(-Speed_Air * 2, rbody.velocity.y));
                        Dashparticle.Stop();



                        //アニメーション関連の処理
                        anim.SetBool("Standing", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x <= -MaxSpeed + 2.2f)
                        rbody.velocity = new Vector2(-MaxSpeed + 2.2f, rbody.velocity.y);

                    //アニメーションに関する処理
                    anim.SetBool("FloorTaken", FloorTaken);


                    //パーティクルに関する処理


                    break;
                case _Move.Right:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(Speed / 2, rbody.velocity.y));
                        anim.SetBool("Standing", false);
                        anim.SetBool("Running", true);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(Speed_Air /2, rbody.velocity.y));
                        Dashparticle.Stop();

                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x >= MaxSpeed / 2)
                        rbody.velocity = new Vector2(MaxSpeed / 2, rbody.velocity.y);
                    //アニメーションに関する処理


                    //パーティクルに関する処理
                    break;
                case _Move.Stop:
                    Dashparticle.Stop();
                    anim.SetBool("Standing", false);
                    anim.SetBool("Running", false);
                    anim.SetBool("FloorTaken", FloorTaken);

                    //止まっているときは坂の上でも滑らないようにする
                    if (rbody.velocity.x <= 0) rbody.velocity = new Vector2(0, rbody.velocity.y);
                    break;
            }
        }
        else if (Camera.main.transform.localPosition.x - this.transform.localPosition.x <= -1)
        {
            switch (_movedir)
            {
                case _Move.Left:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(-Speed/2, rbody.velocity.y));
                        Dashparticle.Play();

                        //アニメーション関連の処理
                        //アニメーションに関する処理
                        anim.SetBool("Standing", true);
                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(-Speed_Air/2, rbody.velocity.y));
                        Dashparticle.Stop();



                        //アニメーション関連の処理
                        anim.SetBool("Standing", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x <= -MaxSpeed )
                        rbody.velocity = new Vector2(-MaxSpeed , rbody.velocity.y);

                    //アニメーションに関する処理
                    anim.SetBool("FloorTaken", FloorTaken);


                    //パーティクルに関する処理


                    break;
                case _Move.Right:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(Speed/1.5f, rbody.velocity.y));
                        anim.SetBool("Standing", false);
                        anim.SetBool("Running", true);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(Speed_Air/1.5f, rbody.velocity.y));
                        Dashparticle.Stop();

                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x >= MaxSpeed)
                        rbody.velocity = new Vector2(MaxSpeed, rbody.velocity.y);
                    //アニメーションに関する処理


                    //パーティクルに関する処理
                    break;
                case _Move.Stop:
                    Dashparticle.Stop();
                    anim.SetBool("Standing", false);
                    anim.SetBool("Running", false);
                    anim.SetBool("FloorTaken", FloorTaken);

                    //止まっているときは坂の上でも滑らないようにする
                    if (rbody.velocity.x <= 0) rbody.velocity = new Vector2(0, rbody.velocity.y);
                    break;
            }
        }
        else
        {
            switch (_movedir)
            {

                case _Move.Left:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(-Speed/2 , rbody.velocity.y));
                        Dashparticle.Play();

                        //アニメーション関連の処理
                        //アニメーションに関する処理
                        anim.SetBool("Standing", true);
                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(-Speed_Air/2, rbody.velocity.y));
                        Dashparticle.Stop();



                        //アニメーション関連の処理
                        anim.SetBool("Standing", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x <= -MaxSpeed)
                        rbody.velocity = new Vector2(-MaxSpeed, rbody.velocity.y);

                    //アニメーションに関する処理
                    anim.SetBool("FloorTaken", FloorTaken);


                    //パーティクルに関する処理


                    break;
                case _Move.Right:

                    //移動に関する処理
                    if (FloorTaken)
                    {
                        rbody.AddForce(new Vector2(Speed, rbody.velocity.y));
                        anim.SetBool("Standing", false);
                        anim.SetBool("Running", true);
                        anim.SetBool("FloorTaken", true);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }
                    else
                    {
                        rbody.AddForce(new Vector2(Speed_Air, rbody.velocity.y));
                        Dashparticle.Stop();

                        anim.SetBool("Running", false);
                        anim.SetBool("FloorTaken", false);
                        anim.SetFloat("velocity_y", rbody.velocity.y);
                    }

                    if (rbody.velocity.x >= MaxSpeed)
                        rbody.velocity = new Vector2(MaxSpeed, rbody.velocity.y);
                    //アニメーションに関する処理


                    //パーティクルに関する処理
                    break;
                case _Move.Stop:
                    Dashparticle.Stop();
                    anim.SetBool("Standing", false);
                    anim.SetBool("Running", false);
                    anim.SetBool("FloorTaken", FloorTaken);

                    //止まっているときは坂の上でも滑らないようにする
                    if (rbody.velocity.x <= 0) rbody.velocity = new Vector2(0, rbody.velocity.y);
                    break;
            }
        }
        //移動に関する処理
        

        if (SmashAttacker && !FloorTaken)//着地するまで、一度もスマッシュアタックを使っていなかったら発動可能
        {
            if (Input.GetMouseButtonDown(0))
            {
                SmashAttacker = false;
                Instantiate_Sphere_Bomb();
            }
        }


    }

    //アニメーションイベントを使用して使う
    public void particle_play()
    {
        particle.Play();
    }

    /*
            anim.SetBool("",true);
            anim.SetBool("",false);
    */
    private void Instantiate_Sphere_Bomb()//爆弾を生成し、飛ばす
    {
        //命令を付与するため、GameObject型を宣言する
        //GameObject obj = Instantiate(Sphere, this.transform.position, Quaternion.identity);

        //玉を　クリックした方向に向けて飛ばす
        //obj.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -Degree()) * Vector3.up * (90 / (2 / FireFlower_Power));

        //プレイヤーを、反動で逆の方へ飛ばす
        if ((Degree() < 70 || Degree() > 290))
        {
            rbody.velocity = Quaternion.Euler(0, 0, -Degree()) * Vector2.up * (JumpPower * 1.5f);

           
            anim.SetBool("Smash_Up", true);
            anim.SetBool("Smash_Down", false);
            //Debug.Log("威力弱の時に上向きに発射");
        }
        else
        {
            rbody.velocity = Quaternion.Euler(0, 0, -Degree()) * Vector2.up * (JumpPower * 1.5f);

            anim.SetBool("Smash_Down", true);
            anim.SetBool("Smash_Up", false);
            //Debug.Log("通常攻撃");
            //Debug.Log("P.Hovering_Power ="+ P.getCount());
        }
        //Debug.Log(rbody.velocity);
        //Debug.Log(Degree());
        StartCoroutine(Dash_());
    }


    private IEnumerator Dash_()
    {
        yield return new WaitForSeconds(0.3f);
        rbody.velocity *= 0.1f;
        anim.SetBool("Smash_Down", false);
        anim.SetBool("Smash_Up", false);
    }

    private Quaternion Arrow_Angle(float angle)
    {
        Quaternion rot = Arrow.transform.rotation;
        rot.z = -angle;
        return Arrow.transform.rotation;
    }

    private float Degree()//マウスをクリックした位置を返す。//矢印の角度を決める。クリックでその方向に弾を放物線上に飛ばす（試作段階では直線に飛ばすのもアリ）
    {
        //クリックした瞬間のであろうマウスの位置とこのオブジェクトの角度を計算する

        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //float dx = mousepos.x - this.transform.position.x;
        //float dy = mousepos.y - this.transform.position.y;

        //float rad = Mathf.Atan2(dy, dx);

        Vector3 dt = mousepos - this.transform.position;

        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;

        if (degree < 0)
        {
            degree += 360;
        }

        return degree;

        //Debug.Log("world = "+this.transform.position);
        //Debug.Log("screen = "+Camera.main.ScreenToWorldPoint(this.transform.position));
        //return rad * Mathf.Rad2Deg +180;


    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        string layername = LayerMask.LayerToName(col.gameObject.layer);

        if(layername == "floor")
        {
            SmashAttacker = true;
            FloorTaken = true;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        string layername = LayerMask.LayerToName(col.gameObject.layer);

        if (layername == "floor")
        {
            SmashAttacker = true;
            FloorTaken = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        string layername = LayerMask.LayerToName(col.gameObject.layer);

        if (layername == "floor")
        {
            FloorTaken = false;
        }
    }
}
