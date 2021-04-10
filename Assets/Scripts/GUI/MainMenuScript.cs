using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<SettingsScript>().MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        //GameObject.FindObjectOfType<SettingsScript>().SoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayHelp()
    {

    }

    public void DisplayAbout()
    {

    }
}
