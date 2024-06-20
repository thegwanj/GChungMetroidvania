using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoader : MonoBehaviour
{
    public AudioManager theAM;

    //For when we start the game in a scene that is not the main menu
    void Awake()
    {
        if(AudioManager.instance == null)
        {
            AudioManager newAM = Instantiate(theAM);
            AudioManager.instance = newAM;
            DontDestroyOnLoad(newAM.gameObject);
        }
    }
}
