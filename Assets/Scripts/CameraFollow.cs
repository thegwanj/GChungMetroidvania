using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Camera will "follow" character at this speed
    [SerializeField] private float followSpeed = 0.1f;

    //Offset the camera otherwise the camera will be on top of the player. Player will be out of camera's FOV in that case
    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Positions camera by changing camera Position to go towards PlayerController Instance position at the followSpeed
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
    }
}
