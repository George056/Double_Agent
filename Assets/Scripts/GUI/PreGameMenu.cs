using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreGameMenu : MonoBehaviour
{
    public InputField customBoardInput;
    public InputField userName;
    public string userNameText;
    public Toggle playerFirstToggle;

    /// <summary>
    /// This is used to set the difficulty of the AI
    /// </summary>
    /// <param name="diff">What is the difficulty of the AI (0 = Easy, 1 = hard)</param>
    public void SetDifficulty(int diff)
    {
        PlayerPrefs.SetInt("Difficulty", diff);
    }

    public void SetPlayerOrder()
    {
        if (playerFirstToggle.isOn)
        {
            PlayerPrefs.SetInt("Human_Player", 0);
            PlayerPrefs.SetInt("AI_Player", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Human_Player", 1);
            PlayerPrefs.SetInt("AI_Player", 0);
        }
    }
    
    /// <summary>
    /// Who will play first, the AI or human?
    /// </summary>
    /// <param name="first">If true the human playes first</param>
    public void SetOrder(bool first)
    {
        if (first)
        {
            PlayerPrefs.SetInt("Human_Player", 0);
            PlayerPrefs.SetInt("AI_Player", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Human_Player", 1);
            PlayerPrefs.SetInt("AI_Player", 0);
        } 
    }

    /// <summary>
    /// What piece is the human?
    /// </summary>
    /// <param name="owner">The piece that the human is</param>
    public void SetPieceType(Owner owner)
    {
        if(owner == Owner.US)
        {
            PlayerPrefs.SetInt("Human_Piece", (int)owner);
            PlayerPrefs.SetInt("AI_Piece", (int)owner + 1);
        }
        else
        {
            PlayerPrefs.SetInt("Human_Piece", (int)owner); 
            PlayerPrefs.SetInt("AI_Piece", (int)owner - 1);
        }
    }

    public void ChooseAlly(bool US)
    {
        if (US)
        {
            PlayerPrefs.SetInt("Human_Piece", 0);
            PlayerPrefs.SetInt("AI_Piece", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Human_Piece", 1);
            PlayerPrefs.SetInt("AI_Piece", 0);
        }
    }

    public void PlayGame()
    {
        PlayerPrefs.SetString("GameType", "local");
        PlayerPrefs.SetString("CustomBoardSeed", customBoardInput.text);
        SceneManager.LoadScene("PVP");
    }
    
    public void SetUserName()
    {
        userNameText = userName.text;
        PlayerPrefs.SetString("UserName", userNameText);
    }
    public void OnlinePlay()
    {
        PlayerPrefs.SetInt("Network_Player", 0);
        PlayerPrefs.SetInt("Network_Piece", 0);
        PlayerPrefs.SetString("GameType", "net");
        SceneManager.LoadScene("Lobby");
    }
}
