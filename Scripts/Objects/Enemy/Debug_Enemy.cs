using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Enemy : EnemyBaseManager, ObjectsBaseManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Damage_Time > 0)
        {
            Damage_Time -= 0.016f;
            return;
        }
        else
        {

        }

        //rbody.velocity = new Vector2(-DashSpeed, rbody.velocity.y);
    }

    public override void OnDamaged(int Damage, float stoptime, float NockBack_Power, float NockBack_Angle)
    {

        HP -= Damage;
        Damage_Time = stoptime;

        NockBack(NockBack_Power,NockBack_Angle);//ノックバック

        Debug.Log("ダメージを食らった");

    }

    public override void NockBack(float NockBack_Power, float NockBack_Angle)
    {
        Vector3 vec = Quaternion.Euler(0, 0, NockBack_Angle) * Vector2.up;//方向は上基準で、
        vec.Normalize();//正規化して、威力を調整


        rbody.velocity = vec * NockBack_Power;//適用
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        var Attackcol = col.gameObject.GetComponent<AttackableCollider>();
        if(col.gameObject.CompareTag("Player") && Attackcol != null)
        {
            Attackcol.ToEnemyDamage(this.gameObject) ;
        }
    }
}
