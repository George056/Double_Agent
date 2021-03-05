using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void PlayGame()
    //{
    //    SceneManager.LoadScene("PVP");
    //}
    public void OnlinePlay()
    {
        SceneManager.LoadScene("Lobby");
    }
}
