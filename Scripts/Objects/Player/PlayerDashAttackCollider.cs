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
        
        //�R���C�_�[�̎������؂ꂽ��\��������

        //�����ɕ\�������������������Ă���̂́A��ѓ���ł�PlayerAttakCollider�Ɛ؂藣���čl������悤�ɂ��邽��

        //��������폜
        /*
        Collider_LifeTime -= Time.deltaTime;
        if (Collider_LifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
        */
    }

    //�o�O���Ă��������́APlayerAttackCollider�ɂ����Ă���OnTriggerEnter2D��
    //�p����̂����ɔ������Ă��܂��Ă�������B

    public void ToEnemyDamage(GameObject obj)
    {
        Current_ComboNumner++;
        Debug.Log("Current_ComboNumner = " + Current_ComboNumner);
        if (Current_ComboNumner == dashskilldata.Max_Combo)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
            //Current_ComboNumner = 0;
            //Debug.Log("�t�B�j�b�V��");
        }
        else if (Current_ComboNumner == 1)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);

            //Debug.Log("����");
        }
        else
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);

            //Debug.Log("�R���{");
        }
        /*
        if (Current_ComboNumner == dashskilldata.Max_Combo)
        {
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);

            Debug.Log("�t�B�j�b�V��");
        }
        else if (Current_ComboNumner == 1)
        {
            //DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
            //�Z�̎������Ԃ�^����
            Debug.Log("����");
        }
        else
        {
            //DashAttack_Afterglow = dashskilldata._Afterglow;
            //�U��
            obj.GetComponent<EnemyBaseManager>().OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
            //�v���C���[�Ɍ㌄�ƈړ����x�̌�������^����
            Debug.Log("�R���{");
            //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//��Œ��߉\�ɂ���
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
            //Debug.Log("�U���I");
            //�����ŏ��߂āA�G�ɍU��������


            Current_ComboNumner++;

            //MAX�R���{
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                Current_ComboNumner = 0;
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;
                //�U��
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);

                //�v���C���[�Ɍ㌄�ƈړ����x�̌�������^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata.Final_Afterglow, dashskilldata.Final_Deceleration_rate);
                Debug.Log("�t�B�j�b�V��");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0f, PlayerBaseManager.Instance.rbody.velocity.y * 0f);//��Œ��߉\�ɂ���
            }
            //�����R���{
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
                //�Z�̎������Ԃ�^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                Debug.Log("����");
            }
            else
            {
                DashAttack_Afterglow = dashskilldata._Afterglow;
                //�U��
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //�v���C���[�Ɍ㌄�ƈړ����x�̌�������^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Deceleration_rate);
                Debug.Log("�R���{");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//��Œ��߉\�ɂ���
            }
        }
        
    }


    //�_���[�W���󂯂�G�͎��炱�����Q�Ƃ��ă_���[�W���󂯂�
    public void ToEnemyDamage(GameObject obj, Rigidbody2D E_rbody)
    {
        //obj.OnDamaged(Damage, stoptime, NockBack_Power, NockBack_Angle);


        var enemydam = obj.GetComponent<EnemyBaseManager>();

        var AttackableObj = transform.root.GetComponent<AttackableObjects>();
        if(enemydam != null)
        {
            //MAX�R���{
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                Current_ComboNumner = 0;
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;
                //�U��
                //enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.Final_NockBack_Power, dashskilldata.NockBack_Angle);
                //�v���C���[�Ɍ㌄�ƈړ����x�̌�������^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata.Final_Afterglow, dashskilldata.Final_Deceleration_rate);
                Debug.Log("�t�B�j�b�V��");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0f, PlayerBaseManager.Instance.rbody.velocity.y * 0f);//��Œ��߉\�ɂ���
            }
            //�����R���{
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;


                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //�Z�̎������Ԃ�^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                Debug.Log("����");
            }
            else
            {
                DashAttack_Afterglow = dashskilldata._Afterglow;
                //�U��
                //enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                enemydam.OnDamaged(dashskilldata.Power, dashskilldata.HitStun_Time, dashskilldata.NockBack_Power, dashskilldata.NockBack_Angle);
                //�v���C���[�Ɍ㌄�ƈړ����x�̌�������^����
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Deceleration_rate);
                Debug.Log("�R���{");
                //PlayerBaseManager.Instance.rbody.velocity = new Vector2(PlayerBaseManager.Instance.rbody.velocity.x * 0.2f, PlayerBaseManager.Instance.rbody.velocity.y * 0.2f);//��Œ��߉\�ɂ���
            }
        }
    }
    */
}
