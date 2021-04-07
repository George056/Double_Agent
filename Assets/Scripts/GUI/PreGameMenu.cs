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
       GameInfo.game_diff = diff;
    }

    public void SetPlayerOrder()
    {
        if (playerFirstToggle.isOn)
        {
            GameInfo.human_player = 0;
            GameInfo.ai_player = 1;
        }
        else
        {
            GameInfo.human_player = 1;
            GameInfo.ai_player = 0;
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
            GameInfo.human_player = 0;
            GameInfo.ai_player = 1;
        }
        else
        {
            GameInfo.human_player = 1;
            GameInfo.ai_player = 0;
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
            GameInfo.human_piece = (int)owner;
            GameInfo.ai_piece = (int)owner + 1;
        }
        else
        {
            GameInfo.human_piece = (int)owner;
            GameInfo.ai_piece = (int)owner - 1;
        }
    }

    public void ChooseAlly(bool US)
    {
        if (US)
        {
            GameInfo.human_piece = 0;
            GameInfo.ai_piece = 1;
        }
        else
        {
            GameInfo.human_piece = 1;
            GameInfo.ai_piece = 0;
        }
    }

    public void PlayGame()
    {
        GameInfo.game_type = "local";
        GameInfo.custom_board_seed = customBoardInput.text;
        SceneManager.LoadScene("PVP");
    }
    
    public void SetUserName()
    {
        GameInfo.user_name = userName.text;
        Debug.Log("UserName: " + GameInfo.user_name);
    }
    public void OnlinePlay()
    {
        GameInfo.network_player = 0;
        GameInfo.network_piece = 0;
        GameInfo.game_type = "net";
        SceneManager.LoadScene("Lobby");
    }
}
