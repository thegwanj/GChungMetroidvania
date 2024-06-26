using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base enemy script. Other enemy types will go off of this one
public class Enemy : MonoBehaviour
{
    [Header ("Main Settings")]
    //Making variables protected so that subclasses can use it
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if(health <= 0)
        {
            Destroy(gameObject);
        }

        if (isRecoiling)
        {
            if(recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        //if enemy is NOT recoiling, it will now recoil in the direction the hit comes from
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }


    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

}
