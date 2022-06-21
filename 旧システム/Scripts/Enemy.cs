using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IDamagable//ここで継承することで、IDamagableの要素がEnemyに付与される
{
    [SerializeField] public int HP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int num)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("敵が何かにあたった");
        var damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null) damagable.TakeDamage(1);
    }
}
