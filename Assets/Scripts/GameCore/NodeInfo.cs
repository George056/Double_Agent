using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeInfo : MonoBehaviour
{
    public Owner nodeOwner;
    public int nodeOrder;

    // List of resources the node gives
    public List<GameObject> resources = new List<GameObject>(0);
    
    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordNode())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalNodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(nodeOrder);

                // Adds node to player's list of owned nodes
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddNode(nodeOrder);

                // Removes two coins and two loyalists from player's resources
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForNode();
            }
        }
    }
}
