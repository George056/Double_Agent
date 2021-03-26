using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public GameObject SettingsWindow;

    public Slider MusicSlider;
    public Slider SoundEffectSlider;

    public AudioSource Music1;
    public AudioSource Music2;
    public AudioSource SFX1;
    public AudioSource SFX2;
    public AudioSource SFX3;
    public AudioSource SFX4;

    void Awake()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        //SoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
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

    public void ChangeSoundEffectVolume()
    {
        SFX1.volume = SoundEffectSlider.value;
        SFX2.volume = SoundEffectSlider.value;
        SFX3.volume = SoundEffectSlider.value;
        SFX4.volume = SoundEffectSlider.value;
        PlayerPrefs.SetFloat("SoundEffectsVolume", SoundEffectSlider.value);
    }
}