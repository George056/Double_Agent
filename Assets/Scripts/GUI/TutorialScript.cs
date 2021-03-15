using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TutorialScript : MonoBehaviour
{
    public GameObject[] TutorialSlides = new GameObject[2];
    public Button BackButton;
    public Button NextButton;
    int slideNum = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void TutorialBack()
    {
        TutorialSlides[slideNum].SetActive(false);
        slideNum--;
        if(slideNum == 0) //Hard code in array bounds
        {
            NextButton.interactable = true;
        }
        if(slideNum == 0)
        {
            BackButton.interactable = false;
        }
        TutorialSlides[slideNum].SetActive(true);

    }

    public void TutorialNext()
    {
        TutorialSlides[slideNum].SetActive(false);
        if(slideNum == 0)
        {
            BackButton.interactable = true;
        }
        slideNum++;
        if(slideNum == 1) //Hard code in array bounds
        {
            NextButton.interactable = false;
        }
        TutorialSlides[slideNum].SetActive(true);
    }
}
