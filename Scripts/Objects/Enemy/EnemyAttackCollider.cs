using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : EnemyBaseManager
{
    
    void OnTriggerEnter2D(Collider2D col)
    {
        var enemydam = col.gameObject.GetComponent<ObjectsBaseManager>();
        if(col.gameObject.tag == "Player" && enemydam != null)
        {
            enemydam.OnDamaged(1,0.5f,0,0);
        }
    }
}
