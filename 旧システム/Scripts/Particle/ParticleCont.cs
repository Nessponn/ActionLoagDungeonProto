using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParticleCont : MonoBehaviour
{
    private float ft = 0;
    private bool OnCol = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ft+= 0.1f;
        this.gameObject.transform.localScale = new Vector3(5*Mathf.Sin(ft), 5*Mathf.Sin(ft), 1);

        GetComponent<CircleCollider2D>().radius += ft;

        //if(GetComponent<CircleCollider2D>().radius => 
        if (GetComponent<CircleCollider2D>().radius > 0.5f)
        {
            GetComponent<CircleCollider2D>().radius = 0.5f; 
        }
        //if (this.gameObject.transform.localScale.x > 3.0f && this.gameObject.transform.localScale.x < 4.7f) GetComponent<CircleCollider2D>().enabled = false;
        if (this.gameObject.transform.localScale.x >= 4.9f && OnCol)
        {
            OnCol = false;
            GetComponent<CircleCollider2D>().enabled = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        var damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null) { damagable.TakeDamage(1); Debug.Log("敵が何かにあたった"); }

        var tilemap = other.gameObject.GetComponent<Tilemap>();
        Vector3 hitPosition = Vector3.zero;
        if (tilemap != null)//"BreakableTile"なんてタグの名前付けなくてもここと後のコードで十分見分けられる
        {
            foreach (ContactPoint2D hit in other.contacts)
            {
                //Debug.Log(hit.point);
                hitPosition.x = hit.point.x - 0.1f;
                hitPosition.y = hit.point.y - 0.1f;

                var tilepos = other.gameObject.GetComponent<TileDamagable>();
                if (tilepos != null) tilepos.TakeDamage(tilemap.WorldToCell(hitPosition),1);
                //tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);


                //var position = new Vector3Int(0, 0, 0);
                //print(tilemap.GetTile(position));
                //Destroy(tilemap.GetInstantiatedObject(hitPosition));
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("敵が何かにあたった");
        var damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null) { damagable.TakeDamage(1); Debug.Log("敵が何かにあたった"); }


    }
}
