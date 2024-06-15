using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherits Enemy functions and variables
public class Zombie : Enemy
{
    [Header("Patrol Settings")]
    [SerializeField] protected Transform[] patrolPoints;
    private int currentPoint;
    [SerializeField] protected float waitAtPoints;
    private float waitCounter;

    protected override void Start()
    {
        //Calls the Start function from its base class
        base.Start();
        rb.gravityScale = 12f;
        waitCounter = waitAtPoints;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Calls the Update function from its base class
        base.Update();

        /*
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
        */
        if (!isRecoiling)
        {
            //Code for patrolling
            if (Mathf.Abs(transform.position.x - patrolPoints[currentPoint].position.x) > 0.2f)
            {
                if (transform.position.x < patrolPoints[currentPoint].position.x)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                    transform.localScale = Vector3.one;
                }

                if (transform.position.y < patrolPoints[currentPoint].position.y - 0.5f && rb.velocity.y < 0.1f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);

                waitCounter -= Time.deltaTime;

                if (waitCounter <= 0)
                {
                    waitCounter = waitAtPoints;

                    currentPoint++;

                    if (currentPoint >= patrolPoints.Length)
                    {
                        currentPoint = 0;
                    }
                }
            }
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
