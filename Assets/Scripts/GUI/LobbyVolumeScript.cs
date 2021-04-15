using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyVolumeScript : MonoBehaviour
{
    public AudioSource ButtonClick;
    public AudioSource drumMusic;
    public AudioSource mainMusic;

    void Awake()
    {
        ButtonClick.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
        drumMusic.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        mainMusic.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }
}
