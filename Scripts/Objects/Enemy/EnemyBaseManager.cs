using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseManager : MonoBehaviour, ObjectsBaseManager
{


    protected int HP = 100;

    [SerializeField] protected float WalkSpeed;
    [SerializeField] protected float DashSpeed;
    [SerializeField] protected float DodgeSpeed;

    [SerializeField] GameObject Master_Collider;//着地判定（本体判定）:Collider
    [SerializeField] GameObject Damage_Collider;//当たり判定          :Trigger
    [SerializeField] GameObject Attack_Collider;//攻撃判定            :Trigger

    protected bool DAMAGE;
    protected float Damage_Time;

    protected bool YARARE;

    //コンポーネント
    protected Rigidbody2D rbody;
    protected SpriteRenderer Sr;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        Sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Damage_Time > 0)
        {
            Damage_Time -= Time.deltaTime;
        }
    }


    public virtual void OnDamaged(int Damage, float stoptime,float NockBack_Power, float NockBack_Angle)
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
