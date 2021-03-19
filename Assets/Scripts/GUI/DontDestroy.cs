using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {   
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            Debug.Log("more than 1 music objects found");
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "PVP")
        {
            Destroy(this.gameObject);
        }
    }
}
