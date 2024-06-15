using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource mainMenuMusic, levelMusic, undergroundLevelMusic, bossMusic;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMainMenuMusic()
    {
        levelMusic.Stop();
        undergroundLevelMusic.Stop();
        bossMusic.Stop();

        mainMenuMusic.Play();
    }

    public void PlayLevelMusic()
    {
        if (!levelMusic.isPlaying)
        {
            bossMusic.Stop();
            mainMenuMusic.Stop();
            undergroundLevelMusic.Stop();

            levelMusic.Play();
        }
    }

    public void PlayUndergroundLevelMusic()
    {
        levelMusic.Stop();
        mainMenuMusic.Stop();

        undergroundLevelMusic.Play();
    }

    public void PlayBossMusic()
    {
        levelMusic.Stop();

        bossMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
