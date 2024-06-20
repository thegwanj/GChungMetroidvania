using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.tag == "Player")
        {
            //Only pick up if not at max health
            if(PlayerController.Instance.Health < PlayerController.Instance.maxHealth)
            {
                PlayerController.Instance.HealthPickup();

                if(pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
