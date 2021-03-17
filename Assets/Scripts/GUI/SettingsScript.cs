using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public GameObject SettingsWindow;

    public Slider MusicSlider;

    public AudioSource Music;


    public void SettingsWindowPopUp()
    {
        SettingsWindow.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        SettingsWindow.SetActive(false);
    }

    private void Update()
    {
        Music.volume = MusicSlider.value;
    }
}