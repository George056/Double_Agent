using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public GameObject SettingsWindow;

    public Slider MusicSlider;
    public Slider SoundEffectSlider;

    public AudioSource MainMusic;
    public AudioSource USMusic;
    public AudioSource USSRMusic;
    public AudioSource USWin;
    public AudioSource USDefeat;
    public AudioSource USSRWin;
    public AudioSource USSRDefeat;

    public AudioSource ButtonClick;
    public AudioSource GameDoorClose;
    public AudioSource GameDoorCreak;
    public AudioSource LightClick;
    public AudioSource PiecePlaced;
    public AudioSource Footsteps;

    void Awake()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
    }

    public void ToggleSettingsWindow()
    {
        SettingsWindow.SetActive(!SettingsWindow.activeSelf);
    }

    public void ChangeMainMusicVolume()
    {
        MainMusic.volume = MusicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void ChangeGameMusicVolume()
    {
        USMusic.volume = MusicSlider.value;
        USSRMusic.volume = MusicSlider.value;
        USWin.volume = MusicSlider.value;
        USDefeat.volume = MusicSlider.value;
        USSRWin.volume = MusicSlider.value;
        USSRDefeat.volume = MusicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void ChangeSoundEffectVolume()
    {
        ButtonClick.volume = SoundEffectSlider.value;
        GameDoorClose.volume = SoundEffectSlider.value;
        GameDoorCreak.volume = SoundEffectSlider.value;
        LightClick.volume = SoundEffectSlider.value;
        PiecePlaced.volume = SoundEffectSlider.value;
        Footsteps.volume = SoundEffectSlider.value;
        PlayerPrefs.SetFloat("SoundEffectsVolume", SoundEffectSlider.value);
    }

    public void RestoreDefaults()
    {
        PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        PlayerPrefs.SetFloat("SoundEffectsVolume", 0.5f);
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
    }
}