using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherits Enemy functions and variables
public class Zombie : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = 12f;
    }

    protected override void Awake()
    {
        //Calls the Awake function from its base class
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Calls the Update function from its base class
        base.Update();

        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
