using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerBaseManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    private void OnCollisionEnter2D(Collision2D col)
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {

    }
}
