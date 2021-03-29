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
    public AudioSource ButtonClick;
    public AudioSource GameDoorClose;
    public AudioSource GameDoorCreak;
    public AudioSource USCapture;
    public AudioSource USSRCapture;
    public AudioSource USWin;
    public AudioSource USDefeat;
    public AudioSource USSRWin;
    public AudioSource USSRDefeat;
    public AudioSource LightClick;
    public AudioSource PiecePlaced;

    void Awake()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SoundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
    }

    public void SettingsWindowPopUp()
    {
        SettingsWindow.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        SettingsWindow.SetActive(false);
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
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void ChangeSoundEffectVolume()
    {
        ButtonClick.volume = SoundEffectSlider.value;
        GameDoorClose.volume = SoundEffectSlider.value;
        GameDoorCreak.volume = SoundEffectSlider.value;
        USCapture.volume = SoundEffectSlider.value;
        USSRCapture.volume = SoundEffectSlider.value;
        USWin.volume = SoundEffectSlider.value;
        USDefeat.volume = SoundEffectSlider.value;
        USSRWin.volume = SoundEffectSlider.value;
        USSRDefeat.volume = SoundEffectSlider.value;
        LightClick.volume = SoundEffectSlider.value;
        PiecePlaced.volume = SoundEffectSlider.value;
        PlayerPrefs.SetFloat("SoundEffectsVolume", SoundEffectSlider.value);
    }
}