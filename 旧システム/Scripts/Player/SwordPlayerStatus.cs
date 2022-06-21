using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerStatus : MonoBehaviour
{
    public GameObject[] Life;
    private int Life_Num = 2;
    private bool Damage = true;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Damage)
        {
            gameObject.GetComponent<SpriteRenderer>().color =
                            new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Sin(Time.time * 10)));//無敵が発動すると点滅を開始);

        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, 1f);//無敵が発動すると点滅を開始
        }
    }

    public void LifeDecrease()
    {
        Life[Life_Num].SetActive(false);
        Life_Num--;

        if(Life_Num < 0)
        {
            var GM = GameObject.FindWithTag("GameMaster");

            if(GM != null)
            {
                //ゲームオーバー入力
                GM.GetComponent<GameMaster>().GameOver();
            }
        }
    }

    public void TakeDamage()
    {
        if (Damage)
        {
            Damage = false;
            LifeDecrease();
            StartCoroutine(DamageSpan(3));
        }
    }

    public IEnumerator DamageSpan(float Time)
    {
        yield return new WaitForSeconds(Time);
        Damage = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
    }

    public bool SpinStateGetter()//スピン中であるかを検査する
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
}
