using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public GameObject[] Buttons;
    [HideInInspector] public GameObject[] UserInput;

    private bool DefaultInput;
    private static bool InConfig = false;
    private string[] InputOptions = new string[] { "Left", "Up", "Right", "Down", "Confirm", "Back", "Menu", "Map" };
    private KeyCode[] DefaultInputs = new KeyCode[] {KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.S, 
                                                    KeyCode.J, KeyCode.H, KeyCode.K, KeyCode.I};
    private KeyCode[] UserInputs = new KeyCode[8];
    private static int difficulty;

    private static Color32 BLACK = new Color32(0, 0, 0, 255);
    private static Color32 ACTIVE_DIFF_BTN = new Color32(25, 25, 25, 255);

    private const int DIFFICULTY_POS = 4;

    private void Start()
    {
        DefaultInput = PlayerPrefs.GetInt("Default", 1) == 1;
        difficulty = PlayerPrefs.GetInt("Difficulty", 1) + DIFFICULTY_POS;
        
        Buttons[difficulty].GetComponent<TextMeshProUGUI>().color = ACTIVE_DIFF_BTN;

        string temp = PlayerPrefs.GetString(InputOptions[0], "nil");
        
        for(int i = 1; i < 8 && temp != "nil"; i++)
        {

        }

    }

    private void Update()
    {

        if(InConfig && !DefaultInput)
        {
            GameObject.FindGameObjectWithTag("RestoreDefault").SetActive(true);
        }
        else if(InConfig && DefaultInput)
        {
            GameObject.FindGameObjectWithTag("RestoreDefault").SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var position = Input.mousePosition;
            bool buttonClicked = false;
            foreach(GameObject btn in Buttons)
            {
                if((position.x >= btn.transform.position.x - btn.transform.GetComponent<RectTransform>().sizeDelta.x && 
                    position.x <= btn.transform.position.x + btn.transform.GetComponent<RectTransform>().sizeDelta.x) &&
                    (position.y > btn.transform.position.y - btn.transform.GetComponent<RectTransform>().sizeDelta.y &&
                    position.y > btn.transform.position.y + btn.transform.GetComponent<RectTransform>().sizeDelta.y))
                {
                    buttonClicked = true;
                }
            }

            if (!buttonClicked)
            {

            }
        }
    }

    public void PlayBtn()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitBtn()
    {
        Application.Quit();
    }

    public void ConfigActive()
    {
        InConfig = true;
        GameObject.FindGameObjectWithTag("PlayingBtn").SetActive(false);
        GameObject.FindGameObjectWithTag("QuitBtn").SetActive(false);
        GameObject.FindGameObjectWithTag("Config").SetActive(true);
        if (DefaultInput)
        {
            GameObject.FindGameObjectWithTag("RestoreDefault").SetActive(false);
        }
    }

    public void ConfigInactive()
    {
        InConfig = false;
        GameObject.FindGameObjectWithTag("Config").SetActive(false);

        GameObject.FindGameObjectWithTag("PlayingBtn").SetActive(true);
        GameObject.FindGameObjectWithTag("QuitBtn").SetActive(true);
    }

    public void RestoreDefault()
    {

    }

    public void SelectDifficulty(int i)
    {
        GameObject[] difficultyBtns = GameObject.FindGameObjectsWithTag("DifficultyBtns");
        foreach(GameObject difficultyBtn in difficultyBtns)
        {
            difficultyBtn.GetComponent<TextMeshProUGUI>().color = BLACK;
        }
        difficultyBtns[i].GetComponent<TextMeshProUGUI>().color = ACTIVE_DIFF_BTN;
    }
}
