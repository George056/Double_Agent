using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void quitGame()
    {
        PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        PlayerPrefs.SetFloat("SoundEffectsVolume", 0.5f);
        Application.Quit();
    }
}
