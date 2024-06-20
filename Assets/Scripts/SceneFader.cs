using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] public float fadeTime;

    private Image fadeOutUIImage;

    public enum FadeDirection
    {
        In,
        Out
    }

    // Start is called before the first frame update
    void Awake()
    {
        fadeOutUIImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallFadeAndLoadScene(string sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, sceneToLoad));
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;

        if(_fadeDirection == FadeDirection.Out)
        {
            while(_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }

            fadeOutUIImage.enabled = false;
        }
        else
        {
            fadeOutUIImage.enabled = true;

            while(_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeOutUIImage.enabled = true;

        yield return Fade(_fadeDirection);

        //Save our progress in between scenes
        PlayerData playerData = new PlayerData
        {
            health = PlayerController.Instance.Health,
            mana = PlayerController.Instance.Mana,
            currentLocation = PlayerController.Instance.transform.position,
            currentScene = SceneManager.GetActiveScene().name,

            canJump = PlayerController.Instance.pState.canJump,
            canDoubleJump = PlayerController.Instance.pState.canDoubleJump,
            canDash = PlayerController.Instance.pState.canDash,
            canHeal = PlayerController.Instance.pState.canHeal
        };

        SaveSystem.SaveData(playerData);

        SceneManager.LoadScene(_sceneToLoad);
    }

    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);

        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
