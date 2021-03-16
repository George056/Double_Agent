using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TradeUIScript : MonoBehaviour
{
    public TextMeshProUGUI copperTraded;
    public TextMeshProUGUI lumberTraded;
    public TextMeshProUGUI loyalistsTraded;
    public TextMeshProUGUI coinsTraded;

    public Button copperInButton;
    public Button lumberInButton;
    public Button loyalistInButton;
    public Button coinInButton;
    public Button submitButton;

    private int totalTradeCount = 0;

    public GameObject tradeWindow;

    List<int> tradeList = new List<int>(4) { 0, 0, 0, 0 };

    void EnableButtons()
    {
        copperInButton.interactable = true;
        lumberInButton.interactable = true;
        loyalistInButton.interactable = true;
        coinInButton.interactable = true;
    }

    void DisableButtons()
    {
        copperInButton.interactable = false;
        lumberInButton.interactable = false;
        loyalistInButton.interactable = false;
        coinInButton.interactable = false;
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
        int numCopperTraded = Convert.ToInt32(copperTraded.text);
        if (totalTradeCount == 3 || numCopperTraded == GameObject.FindObjectOfType<Player>().__resources[0])
        {
            totalTradeCount -= numCopperTraded;
            numCopperTraded = 0;
            copperTraded.text = numCopperTraded.ToString();
            tradeList[0] = 0;

            if (totalTradeCount < 3)
            {
                DisableButtons();
            }

            // Unity Forum User ensiferum888 - https://forum.unity.com/threads/how-to-turn-on-off-button-visibility.290317/
            copperInButton.gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numCopperTraded++;
            copperTraded.text = numCopperTraded.ToString();
            tradeList[0]--;

            if (totalTradeCount == 3)
            {
                EnableButtons();
            }

            copperInButton.gameObject.SetActive(false);
        }
    }

    public void selectLumberOut()
    {
        int numLumberTraded = Convert.ToInt32(lumberTraded.text);
        if (totalTradeCount == 3 || numLumberTraded == GameObject.FindObjectOfType<Player>().__resources[1])
        {
            totalTradeCount -= numLumberTraded;
            numLumberTraded = 0;
            lumberTraded.text = numLumberTraded.ToString();
            tradeList[1] = 0;

            if (totalTradeCount < 3)
            {
                DisableButtons();
            }

            lumberInButton.gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numLumberTraded++;
            lumberTraded.text = numLumberTraded.ToString();
            tradeList[1]--;

            if (totalTradeCount == 3)
            {
                EnableButtons();
            }

            lumberInButton.gameObject.SetActive(false);
        }
    }

    public void selectLoyalistOut()
    {
        int numLoyalistsTraded = Convert.ToInt32(loyalistsTraded.text);
        if (totalTradeCount == 3 || numLoyalistsTraded == GameObject.FindObjectOfType<Player>().__resources[2])
        {
            totalTradeCount -= numLoyalistsTraded;
            numLoyalistsTraded = 0;
            loyalistsTraded.text = numLoyalistsTraded.ToString();
            tradeList[2] = 0;

            if (totalTradeCount < 3)
            {
                DisableButtons();
            }

            loyalistInButton.gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numLoyalistsTraded++;
            loyalistsTraded.text = numLoyalistsTraded.ToString();
            tradeList[2]--;
            
            if (totalTradeCount == 3)
            {
                EnableButtons();
            }

            loyalistInButton.gameObject.SetActive(false);
        }
    }

    public void selectCoinOut()
    {
        int numCoinsTraded = Convert.ToInt32(coinsTraded.text);
        if (totalTradeCount == 3 || numCoinsTraded == GameObject.FindObjectOfType<Player>().__resources[3])
        {
            totalTradeCount -= numCoinsTraded;
            numCoinsTraded = 0;
            coinsTraded.text = numCoinsTraded.ToString();
            tradeList[3] = 0;

            if (totalTradeCount < 3)
            {
                DisableButtons();
            }

            coinInButton.gameObject.SetActive(true);
        }
        else
        {
            totalTradeCount++;
            numCoinsTraded++;
            coinsTraded.text = numCoinsTraded.ToString();
            tradeList[3]--;

            if (totalTradeCount == 3)
            {
                EnableButtons();
            }

            coinInButton.gameObject.SetActive(false);
        }
    }

    public void selectCopperIn()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tradeList[i] > 0)
            {
                tradeList[i] = 0;
            }
        }
        tradeList[0] = 1;

        submitButton.interactable = true;
    }

    public void selectLumberIn()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tradeList[i] > 0)
            {
                tradeList[i] = 0;
            }
        }
        tradeList[1] = 1;

        submitButton.interactable = true;
    }

    public void selectLoyalistIn()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tradeList[i] > 0)
            {
                tradeList[i] = 0;
            }
        }
        tradeList[2] = 1;

        submitButton.interactable = true;
    }

    public void selectCoinIn()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tradeList[i] > 0)
            {
                tradeList[i] = 0;
            }
        }
        tradeList[3] = 1;

        submitButton.interactable = true;
    }

    public void SubmitTrade()
    {
        coinsTraded.text = "0";
        loyalistsTraded.text = "0";
        copperTraded.text = "0";
        lumberTraded.text = "0";

        Owner playerSide = GameObject.FindObjectOfType<Player>().__piece_type;
        GameObject.FindObjectOfType<BoardManager>().Trade(tradeList, playerSide);

        tradeList[0] = 0;
        tradeList[1] = 0;
        tradeList[2] = 0;
        tradeList[3] = 0;

        totalTradeCount = 0;
        GameObject.FindObjectOfType<BoardManager>().playerTraded = true;

        DisableButtons();
        copperInButton.gameObject.SetActive(true);
        lumberInButton.gameObject.SetActive(true);
        loyalistInButton.gameObject.SetActive(true);
        coinInButton.gameObject.SetActive(true);

        submitButton.interactable = false;
    }

    public void CancelTrade()
    {
        coinsTraded.text = "0";
        loyalistsTraded.text = "0";
        copperTraded.text = "0";
        lumberTraded.text = "0";

        tradeList[0] = 0;
        tradeList[1] = 0;
        tradeList[2] = 0;
        tradeList[3] = 0;

        totalTradeCount = 0;

        DisableButtons();
        copperInButton.gameObject.SetActive(true);
        lumberInButton.gameObject.SetActive(true);
        loyalistInButton.gameObject.SetActive(true);
        coinInButton.gameObject.SetActive(true);

        submitButton.interactable = false;
    }
}