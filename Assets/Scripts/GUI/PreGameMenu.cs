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
    public bool USAlly = true;
    public bool USAllyNetwork = true;
    public GameObject allyToggle;
    public GameObject allyToggleNetwork;
    public Sprite US;
    public Sprite USSR;

    public GameObject userNameTooLong;
    public GameObject userNameEmpty;

    public int Diff = 0;
    public GameObject DiffToggle;
    public Sprite easy;
    public Sprite hard;

    private void Awake()
    {
        if (GameInfo.pregame_menu_first_visit != true)
        {
            userName.text = GameInfo.user_name;
        }
    }
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

    public void ChooseAllyNetwork (bool US)
    {
        if (US)
        {
            GameInfo.network_piece = 0;
        }
        else
        {
            GameInfo.network_piece = 1;
        }
    }

    public void PlayGame()
    {
        GameInfo.game_type = "local";
        GameInfo.custom_board_seed = customBoardInput.text;
        ChooseAlly(USAlly);
        SetDifficulty(Diff);
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
        ChooseAllyNetwork(USAllyNetwork);
        Debug.Log("PregameMenu Net Piece = " + GameInfo.network_piece);
        GameInfo.game_type = "net";

        if (CheckUserName(GameInfo.user_name))
        {
            GameInfo.pregame_menu_first_visit = false;
            SceneManager.LoadScene("Lobby");
        }
    }

    public void ChooseSideToggle()
    {
        USAlly = !USAlly;
        
        if (USAlly)
        {
            allyToggle.GetComponent<Image>().sprite = US;
        }
        else
        {
            allyToggle.GetComponent<Image>().sprite = USSR;
        }
    }

    public void ChooseSideToggleNetwork()
    {
        USAllyNetwork = !USAllyNetwork;

        Debug.Log("USAllyNetwork: " + USAllyNetwork);

        if (USAllyNetwork)
        {
            allyToggleNetwork.GetComponent<Image>().sprite = US;
        }
        else
        {
            allyToggleNetwork.GetComponent<Image>().sprite = USSR;
        }
    }

    public void ChooseDiffToggle()
    {
        if(Diff == 1)
        {
            Diff = 0;
        }
        else
        {
            Diff = 1;
        }
        
        if (Diff == 0)
        {
            DiffToggle.GetComponent<Image>().sprite = easy;
        }
        else
        {
            DiffToggle.GetComponent<Image>().sprite = hard;
        }
    }

    public bool CheckUserName(string name)
    {
        bool userNameOkay = false;
        if(name.Trim() == "")
        {
            userNameEmpty.gameObject.SetActive(true);
            userNameTooLong.gameObject.SetActive(false);
        }
        else if(name.Trim().Length > 15)
        {
            userNameTooLong.gameObject.SetActive(true);
            userNameEmpty.gameObject.SetActive(false);           
        }
        else
        {
            userNameOkay = true;
            userNameTooLong.gameObject.SetActive(false);
            userNameEmpty.gameObject.SetActive(false);
        }

        return userNameOkay;
    }
}
