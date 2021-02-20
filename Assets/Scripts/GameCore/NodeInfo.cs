using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NodeInfo : MonoBehaviour
{

    private int cc;
    public BoardManager.Owner nodeOwner;
    public int nodeOrder;
    public List<GameObject> resources = new List<GameObject>();
    //public string countryAbreviation;
    //public string countryName;

    
    void OnMouseDown()
    {
        
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordNode())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalNodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches)) // localPlayer.Owner)) // or could use GameObject.FindObjectOfType<BoardManager>().activeSide for 2nd parameter
            {
                //Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOrder);
                //Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOwner);

                GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(nodeOrder);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddNode(nodeOrder);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForNode();
            }
        }
        
    }
}
