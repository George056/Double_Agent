using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TradeUIScript : MonoBehaviour
{
    public TextMeshProUGUI copperTraded;
    public TextMeshProUGUI lumberTraded;
    public TextMeshProUGUI loyalistsTraded;
    public TextMeshProUGUI coinsTraded;

    private int totalTradeCount = 0;

    public GameObject tradeWindow;

    List<int> tradeList = new List<int>(4) { 0, 0, 0, 0 };

    //List<int> playerResources = GameObject.FindObjectOfType<Player>().__resources;

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
        }
        else
        {
            totalTradeCount++;
            numCopperTraded++;
            copperTraded.text = numCopperTraded.ToString();
            tradeList[0]--;
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
        }
        else
        {
            totalTradeCount++;
            numLumberTraded++;
            lumberTraded.text = numLumberTraded.ToString();
            tradeList[1]--;
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
        }
        else
        {
            totalTradeCount++;
            numLoyalistsTraded++;
            loyalistsTraded.text = numLoyalistsTraded.ToString();
            tradeList[2]--;
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
        }
        else
        {
            totalTradeCount++;
            numCoinsTraded++;
            coinsTraded.text = numCoinsTraded.ToString();
            tradeList[3]--;
        }
    }

    public void selectCopperIn()
    {
        tradeList[0] = 1;
    }

    public void selectLumberIn()
    {
        tradeList[1] = 1;
    }

    public void selectLoyalistIn()
    {
        tradeList[2] = 1;
    }

    public void selectCoinIn()
    {
        tradeList[3] = 1;
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
    }
}