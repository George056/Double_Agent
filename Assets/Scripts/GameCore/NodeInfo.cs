using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeInfo : MonoBehaviour
{
    public Owner nodeOwner;
    public int nodeOrder;
    public bool isSetupNode = false;
    public bool placementConfirmed = false;
    public bool connectedToSetup = false;

    // List of resources the node gives
    public List<GameObject> resources = new List<GameObject>(0);

    bool CanUnplace(int nodeToUnplace)
    {
        bool canUnplace = true;

        Relationships.connectionsNode.TryGetValue(nodeToUnplace, out List<int> adjacentBranches);
        if (GameObject.FindObjectOfType<BoardManager>().turnCount < 3)
        { 
            foreach (int i in adjacentBranches)
            {
                if (i == GameObject.FindObjectOfType<BoardManager>().firstSetupBranch)
                    canUnplace = false;
            }
        }
        else if (GameObject.FindObjectOfType<BoardManager>().turnCount == 3 || GameObject.FindObjectOfType<BoardManager>().turnCount == 4)
        {
            foreach (int i in adjacentBranches)
            {
                if (i == GameObject.FindObjectOfType<BoardManager>().secondSetupBranch)
                    canUnplace = false;
            }
        }

        return canUnplace;
    }
    
    void OnMouseDown()
    {
        Debug.Log("Node Owner: " + GameObject.FindObjectOfType<BoardManager>().nodes[nodeOrder].GetComponent<NodeInfo>().nodeOwner);

        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode)
        {

            if (GameObject.FindObjectOfType<BoardManager>().nodes[nodeOrder].GetComponent<NodeInfo>().nodeOwner == GameObject.FindObjectOfType<BoardManager>().activeSide &&
                !GameObject.FindObjectOfType<BoardManager>().nodes[nodeOrder].GetComponent<NodeInfo>().placementConfirmed)
            {
                if (CanUnplace(nodeOrder))
                {
                    GameObject.FindObjectOfType<BoardManager>().UnplaceNode(nodeOrder);

                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().RemoveNode(nodeOrder);

                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ReimburseForNode();
                }

            }
            else if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordNode())
            {
                if (GameObject.FindObjectOfType<BoardManager>().LegalUINodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
                {
                    GameObject.FindObjectOfType<BoardManager>().PiecePlaced.Play(0);

                    GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(nodeOrder);

                    // Adds node to player's list of owned nodes
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddNode(nodeOrder);

                    // Removes two coins and two loyalists from player's resources
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForNode();
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordNode())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalUINodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                if (GameObject.FindObjectOfType<BoardManager>().activeSide == Owner.US)
                {
                    this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
                }
            }
        }
    }

    void OnMouseExit()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordNode())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalUINodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(104, 118, 137, 255);
            }
        }
    }
}