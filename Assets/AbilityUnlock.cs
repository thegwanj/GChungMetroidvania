using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlock : MonoBehaviour
{
    public bool unlockJump, unlockDoubleJump, unlockDash;
    bool used;

    private void Start()
    {
        if(unlockJump && PlayerController.Instance.pState.canJump)
        {
            Destroy(gameObject);
        }
        
        if(unlockDoubleJump && PlayerController.Instance.pState.canDoubleJump)
        {
            Destroy(gameObject);
        }
        
        if(unlockDash && PlayerController.Instance.pState.canDash)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !used)
        {
            if (unlockJump)
            {
                PlayerController.Instance.pState.canJump = true;
            }
            
            if (unlockDoubleJump)
            {
                PlayerController.Instance.pState.canDoubleJump = true;
            }
            
            if (unlockDash)
            {
                PlayerController.Instance.pState.canDash = true;
            }
            
            Destroy(gameObject);
        }
    }
}
