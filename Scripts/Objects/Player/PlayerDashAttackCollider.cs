using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttackCollider : MonoBehaviour,AttackableCollider 
{
    //private float Collider_LifeTime;//

    //private GameObject AttackColider;
    protected static SkillData dashskilldata;

    [System.NonSerialized] public int Current_ComboNumner;

    void Start()
    {
        dashskilldata = PlayerBaseManager.Instance.DashAttack;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        //Collider_LifeTime = dashskilldata.Collider_LifeTime;
    }

    public void Combo_Reset()
    {
        Current_ComboNumner = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        //コライダーの寿命が切れたら表示を消す

        //ここに表示を消す処理を書いているのは、飛び道具でもPlayerAttakColliderと切り離して考えられるようにするため

        //いったん削除
        /*
        Collider_LifeTime -= Time.deltaTime;
        if (Collider_LifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
        */
    }

    //バグっていた原因は、PlayerAttackColliderにおいていたOnTriggerEnter2Dが
    //継承先のこいつに反応してしまっていたから。

    public void ToEnemyDamage(GameObject obj)
    {
        Current_ComboNumner++;
        Debug.Log("Current_ComboNumner = " + Current_ComboNumner);
        if (Current_ComboNumner == dashskilldata.Max_Combo)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
            //Current_ComboNumner = 0;
            //Debug.Log("フィニッシュ");
        }
        else if (Current_ComboNumner == 1)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);

            //Debug.Log("初動");
        }
        else
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);

            //Debug.Log("コンボ");
        }
        /*
        if (Current_ComboNumner == dashskilldata.Max_Combo)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);

            Debug.Log("フィニッシュ");
        }
        else if (Current_ComboNumner == 1)
        {
            //DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
            //技の持続時間を与える
            Debug.Log("初動");
        }
        else
        {
            //DashAttack_Afterglow = dashskilldata._Afterglow;
            //攻撃
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
            //プレイヤーに後隙と移動速度の減少率を与える
            Debug.Log("コンボ");
            //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//後で調節可能にする
        }
        */
    }

    /*
    void OnTriggerEnter2D(Collider2D col)
    {
        /*
        var enemydam = col.gameObject.transform.root.GetComponent<EnemyBaseManager>();
        var AttackableObj = transform.root.GetComponent<AttackableObjects>();
        if (enemydam != null)
        {
            //Debug.Log("攻撃！");
            //ここで初めて、敵に攻撃が入る


            Current_ComboNumner++;

            //MAXコンボ
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                Current_ComboNumner = 0;
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;
                //攻撃
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);

                //プレイヤーに後隙と移動速度の減少率を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata.Final_Afterglow, dashskilldata.Final_Deceleration_rate);
                Debug.Log("フィニッシュ");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0f, PlayerBaseManager.Instance.rbody.velocity.y * 0f);//後で調節可能にする
            }
            //初動コンボ
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
                //技の持続時間を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                Debug.Log("初動");
            }
            else
            {
                DashAttack_Afterglow = dashskilldata._Afterglow;
                //攻撃
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //プレイヤーに後隙と移動速度の減少率を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Deceleration_rate);
                Debug.Log("コンボ");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//後で調節可能にする
            }
        }
        
    }


    //ダメージを受ける敵は自らここを参照してダメージを受ける
    public void ToEnemyDamage(GameObject obj, Rigidbody2D E_rbody)
    {
        //obj.OnDamaged(Damage, stoptime, NockBack_Power, NockBack_Angle);


        var enemydam = obj.GetComponent<EnemyBaseManager>();

        var AttackableObj = transform.root.GetComponent<AttackableObjects>();
        if(enemydam != null)
        {
            //MAXコンボ
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                Current_ComboNumner = 0;
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;
                //攻撃
                //enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
                //プレイヤーに後隙と移動速度の減少率を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata.Final_Afterglow, dashskilldata.Final_Deceleration_rate);
                Debug.Log("フィニッシュ");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0f, PlayerBaseManager.Instance.rbody.velocity.y * 0f);//後で調節可能にする
            }
            //初動コンボ
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;


                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //技の持続時間を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                Debug.Log("初動");
            }
            else
            {
                DashAttack_Afterglow = dashskilldata._Afterglow;
                //攻撃
                //enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //プレイヤーに後隙と移動速度の減少率を与える
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Deceleration_rate);
                Debug.Log("コンボ");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//後で調節可能にする
            }
        }
    }
    */
}
