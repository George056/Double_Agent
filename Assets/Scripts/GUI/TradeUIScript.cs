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

    public GameObject tradeWindow;

    List<int> tradeList = new List<int>(4) { 0, 0, 0, 0 };

    public void toggleTradeWindow()
    {
        tradeWindow.SetActive(!tradeWindow.activeSelf);
    }

    public void selectCopperOut()
    {
        int numCopperTraded = Convert.ToInt32(copperTraded.text);
        if (numCopperTraded == 3)
        {
            numCopperTraded = 0;
            copperTraded.text = numCopperTraded.ToString();
            tradeList[0] = 0;
        }
        else
        {
            numCopperTraded++;
            copperTraded.text = numCopperTraded.ToString();
            tradeList[0]--;
        }
    }

    public void selectLumberOut()
    {
        int numLumberTraded = Convert.ToInt32(lumberTraded.text);
        if (numLumberTraded == 3)
        {
            numLumberTraded = 0;
            lumberTraded.text = numLumberTraded.ToString();
            tradeList[1] = 0;
        }
        else
        {
            numLumberTraded++;
            lumberTraded.text = numLumberTraded.ToString();
            tradeList[1]--;
        }
    }

    public void selectLoyalistOut()
    {
        int numLoyalistsTraded = Convert.ToInt32(loyalistsTraded.text);
        if (numLoyalistsTraded == 3)
        {
            numLoyalistsTraded = 0;
            loyalistsTraded.text = numLoyalistsTraded.ToString();
            tradeList[2] = 0;
        }
        else
        {
            numLoyalistsTraded++;
            loyalistsTraded.text = numLoyalistsTraded.ToString();
            tradeList[2]--;
        }
    }

    public void selectCoinOut()
    {
        int numCoinsTraded = Convert.ToInt32(coinsTraded.text);
        if (numCoinsTraded == 3)
        {
            numCoinsTraded = 0;
            coinsTraded.text = numCoinsTraded.ToString();
            tradeList[3] = 0;
        }
        else
        {
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
    }
}