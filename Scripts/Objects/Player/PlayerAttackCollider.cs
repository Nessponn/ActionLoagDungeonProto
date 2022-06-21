using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    //�����ɃX�L���f�[�^�n�������ŁA�U�������藧�悤�ɂ���

    private GameObject AttackColider;//�p����ƒl�����L(static��)
    private PlayerDashAttackCollider PDA;
    protected SkillData dashskilldata;//�p����ƒl�����L(static��)

    //protected static float DashAttack_Afterglow;//�p����ƒl�����L(static��)


    //protected static float DashCombo_LifeTime;//�p����ƒl�����L(static��)
    protected int Current_ComboNumner;//�p����ƒl�����L(static��)


    private float _AttackingTime;          //�ꔭ�̃X�L���S�̂̎��� �@ �E
    private float Attack_Frontglow;        //�O���@�@�@�@�@�@�@�@�@�@�@��
    private float Attack_Collider_LifeTime;//�U�����ԁi�U���L�����ԁj�@��
    private float Attack_Afterglow;        //�㌄�@�@�@�@�@�@�@�@�@�@�@��
    private float ContinueTime;            //�㌄��̃R���{���̕ێ�����


    private AttackableObjects AttackableObj ;

    //�㌄���Ԍ����莞�ԓG�̎�t�����݂��Ȃ��@���@�i���v�j�R���{���Łi�����ō��v�R���{�ɂ��Ă͂��܂�l���Ȃ��Ă����j


    #region //�R���|�[�l���gresion
    private SpriteRenderer Sr;
    private Color _availableColor;
    private Color _UnavailableColor;

    private BoxCollider2D Collider;

    private SoundManager SM;
    private AudioSource SE_AS;
    #endregion

    //private List<GameObject> objs = new List<GameObject>();

    //protected float Skill_LifeTime;//�X�L���̗L�����ԁB���̎��Ԃ���������̓R���{���r�؂�Ȃ�

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
        if(_AttackingTime > Attack_Collider_LifeTime + Attack_Afterglow)//�O��
        {
            //�G�̎�t�𒆎~
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //�A�^�b�N�R���C�_�[������
            //AttackColider.SetActive(false);

            _AttackingTime -= Time.deltaTime;
            //Debug.Log("�O��");
        }
        else if(_AttackingTime > Attack_Afterglow)//�U����
        {
            //�G�̎�t�𒆎~
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //�A�^�b�N�R���C�_�[���o��
             AttackColider.SetActive(true);

            _AttackingTime -= Time.deltaTime;
           // Debug.Log("�U����");
        }
        else if (_AttackingTime >= 0)//�㌄
        {
            //�G�̎�t�𒆎~
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //�A�^�b�N�R���C�_�[������
            AttackColider.SetActive(false);

            _AttackingTime -= Time.deltaTime;
            //Debug.Log("�㌄");

        }
        else if(_AttackingTime <= 0&& ContinueTime > 0)//�Ď�t��(�R���{���̃R���{�ێ���)
        {
            //�G�̎�t���ĊJ
            Sr.color = _availableColor;
            Collider.enabled = true;

            //�A�^�b�N�R���C�_�[������
            //AttackColider.SetActive(false);

            ContinueTime -= Time.deltaTime;
            //Debug.Log("�R���{�ێ���");
        }
        else
        {
            //�G�̎�t���ĊJ
            Sr.color = _availableColor;
            Collider.enabled = true;

            //�A�^�b�N�R���C�_�[������
            AttackColider.SetActive(false);

            //�R���{���Z�b�g
            Current_ComboNumner = 0;
            PDA.Combo_Reset();
            //Debug.Log("�R���{����");
        }
        /*
        //�ꔭ�̃X�L���S�̂̎��� = (�R���C�_�[�̐������ԁ@�{�@�㌄)
        if (_AttackingTime > Attack_Afterglow)
        {
            //�G�̎�t�𒆎~
            Sr.color = _UnavailableColor;
            Collider.enabled = false;

            //�A�^�b�N�R���C�_�[���o��
            AttackColider.SetActive(true);
            _AttackingTime -= Time.deltaTime;
        }
        //�R���C�_�[���Ō�A�㌄���Ԓ��Ɏ�t�R���C�_�[�ɓG��������@���@�R���{
        else if (_AttackingTime > 0)
        {
            //�G�̎�t���ĊJ
            Sr.color = _availableColor;
            Collider.enabled = true;

            AttackColider.SetActive(false);
            _AttackingTime -= Time.deltaTime;
        }
        //�㌄���Ԍ���G�����Ȃ��@���@�R���{����
        else
        {
            Current_ComboNumner = 0;
        }
        */

        /*
        {
            //�U���̔��肪�o�Ă���Ԃ́A�_�b�V���U���͐V�����ł��Ȃ��悤�ɂ���
            //�܂�A�_�b�V���A�^�b�N�̓G��t�R���C�_�[�ƍU���R���C�_�[�͋������Ȃ�

            //�G�̎�t�R���C�_�[�́A��Ƀv���C���[�̎q�I�u�W�F�N�g�ɂ����Ă����Asetactive��true�̂܂܂ɂ���
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
        

        //�R���{�̗L���������Ԃ��؂ꂽ��A�R���{�̒l���O�ɖ߂�
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
                Attack_Frontglow = dashskilldata.Final_Frontglow;//�ŏI�i�O��
                Attack_Collider_LifeTime = dashskilldata.Final_Collider_LifeTime;//�ŏI�i�R���C�_�[�^�C��
                Attack_Afterglow = dashskilldata.Final_Afterglow;//�ŏI�i�㌄

                ContinueTime = 0;//�ŏI�i�R���{��������

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//�X�L���S�̂̒ʏ탂�[�V��������


                //Current_ComboNumner = 0;
                Debug.Log("P:�t�B�j�b�V��");

                //�㌄�A�v���C���[�ɓK�p

                //�X�L���̈ꔭ�S�̂̍d���A������
                AttackableObj.DashAttackHit(_AttackingTime, dashskilldata.Final_Deceleration_rate);

                //���ʉ���炷
                SM.PlaySE(SEID.SLASH_Burst, SE_AS, false);

            }
            else if (Current_ComboNumner == 1)
            {
                //�p�����[�^�[�ݒ�

                Attack_Frontglow = dashskilldata.Normal_Frontglow;//�ʏ�O��
                Attack_Collider_LifeTime = dashskilldata.Normal_Collider_LifeTime;//�ʏ�R���C�_�[�^�C��
                Attack_Afterglow = dashskilldata.Normal_Afterglow;//�ʏ�㌄

                ContinueTime = dashskilldata.Combo_ContinueTime;//�ʏ�R���{��������

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//�X�L���S�̂̒ʏ탂�[�V��������

                Debug.Log("P:����");

                //Debug.Log("�O���@�n���� ="+ _AttackingTime);
                //Debug.Log("�U�����@�n���� =" + (Attack_Collider_LifeTime + Attack_Afterglow));
                //Debug.Log("�㌄�@�n���� =" + Attack_Afterglow);
                //�㌄�A�v���C���[�ɓK�p

                //�X�L���̈ꔭ�S�̂̍d���A������
                AttackableObj.DashAttackHit(_AttackingTime , dashskilldata.Deceleration_rate);
                //���ʉ���炷
                SM.PlaySE(SEID.SLASH_Hit, SE_AS, false);
            }
            else
            {
                //�p�����[�^�[�ݒ�

                Attack_Frontglow = dashskilldata.Normal_Frontglow;//�ʏ�O��
                Attack_Collider_LifeTime = dashskilldata.Normal_Collider_LifeTime;//�ʏ�R���C�_�[�^�C��
                Attack_Afterglow = dashskilldata.Normal_Afterglow;//�ʏ�㌄

                ContinueTime = dashskilldata.Combo_ContinueTime;//�ʏ�R���{��������

                _AttackingTime = Attack_Frontglow + Attack_Collider_LifeTime + Attack_Afterglow;//�X�L���S�̂̒ʏ탂�[�V��������

                Debug.Log("P:�R���{");

                //�㌄�A�v���C���[�ɓK�p

                //�X�L���̈ꔭ�S�̂̍d���A������
                AttackableObj.DashAttackHit(_AttackingTime , dashskilldata.Deceleration_rate);
                //���ʉ���炷
                SM.PlaySE(SEID.SLASH_Hit, SE_AS, false);
            }


        }
        
        /*
        var AttackableObj = transform.root.GetComponent<AttackableObjects>();
        //var enemydam = col.gameObject.transform.root.GetComponent<ObjectsBaseManager>();
        //���������v���C���[�ȊO�̉󂹂���̂ɋߊ��΁A�U���͔�������悤�ɂ���
        if (col.gameObject.tag != "Player" && enemydam != null)
        {
            //�����ŏ��߂āA�A�^�b�N�R���C�_�[�𔭐�������
            AttackColider.SetActive(true);
            Current_ComboNumner++;
            //Debug.Log(" Current_ComboNumner =" + Current_ComboNumner);
            //���݂̃R���{���̑���
            //Current_ComboNumner++;

            //���݂̃R���{���ɉ����Č㌄�̔���
            if (Current_ComboNumner == dashskilldata.Max_Combo)
            {
                DashAttack_Afterglow = dashskilldata.Final_Afterglow;//�ŏI�i

                Current_ComboNumner = 0;
            }
            else if (Current_ComboNumner == 1)
            {
                DashSkill_LifeTime = dashskilldata.Skill_LifeTime;
                if (AttackableObj != null) AttackableObj.DashAttackHit(dashskilldata._Afterglow, dashskilldata.Skill_LifeTime, dashskilldata.Deceleration_rate);
                DashAttack_Afterglow = dashskilldata._Afterglow;//�ʏ�

                //objs.Add(col.gameObject);
                //Debug.Log(" objs.Count =" + objs.Count);
            }
            else
            {
                //Current_ComboNumner++;
                DashAttack_Afterglow = dashskilldata._Afterglow;//�ʏ�
            }

        }
        */
    }
}
