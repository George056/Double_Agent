using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public GameObject SettingsWindow;

    public Slider MusicSlider;

    public AudioSource Music1;
    public AudioSource Music2;

    void Awake()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

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
        Music1.volume = MusicSlider.value;
        Music2.volume = MusicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }
}