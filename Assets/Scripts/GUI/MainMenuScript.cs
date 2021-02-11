﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("PVP");
    }

    public void ViewTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Exit()
    {
        Debug.Log("Application Closed");
        Application.Quit();
    }
}
