using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Before : MonoBehaviour
{
    //クラス
    private Player P;

    //変数
    private float speed = 0;
    public float FireFlower_Power = 2;

    

    public GameObject Arrow;

    //オブジェクト
    public GameObject Sphere;//発射する玉。

    public enum Player_State
    {
        Stop, Left, Right
    }
    Player_State PS = Player_State.Stop;

    //コンポーネント
    private Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        P = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = Arrow.transform.localRotation;
        rot.z = -Degree();
        Arrow.transform.localRotation = Quaternion.Euler(0, 0, rot.z);

        //Debug.Log(rot.z);

        if (Input.GetKey(KeyCode.A)) PS = Player_State.Left;

        else if (Input.GetKey(KeyCode.D)) PS = Player_State.Right;

        else if (Input.GetKeyUp(KeyCode.A)) PS = Player_State.Stop;

        else if (Input.GetKeyUp(KeyCode.D)) PS = Player_State.Stop;

        if (Input.GetKeyDown(KeyCode.W) && P.Jump_Hoge) rbody.velocity = new Vector2(rbody.velocity.x, 10);

        if (Input.GetMouseButtonDown(0)) Instantiate_Sphere_Bomb();

        //if (Input.GetKeyDown(KeyCode.Space)) rbody.velocity = new Vector2(rbody.velocity.x, 80);

        speed = rbody.velocity.x;

        switch (PS)
        {
            case Player_State.Stop:
                if (speed > 0) speed -= 0.25f;
                else if (speed < 0) speed += 0.25f;
                else speed = 0;
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
                break;
            case Player_State.Left:
                //rbody.AddForce(new Vector2(-100, 0));
                
                speed--;
                if (speed <= -7) speed = -7;
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
                break;
            case Player_State.Right:
                //rbody.AddForce(new Vector2(100, 0));
                
                speed++;
                if (speed >= 7) speed = 7;
                rbody.velocity = new Vector2(speed, rbody.velocity.y);
                break;

        }

        
    }

    public Quaternion Arrow_Angle(float angle)
    {
        Quaternion rot = Arrow.transform.rotation;
        rot.z = -angle;
        return Arrow.transform.rotation; 
    }

    public void Instantiate_Sphere_Bomb()//爆弾を生成し、飛ばす
    {
        //命令を付与するため、GameObject型を宣言する
        GameObject obj = Instantiate(Sphere, this.transform.position, Quaternion.identity);

        float Real_Degree = Degree();//実際に適用する角度

        //玉を　クリックした方向に向けて飛ばす
        obj.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -Degree()) * Vector3.up * (90 / (2 / FireFlower_Power));

        //プレイヤーを、反動で逆の方へ飛ばす
        if((Degree() <70 || Degree() > 290))
        {
            rbody.velocity = Quaternion.Euler(0, 0, -Degree()) * Vector2.down * 20;
            //Debug.Log("威力弱の時に上向きに発射");
        }
        else if(P.Hover_Count < 0)
        {
            rbody.velocity = rbody.velocity;
        }
        else
        {
            rbody.velocity = Quaternion.Euler(0, 0, -Degree()) * Vector2.down * P.Hovering_Power;
            //Debug.Log("通常攻撃");
            //Debug.Log("P.Hovering_Power ="+ P.getCount());
        }
        //Debug.Log(rbody.velocity);
        //Debug.Log(Degree());
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


    private void Item_Use()//スペースボタンが押されたときに発動。
    {

    }
}
