using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TradeUIScript : MonoBehaviour
{
    public TextMeshProUGUI[] tradeNumbers = new TextMeshProUGUI[4];
    public Button[] inButtons = new Button[4];

    public Button submitButton;

    private int totalTradeCount = 0;

    public GameObject tradeWindow;

    List<int> tradeList = new List<int>(4) { 0, 0, 0, 0 };

    void EnableInButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            inButtons[i].interactable = true;
        }
    }

    void DisableInButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            inButtons[i].interactable = false;
        }
    }

    public void ClearSelectedResource()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tradeList[i] > 0)
            {
                tradeList[i] = 0;
            }
        }
    }

    public void toggleTradeWindow()
    {
        if (!GameObject.FindObjectOfType<BoardManager>().playerTraded)
        {
            tradeWindow.SetActive(!tradeWindow.activeSelf);
        }
    }

    public void selectCopperOut()
    {
        int numCopperTraded = Convert.ToInt32(tradeNumbers[0].text);
        if (totalTradeCount == 3 || numCopperTraded == GameObject.FindObjectOfType<Player>().__resources[0])
        {
            totalTradeCount -= numCopperTraded;
            numCopperTraded = 0;
            tradeNumbers[0].text = numCopperTraded.ToString();
            tradeNumbers[0].enabled = false;
            tradeList[0] = 0;

            if (totalTradeCount < 3)
            {
                DisableInButtons();
            }

            // Unity Forum User ensiferum888 - https://forum.unity.com/threads/how-to-turn-on-off-button-visibility.290317/
            inButtons[0].gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numCopperTraded++;
            tradeNumbers[0].text = numCopperTraded.ToString();
            tradeNumbers[0].enabled = true;
            tradeList[0]--;

            if (totalTradeCount == 3)
            {
                EnableInButtons();
            }

            inButtons[0].gameObject.SetActive(false);
        }
    }

    public void selectLumberOut()
    {
        int numLumberTraded = Convert.ToInt32(tradeNumbers[1].text);
        if (totalTradeCount == 3 || numLumberTraded == GameObject.FindObjectOfType<Player>().__resources[1])
        {
            totalTradeCount -= numLumberTraded;
            numLumberTraded = 0;
            tradeNumbers[1].text = numLumberTraded.ToString();
            tradeNumbers[1].enabled = false;
            tradeList[1] = 0;

            if (totalTradeCount < 3)
            {
                DisableInButtons();
            }

            inButtons[1].gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numLumberTraded++;
            tradeNumbers[1].text = numLumberTraded.ToString();
            tradeNumbers[1].enabled = true;
            tradeList[1]--;

            if (totalTradeCount == 3)
            {
                EnableInButtons();
            }

            inButtons[1].gameObject.SetActive(false);
        }
    }

    public void selectLoyalistOut()
    {
        int numLoyalistsTraded = Convert.ToInt32(tradeNumbers[2].text);
        if (totalTradeCount == 3 || numLoyalistsTraded == GameObject.FindObjectOfType<Player>().__resources[2])
        {
            totalTradeCount -= numLoyalistsTraded;
            numLoyalistsTraded = 0;
            tradeNumbers[2].text = numLoyalistsTraded.ToString();
            tradeNumbers[2].enabled = false;
            tradeList[2] = 0;

            if (totalTradeCount < 3)
            {
                DisableInButtons();
            }

            inButtons[2].gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numLoyalistsTraded++;
            tradeNumbers[2].text = numLoyalistsTraded.ToString();
            tradeNumbers[2].enabled = true;
            tradeList[2]--;
            
            if (totalTradeCount == 3)
            {
                EnableInButtons();
            }

            inButtons[2].gameObject.SetActive(false);
        }
    }

    public void selectCoinOut()
    {
        int numCoinsTraded = Convert.ToInt32(tradeNumbers[3].text);
        if (totalTradeCount == 3 || numCoinsTraded == GameObject.FindObjectOfType<Player>().__resources[3])
        {
            totalTradeCount -= numCoinsTraded;
            numCoinsTraded = 0;
            tradeNumbers[3].text = numCoinsTraded.ToString();
            tradeNumbers[3].enabled = false;
            tradeList[3] = 0;

            if (totalTradeCount < 3)
            {
                DisableInButtons();
            }

            inButtons[3].gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numCoinsTraded++;
            tradeNumbers[3].text = numCoinsTraded.ToString();
            tradeNumbers[3].enabled = true;
            tradeList[3]--;

            if (totalTradeCount == 3)
            {
                EnableInButtons();
            }

            inButtons[3].gameObject.SetActive(false);
        }
    }

    public void selectCopperIn()
    {
        ClearSelectedResource();
        tradeList[0] = 1;

        submitButton.interactable = true;
    }

    public void selectLumberIn()
    {
        ClearSelectedResource();
        tradeList[1] = 1;

        submitButton.interactable = true;
    }

    public void selectLoyalistIn()
    {
        ClearSelectedResource();
        tradeList[2] = 1;

        submitButton.interactable = true;
    }

    public void selectCoinIn()
    {
        ClearSelectedResource();
        tradeList[3] = 1;

        submitButton.interactable = true;
    }

    public void ResetTradeWindow()
    {
        for (int i = 0; i < 4; i++)
        {
            tradeNumbers[i].text = "0";
            tradeNumbers[i].enabled = false;
            tradeList[i] = 0;
            inButtons[i].gameObject.SetActive(true);
        }

        DisableInButtons();
        totalTradeCount = 0;
        submitButton.interactable = false;
    }

    public void SubmitTrade()
    {
        Owner playerSide = GameObject.FindObjectOfType<Player>().__piece_type;
        GameObject.FindObjectOfType<BoardManager>().Trade(tradeList, playerSide);

        GameObject.FindObjectOfType<BoardManager>().playerTraded = true;

        ResetTradeWindow();
    }
}