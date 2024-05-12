using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public SceneFader sceneFader;

    [SerializeField] GameObject deathScreen;

    private void Awake()
    {
        //If Instance variable has already been set AND this UIManager is not that Instance, destroy the gameObject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        //Otherwise, make this gameObject the UIManager instance
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSeconds(0.8f);
        deathScreen.SetActive(true);
    }

    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
}
