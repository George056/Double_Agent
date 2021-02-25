using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreGameMenu : MonoBehaviour
{
    public InputField customBoardInput;

    /// <summary>
    /// This is used to set the difficulty of the AI
    /// </summary>
    /// <param name="diff">What is the difficulty of the AI (0 = Easy, 1 = hard)</param>
    public void SetDifficulty(Difficulty diff)
    {
        PlayerPrefs.SetInt("Difficulty", (int)diff);
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
        PlayerPrefs.SetString("CustomBoardSeed", customBoardInput.text);
        SceneManager.LoadScene("PVP");
    }
    
    public void OnlinePlay()
    {
        SceneManager.LoadScene("Lobby");
    }
}
