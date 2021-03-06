using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TutorialScript : MonoBehaviour
{
    public Button BackButton;
    public Button NextButton;
    private int slideNum = 0;
    private int timesCount = 0;
    public GameObject[] TutorialSlidesTest = new GameObject[12];
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
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[i].transform.SetSiblingIndex(TutorialSlidesTest.Length - i);
            Debug.Log("slide " + i + " 's siblingIndex is " + TutorialSlidesTest[i].transform.GetSiblingIndex());
            TutorialSlidesTest[i].transform.position = defaultPos;

            //https://answers.unity.com/questions/1187850/how-do-i-make-gameobjecttransformrotationz-equal-t.html
            TutorialSlidesTest[i].transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        NextButton.interactable = true;
        BackButton.interactable = true;
        slideNum = 0;
    }

    public void TutorialBack()
    {                
        StartCoroutine(MovePicBack(defaultPos, targetPos, 4000));
    }

    public void TutorialNext()
    {
        StartCoroutine(MovePicNext(defaultPos, targetPos, 4000));
    }

    private IEnumerator MovePicNext(Vector3 deafultPos, Vector3 targetPos, float speed)
    {
        BackButton.interactable = false;
        NextButton.interactable = false;

        TutorialSlidesTest[slideNum].transform.Rotate(new Vector3(0, 0, 15));

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

        yield return new WaitForSeconds(0.075f);

        tempSlideNum = slideNum;
        tempIndex = 1;
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[tempSlideNum].transform.SetSiblingIndex(tempIndex);
            //Debug.Log("slide " + tempSlideNum + " 's siblingIndex is " + TutorialSlidesTest[tempSlideNum].transform.GetSiblingIndex());
            tempIndex++;
            if (tempSlideNum == 0)
            {
                tempSlideNum = TutorialSlidesTest.Length - 1;
            }
            else
                tempSlideNum--;
        }

        TutorialSlidesTest[slideNum].transform.Rotate(new Vector3(0, 0, -15));

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

        if (slideNum < TutorialSlidesTest.Length - 1)
        {
            slideNum++;
        }
        else
        {
            slideNum = 0;
        }
        BackButton.interactable = true;
        NextButton.interactable = true;

        //slideNum++;
        //BackButton.interactable = true;
        //if (slideNum < TutorialSlidesTest.Length - 1)
        //{
        //    NextButton.interactable = true;
        //}

    }

    private IEnumerator MovePicBack(Vector3 deafultPos, Vector3 targetPos, float speed)
    {
        int slideToMove = (slideNum > 0) ? slideNum - 1 : TutorialSlidesTest.Length - 1;

        BackButton.interactable = false;
        NextButton.interactable = false;

        TutorialSlidesTest[slideToMove].transform.Rotate(new Vector3(0, 0, 15));

        float startTime = Time.time;
        float length = Vector3.Distance(deafultPos, targetPos);
        float frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            TutorialSlidesTest[slideToMove].transform.position = Vector3.Lerp(deafultPos, targetPos, frac);
            yield return null;
        }

        yield return new WaitForSeconds(0.075f);

        TutorialSlidesTest[slideToMove].transform.Rotate(new Vector3(0, 0, -15));

        tempSlideNum = slideToMove;
        tempIndex = TutorialSlidesTest.Length;
        for (int i = 0; i < TutorialSlidesTest.Length; i++)
        {
            TutorialSlidesTest[tempSlideNum].transform.SetSiblingIndex(tempIndex);
           // Debug.Log("slide " + tempSlideNum + " 's siblingIndex is " + TutorialSlidesTest[tempSlideNum].transform.GetSiblingIndex());
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
            TutorialSlidesTest[slideToMove].transform.position = Vector3.Lerp(targetPos, deafultPos, frac);
            yield return null;

        }

        if (slideNum > 0)
        {
            slideNum--;
        }
        else
        {
            slideNum = TutorialSlidesTest.Length - 1;
        }
        BackButton.interactable = true;
        NextButton.interactable = true;

        //slideNum--;
        //NextButton.interactable = true;
        //if (slideNum > 0)
        //{
        //    BackButton.interactable = true;
        //}
        //Debug.Log("new pos: " + TutorialSlidesTest[0].transform.position);
    }

}
