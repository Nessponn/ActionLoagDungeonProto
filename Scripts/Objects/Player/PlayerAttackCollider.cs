using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    //こいつにスキルデータ渡すだけで、攻撃が成り立つようにする

    private GameObject AttackColider;//継承先と値を共有(static化)
    private PlayerDashAttackCollider PDA;
    protected SkillData dashskilldata;//継承先と値を共有(static化)

    //protected static float DashAttack_Afterglow;//継承先と値を共有(static化)


    //protected static float DashCombo_LifeTime;//継承先と値を共有(static化)
    protected int Current_ComboNumner;//継承先と値を共有(static化)


    private float _AttackingTime;          //一発のスキル全体の時間 　 ・
    private float Attack_Frontglow;        //前隙　　　　　　　　　　　↓
    private float Attack_Collider_LifeTime;//攻撃時間（攻撃有効時間）　↓
    private float Attack_Afterglow;        //後隙　　　　　　　　　　　↓
    private float ContinueTime;            //後隙後のコンボ数の保持時間


    private AttackableObjects AttackableObj ;

    //後隙時間後も一定時間敵の受付が存在しない　→　（合計）コンボ消滅（ここで合計コンボについてはあまり考えなくていい）


    #region //コンポーネントresion
    private SpriteRenderer Sr;
    private Color _availableColor;
    private Color _UnavailableColor;

    private BoxCollider2D Collider;

    private SoundManager SM;
    private AudioSource SE_AS;
    #endregion

    //private List<GameObject> objs = new List<GameObject>();

    //protected float Skill_LifeTime;//スキルの有効時間。この時間が続く限りはコンボが途切れない

    // Start is called before the first frame update
    void Start()
    {
        AttackColider = PlayerBaseManager.Instance.DashAttack_Collider;
        dashskilldata = PlayerBaseManager.Instance.DashAttack;

        Sr = GetComponent<SpriteRenderer>();
        _availableColor = new Color(Sr.color.r, Sr.color.g, Sr.color.b, Sr.color.a);
        _UnavailableColor = new Color(Sr.color.r, Sr.color.g, Sr.color.b, 0);

        Collider = GetComponent<BoxCollider2D>();

        AttackableObj = transform.root.GetComponent<AttackableObjects>();

        PDA = AttackColider.GetComponent<PlayerDashAttackCollider>();

        SM = SoundManager.Instance;
        SE_AS = GetComponent<AudioSource>();
    }
    void Update()
    {
        if(_AttackingTime > Attack_Collider_LifeTime + Attack_Afterglow)//前隙
        {
            //敵の受付を中止
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //アタックコライダーを消去
            //AttackColider.SetActive(false);

            _AttackingTime -= Time.deltaTime;
            //Debug.Log("前隙");
        }
        else if(_AttackingTime > Attack_Afterglow)//攻撃中
        {
            //敵の受付を中止
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //アタックコライダーを出現
             AttackColider.SetActive(true);

            _AttackingTime -= Time.deltaTime;
           // Debug.Log("攻撃中");
        }
        else if (_AttackingTime >= 0)//後隙
        {
            //敵の受付を中止
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //アタックコライダーを消去
            AttackColider.SetActive(false);

            _AttackingTime -= Time.deltaTime;
            //Debug.Log("後隙");

        }
        else if(_AttackingTime <= 0&& ContinueTime > 0)//再受付可(コンボ中のコンボ保持中)
        {
            //敵の受付を再開
            Sr.color = _availableColor;
            Collider.enabled = true;

            //アタックコライダーを消去
            //AttackColider.SetActive(false);

            ContinueTime -= Time.deltaTime;
            //Debug.Log("コンボ保持中");
        }
        else
        {
            //敵の受付を再開
            Sr.color = _availableColor;
            Collider.enabled = true;

            //アタックコライダーを消去
            AttackColider.SetActive(false);

            //コンボリセット
            Current_ComboNumner = 0;
            PDA.Combo_Reset();
            //Debug.Log("コンボ解除");
        }
        /*
        //一発のスキル全体の時間 = (コライダーの生成時間　＋　後隙)
        if (_AttackingTime > Attack_Afterglow)
        {
            //敵の受付を中止
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //アタックコライダーを出現
            AttackColider.SetActive(true);
            _AttackingTime -= Time.deltaTime;
        }
        //コライダー消滅後、後隙時間中に受付コライダーに敵が当たる　→　コンボ
        else if (_AttackingTime > 0)
        {
            //敵の受付を再開
            Sr.color = _availableColor;
            Collider.enabled = true;

            AttackColider.SetActive(false);
            _AttackingTime -= Time.deltaTime;
        }
        //後隙時間後も敵が来ない　→　コンボ消滅
        else
        {
            Current_ComboNumner = 0;
        }
        */

        /*
        {
            //攻撃の判定が出ている間は、ダッシュ攻撃は新しくできないようにする
            //つまり、ダッシュアタックの敵受付コライダーと攻撃コライダーは共存しない

            //敵の受付コライダーは、常にプレイヤーの子オブジェクトにおいておき、setactiveはtrueのままにする
            if (Attack_Collider_LifeTime > 0)
            {
                Sr.color = _UnavailableColor;
                Collider.enabled = false;

                Attack_Collider_LifeTime -= Time.deltaTime;
            }
            else
            {
                Sr.color = _availableColor;
                Collider.enabled = true;
            }
            
        }
        

        //コンボの有効持続時間が切れたら、コンボの値が０に戻る
        if (DashCombo_LifeTime < 0)
        {
            //Current_ComboNumner = 0;
        }
        else
        {
            //DashCombo_LifeTime -= Time.deltaTime;
        }
        */
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Current_ComboNumner++;
            Debug.Log("P:Current_ComboNumner = " + Current_ComboNumner);

            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                Attack_Frontglow = dashskilldata.Final_Frontglow;//最終段前隙
                Attack_Collider_LifeTime = dashskilldata.Final_Collider_LifeTime;//最終段コライダータイム
                Attack_Afterglow = dashskilldata.Final_Afterglow;//最終段後隙

                ContinueTime = 0;//最終段コンボ持続時間

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//スキル全体の通常モーション時間


                //Current_ComboNumner = 0;
                Debug.Log("P:フィニッシュ");

                //後隙、プレイヤーに適用

                //スキルの一発全体の硬直、減速率
                AttackableObj.DashAttackHit(_AttackingTime, dashskilldata.Final_Deceleration_rate);

                //効果音を鳴らす
                SM.PlaySE(SEID.SLASH_Burst, SE_AS, false);

            }
            else if (Current_ComboNumner == 1)
            {
                //パラメーター設定

                Attack_Frontglow = dashskilldata.Normal_Frontglow;//通常前隙
                Attack_Collider_LifeTime = dashskilldata.Normal_Collider_LifeTime;//通常コライダータイム
                Attack_Afterglow = dashskilldata.Normal_Afterglow;//通常後隙

                ContinueTime = dashskilldata.Combo_ContinueTime;//通常コンボ持続時間

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//スキル全体の通常モーション時間

                Debug.Log("P:初動");

                //Debug.Log("前隙　始動時 ="+ _AttackingTime);
                //Debug.Log("攻撃中　始動時 =" + (Attack_Collider_LifeTime + Attack_Afterglow));
                //Debug.Log("後隙　始動時 =" + Attack_Afterglow);
                //後隙、プレイヤーに適用

                //スキルの一発全体の硬直、減速率
                AttackableObj.DashAttackHit(_AttackingTime , dashskilldata.Deceleration_rate);
                //効果音を鳴らす
                SM.PlaySE(SEID.SLASH_Hit, SE_AS, false);
            }
            else
            {
                //パラメーター設定

                Attack_Frontglow = dashskilldata.Normal_Frontglow;//通常前隙
                Attack_Collider_LifeTime = dashskilldata.Normal_Collider_LifeTime;//通常コライダータイム
                Attack_Afterglow = dashskilldata.Normal_Afterglow;//通常後隙

                ContinueTime = dashskilldata.Combo_ContinueTime;//通常コンボ持続時間

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//スキル全体の通常モーション時間

                Debug.Log("P:コンボ");

                //後隙、プレイヤーに適用

                //スキルの一発全体の硬直、減速率
                AttackableObj.DashAttackHit(_AttackingTime , dashskilldata.Deceleration_rate);
                //効果音を鳴らす
                SM.PlaySE(SEID.SLASH_Hit, SE_AS, false);
            }


        }
        
        /*
        var AttackableObj = transform.root.GetComponent<AttackableObjects>();
        //var enemydam = col.gameObject.transform.root.GetComponent<ObjectsBaseManager>();
        //いちおうプレイヤー以外の壊せるものに近寄れば、攻撃は発生するようにする
        if (col.gameObject.tag != "Player" && enemydam != null)
        {
            //ここで初めて、アタックコライダーを発生させる
            AttackColider.SetActive(true);
            Current_ComboNumner++;
            //Debug.Log(" Current_ComboNumner =" + Current_ComboNumner);
            //現在のコンボ数の増加
            //Current_ComboNumner++;

            //現在のコンボ数に応じて後隙の発生
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;//最終段

                Current_ComboNumner = 0;
            }
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                DashAttack_Afterglow = dashskilldata._Afterglow;//通常

                //objs.Add(col.gameObject);
                //Debug.Log(" objs.Count =" + objs.Count);
            }
            else
            {
                //Current_ComboNumner++;
                DashAttack_Afterglow = dashskilldata._Afterglow;//通常
            }

        }
        */
    }
}
