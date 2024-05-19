using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour
{
    public bool interacted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collision)
    {
        if(collision.CompareTag("Player") && Input.GetButtonDown("Interact"))
        {
            interacted = true;
            Debug.Log("Interacted!");
        }
    }
}
