using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;

    public GameObject mainMenu;
    public GameObject continueButton;

    private PlayerData playerData;
    [SerializeField] public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);

        if(mainMenu.activeInHierarchy && File.Exists(SaveSystem.filePath))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }

        //player = PlayerController.Instance;
    }

    public void CallFadeAndStartGame(string sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(sceneToLoad));
    }

    public void CallFadeAndContinueGame()
    {
        playerData = SaveSystem.LoadData();

        StartCoroutine(FadeAndContinueGame(playerData));
    }


    IEnumerator FadeAndStartGame(string sceneToLoad)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator FadeAndContinueGame(PlayerData playerData)
    {
        //Fade before doing character shenanigans
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);

        //Make the Player object active so we can modify it
        player.gameObject.SetActive(true);
        //Move the Player to the saved location
        player.transform.position = new Vector3(playerData.currentLocation.x, playerData.currentLocation.y, playerData.currentLocation.z);
        //Loading stats
        player.Health = playerData.health;
        player.Mana = playerData.mana;
        Debug.Log("Player health " + player.Health);
        Debug.Log("Player mana " + player.Mana);

        player.pState.canJump = playerData.canJump;
        player.pState.canDoubleJump = playerData.canDoubleJump;
        player.pState.canDash = playerData.canDash;
        player.pState.canHeal = playerData.canHeal;

        //Now load the saved scene
        SceneManager.LoadScene(playerData.currentScene);
    }
}
