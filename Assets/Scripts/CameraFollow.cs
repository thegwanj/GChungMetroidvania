using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    //Camera will "follow" character at this speed
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] public BoxCollider2D boundsBox;

    private PlayerController player;

    private float halfHeight, halfWidth;

    //Offset the camera otherwise the camera will be on top of the player. Player will be out of camera's FOV in that case
    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        if(SceneManager.GetActiveScene().name == "Forest_1" || SceneManager.GetActiveScene().name == "Forest_2")
        {
            AudioManager.instance.PlayLevelMusic();
        }
        else if(SceneManager.GetActiveScene().name == "Cave_1")
        {
            AudioManager.instance.PlayUndergroundLevelMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(boundsBox == null)
        {
            //Positions camera by changing camera Position to go towards PlayerController Instance position at the followSpeed
            transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
        }
        else
        {
            if(player != null)
            {
                //Positions camera so that it can only move within the boundaries we set for it
                transform.position = new Vector3(
                    Mathf.Clamp(player.transform.position.x, boundsBox.bounds.min.x + halfWidth, boundsBox.bounds.max.x - halfWidth),
                    Mathf.Clamp(player.transform.position.y, boundsBox.bounds.min.y + halfHeight, boundsBox.bounds.max.y - halfHeight),
                    transform.position.z);
            }
            else
            {
                //Useful for if we are moving between scenes that have a Player object in both of them
                Debug.Log("No Player found. Searching for Player");
                player = FindObjectOfType<PlayerController>();
            }
        }
    }
}
