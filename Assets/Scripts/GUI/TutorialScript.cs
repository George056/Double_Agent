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
    public GameObject[] TutorialSlidesTest = new GameObject[4];
    private Vector3 defaultPos;
    private Vector3 targetPos = new Vector3(150, 613, 0);
    private int tempIndex;
    private int tempSlideNum;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[i].transform.SetSiblingIndex(TutorialSlidesTest.Length - i);
            Debug.Log("slide " + i + " 's siblingIndex is " + TutorialSlidesTest[i].transform.GetSiblingIndex());
        }
        defaultPos = TutorialSlidesTest[0].transform.position; // (960.0, 613.1, 0.0)
        Debug.Log("defaultPos is: " + defaultPos);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetTutorial()
    {
        //TutorialSlides[0].SetActive(true);
        //for (int i = 1; i < 6; i++) // hard code in array bounds
        //{
        //    TutorialSlides[i].SetActive(false);
        //}
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[i].transform.SetSiblingIndex(TutorialSlidesTest.Length - i);
            Debug.Log("slide " + i + " 's siblingIndex is " + TutorialSlidesTest[i].transform.GetSiblingIndex());
        }

        NextButton.interactable = true;
        BackButton.interactable = false;
        slideNum = 0;
    }

    public void TutorialBack()
    {
        //TutorialSlides[slideNum].SetActive(false);
        //if (slideNum == 5) //Hard code the index of the last slide
        //{
        //    NextButton.interactable = true;
        //}
        //slideNum--;
        //if (slideNum == 0)
        //{
        //    BackButton.interactable = false;
        //}
        //TutorialSlides[slideNum].SetActive(true);
        StartCoroutine(MovePicBack(defaultPos, targetPos, 1700));

    }

    public void TutorialNext()
    {
        //TutorialSlides[slideNum].SetActive(false);
        //if(slideNum == 0)
        //{
        //    BackButton.interactable = true;
        //}
        //slideNum++;
        //if(slideNum == 5) //Hard code in array bounds
        //{
        //    NextButton.interactable = false;
        //}
        //TutorialSlides[slideNum].SetActive(true);
        StartCoroutine(MovePicNext(defaultPos, targetPos, 1700));
    }
    private IEnumerator MovePicNext(Vector3 deafultPos, Vector3 targetPos, float speed)
    {
        BackButton.interactable = false;
        NextButton.interactable = false;

        float startTime = Time.time;
        float length = Vector3.Distance(deafultPos, targetPos);
        float frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            TutorialSlidesTest[slideNum].transform.position = Vector3.Lerp(deafultPos, targetPos, frac);
            yield return null;
        }
        //Debug.Log("new pos: " + TutorialSlidesTest[0].transform.position);
        tempSlideNum = slideNum;
        tempIndex = 1;
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[tempSlideNum].transform.SetSiblingIndex(tempIndex);
            Debug.Log("slide " + tempSlideNum + " 's siblingIndex is " + TutorialSlidesTest[tempSlideNum].transform.GetSiblingIndex());
            tempIndex++;
            if (tempSlideNum == 0)
            {
                tempSlideNum = TutorialSlidesTest.Length - 1;
            }
            else
                tempSlideNum--;
        }

        startTime = Time.time;
        length = Vector3.Distance(deafultPos, targetPos);
        frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            TutorialSlidesTest[slideNum].transform.position = Vector3.Lerp(targetPos, deafultPos, frac);
            yield return null;

        }
        slideNum++;
        BackButton.interactable = true;
        if (slideNum < TutorialSlidesTest.Length - 1)
        {
            NextButton.interactable = true;
        }
        //Debug.Log("new pos: " + TutorialSlidesTest[0].transform.position);
    }

    private IEnumerator MovePicBack(Vector3 deafultPos, Vector3 targetPos, float speed)
    {

        BackButton.interactable = false;
        NextButton.interactable = false;

        float startTime = Time.time;
        float length = Vector3.Distance(deafultPos, targetPos);
        float frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            TutorialSlidesTest[slideNum - 1].transform.position = Vector3.Lerp(deafultPos, targetPos, frac);
            yield return null;
        }
        //Debug.Log("new pos: " + TutorialSlidesTest[0].transform.position);

        tempSlideNum = slideNum - 1;
        tempIndex = TutorialSlidesTest.Length;
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[tempSlideNum].transform.SetSiblingIndex(tempIndex);
            Debug.Log("slide " + tempSlideNum + " 's siblingIndex is " + TutorialSlidesTest[tempSlideNum].transform.GetSiblingIndex());
            tempIndex--;
            if (tempSlideNum == TutorialSlidesTest.Length - 1)
            {
                tempSlideNum = 0;
            }
            else
                tempSlideNum++;
        }

        startTime = Time.time;
        length = Vector3.Distance(deafultPos, targetPos);
        frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            TutorialSlidesTest[slideNum - 1].transform.position = Vector3.Lerp(targetPos, deafultPos, frac);
            yield return null;

        }

        slideNum--;
        NextButton.interactable = true;
        if (slideNum > 0)
        {
            BackButton.interactable = true;
        }
        //Debug.Log("new pos: " + TutorialSlidesTest[0].transform.position);
    }


}
