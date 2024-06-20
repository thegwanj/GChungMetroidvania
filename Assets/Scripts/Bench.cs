using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bench : MonoBehaviour
{
    private bool collidingWithPlayer = false;
    public bool interacted;
    private PlayerController player;

    [SerializeField] GameObject saveEffect;
    [SerializeField] GameObject tip;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Interact") && collidingWithPlayer)
        {
            interacted = true;
            Debug.Log("Interacted!");
            SaveGame();
            Debug.Log("Game Saved!");
            GameObject saveComplete = Instantiate(saveEffect, transform.position, Quaternion.identity);
            Destroy(saveComplete, 2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            collidingWithPlayer = true;
            tip.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            collidingWithPlayer = false;
            tip.SetActive(false);
        }
    }

    private void SaveGame()
    {
        PlayerData playerData = new PlayerData
        {
            health = player.Health,
            mana = player.Mana,
            currentLocation = player.transform.position,
            currentScene = SceneManager.GetActiveScene().name,

            canJump = player.pState.canJump,
            canDoubleJump = player.pState.canDoubleJump,
            canDash = player.pState.canDash,
            canHeal = player.pState.canHeal
        };

        SaveSystem.SaveData(playerData);
    }
}
