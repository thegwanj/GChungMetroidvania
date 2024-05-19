using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public static GameManager Instance { get; private set; }

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Vector2 defaultRespawnPoint;
    [SerializeField] Bench bench;

    [Header("Pause Settings")]
    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void RespawnPlayer()
    {
        if(bench != null)
        {
            if (bench.interacted)
            {
                respawnPoint = bench.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}
