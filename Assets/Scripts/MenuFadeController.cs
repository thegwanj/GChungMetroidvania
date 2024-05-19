using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }

    public void CallFadeAndStartGame(string sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(sceneToLoad));
    }

    IEnumerator FadeAndStartGame(string sceneToLoad)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
