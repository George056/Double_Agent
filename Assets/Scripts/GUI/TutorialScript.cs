using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TutorialScript : MonoBehaviour
{
    public GameObject[] TutorialSlides = new GameObject[6];
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

    public void ResetTutorial()
    {
        TutorialSlides[0].SetActive(true);
        for (int i = 1; i < 6; i++) // hard code in array bounds
        {
            TutorialSlides[i].SetActive(false);
        }

        NextButton.interactable = true;
        BackButton.interactable = false;
        slideNum = 0;
    }

    public void TutorialBack()
    {
        TutorialSlides[slideNum].SetActive(false);
        if (slideNum == 5) //Hard code the index of the last slide
        {
            NextButton.interactable = true;
        }
        slideNum--;
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
        if(slideNum == 5) //Hard code in array bounds
        {
            NextButton.interactable = false;
        }
        TutorialSlides[slideNum].SetActive(true);
    }
}
