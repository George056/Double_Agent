using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevelAfterDelay());
    }

    IEnumerator LoadLevelAfterDelay()
    {
        yield return new WaitForSeconds(8.0f);
        SceneManager.LoadScene("MainMenuScene");
    }
}