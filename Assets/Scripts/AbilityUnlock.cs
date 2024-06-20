using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityUnlock : MonoBehaviour
{
    public bool unlockJump, unlockDoubleJump, unlockDash, unlockHeal;
    bool used;

    public string unlockMessage;
    public TMP_Text unlockText;

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

        if (unlockHeal && PlayerController.Instance.pState.canHeal)
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

            if (unlockHeal)
            {
                PlayerController.Instance.pState.canHeal = true;
            }

            unlockText.transform.parent.SetParent(null);
            unlockText.transform.parent.position = transform.position;

            unlockText.text = unlockMessage;
            unlockText.gameObject.SetActive(true);

            Destroy(unlockText.transform.parent.gameObject, 5f);
            Destroy(gameObject);
        }
    }
}
