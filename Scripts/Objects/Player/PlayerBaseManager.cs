using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extentions.UnityEngine.Vector;
using UnityEngine.UI;

public abstract class PlayerBaseManager : SingletonMonoBehaviourFast<PlayerBaseManager>,ObjectsBaseManager, AttackableObjects
{
    public GameObject text;
    public Text _Aftergrow_text;

    protected int HP = 100;

    [SerializeField] protected float WalkSpeed;
    [SerializeField] protected float DashSpeed;
    [SerializeField] protected float DodgeSpeed;

    [SerializeField] protected float Dodge_Span;//回避後のスパン(未調整可のままにしているので、後で直す)

    [SerializeField] GameObject Master_Collider;//着地判定（本体判定）:Collider
    [SerializeField] GameObject Damage_Collider;//当たり判定          :Trigger
    [SerializeField] GameObject EnemyReception_Collider;//敵受付判定  :Trigger

    protected bool DAMAGE;
    protected bool YARARE;


    //コンポーネント
    [System.NonSerialized]public Rigidbody2D rbody;
    protected SpriteRenderer Sr;

    /// <summary>
    /// 格攻撃のオブジェクトには、スキルスクリプトとスキルコライダーが存在する。
    /// そこに各スキルデータをぶち込む
    /// </summary>

    [SerializeField] public GameObject DashAttack_Collider;//ダッシュ攻撃判定枠:Trigger

    [SerializeField] public GameObject Skill1_Collider;//スキル攻撃判定枠１:Trigger

    [SerializeField] public GameObject Skill2_Collider;//スキル攻撃判定枠２:Trigger

    [SerializeField] public GameObject Skill3_Collider;//スキル攻撃判定枠３:Trigger

    public SkillData DashAttack;
    public SkillData Skill1;
    public SkillData Skill2;
    public SkillData Skill3;


    private float Dash_Afterglow;
    private float DashSkill_LifeTime;
    private float Deceleration_rate;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        Sr = GetComponent<SpriteRenderer>();

        Vector3 vec = Extention.NormalizedEx(new Vector3(1,1,1));

        //Time.timeScale = 0.3f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _Aftergrow_text.text = "" + Dash_Afterglow;

        if (Dodge_Span > 0)
        {
            Dodge_Span -= Time.deltaTime;
            rbody.velocity = new Vector2(rbody.velocity.x * 0.995f, rbody.velocity.y);
            Debug.Log("回避無効");
            return;
        }


        if(Dash_Afterglow > 0)
        {
            Dash_Afterglow -= Time.deltaTime;
            rbody.velocity = new Vector2(rbody.velocity.x * Deceleration_rate, rbody.velocity.y);
            text.SetActive(false);
        }
        else
        {
            text.SetActive(true);
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rbody.velocity = new Vector2(DashSpeed, rbody.velocity.y);
                //EnemyReception_Collider.SetActive(true);
            }
            else
            {
                rbody.velocity = new Vector2(rbody.velocity.x * 0.995f, rbody.velocity.y);
                //EnemyReception_Collider.SetActive(false);
            }
        }

        if (DashSkill_LifeTime >= 0)
        {
            DashSkill_LifeTime -= Time.deltaTime;
            return;
        }

        //回避が現れたら回避優先の操作性にする
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rbody.velocity = new Vector2(-DodgeSpeed, rbody.velocity.y);
            Dodge_Span = 0.5f;
        }
       

        
    }


    //コンボ初動
    public void DashAttackHit(float _Afterglow, float Skill_LifeTime, float Deceleration_rate)
    {
        Dash_Afterglow = _Afterglow;
        DashSkill_LifeTime = Skill_LifeTime;
        this.Deceleration_rate = Deceleration_rate;
    }
    
    //コンボ中
    public void DashAttackHit(float _Afterglow, float Deceleration_rate)
    {
        Debug.Log("ヒット");
        Dash_Afterglow = _Afterglow;
        this.Deceleration_rate = Deceleration_rate;
    }

    public void SkillShot(float _Afterglow, float Skill_LifeTime, float Deceleration_rate)
    {

    }

    public virtual void Walk()
    {

    }

    public virtual void _DashAttack(bool dash)
    {
        //ダッシュアタックの現在のコンボ数をカウント
        //現在のコンボ数に応じて、ノックバックの威力などを


    }

    public virtual void _SkillAttack()
    {

    }

    public virtual void _MagicAttack()
    {

    }

    public virtual void OnDamaged(int Damage, float stoptime, float NockBack_Power, float NockBack_Angle)
    {

    }

    public virtual void NockBack(float NockBack_Power, float NockBack_Angle)
    {

    }

    private IEnumerator DAMAGE_SPAN(float stoptime)
    {
        yield return null;
    }

    //持続ダメージ、ダメージの単発火力が高い。氷系の敵に有効
    public virtual void FIRE()
    {

    }
    //発動した場合、敵を一定時間動けなくする
    public virtual void FREEZE()
    {

    }
    //ショック中、一定確率で敵を一定時間動けなくする
    public virtual void SHOCK()
    {

    }
    //持続ダメージ、発動中は攻撃力が半減する。
    public virtual void POIZON()
    {

    }
    //持続する連続ダメージ。他の持続ダメージ系より連打力が高く、装備によっては最も火力が出る
    public virtual void WIND()
    {

    }
}
