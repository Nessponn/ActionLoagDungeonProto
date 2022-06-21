using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCont : MonoBehaviour
{

    private Rigidbody2D rbody;
    private float t;

    private AudioSource AS;
    public AudioClip AC;//爆発音
    public GameObject Particle;

    private bool alreadyBomb = true;
    // Start is called before the first frame update
    void Start()
    {
        AS = Camera.main.GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(rbody.velocity.x <= 3)
        {
            tx++;
            rbody.velocity = new Vector3(rbody.velocity.x - (tx * tx), rbody.velocity.y);
        }
        */

        if(rbody.velocity.y <= 0)
        {
            if(rbody.velocity.y >= -10) t++;
            rbody.velocity = new Vector2(rbody.velocity.x,rbody.velocity.y - t);
        }

        if(transform.position.y <= -25)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag != "Player" && col.gameObject.tag != "Bomb")
        {
            AS.clip = AC;
            AS.Play();
            if (alreadyBomb)
            {
                Destroy(Instantiate(Particle, this.transform.position, Quaternion.identity), 0.5f);
                alreadyBomb = false;
            }
            Destroy(this.gameObject);
        }
    }
}
