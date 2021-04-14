using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyVolumeScript : MonoBehaviour
{
    public AudioSource ButtonClick;

    void Awake()
    {
        ButtonClick.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
    }
}
