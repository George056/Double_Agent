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

    public void ChangeVolume()
    {
        Music.volume = MusicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }
}