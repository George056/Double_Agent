using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    public Owner branchOwner;
    public int branchOrder;
    public bool isSetupBranch = false;
    public bool placementConfirmed = false;
    public bool connectedToSetup = false;

    public void MarkConnectedPieces(int currentBranch)
    {
        Debug.Log("Calling markConnectedPieces on Branch " + currentBranch);

        GameObject.FindObjectOfType<BoardManager>().allBranches[currentBranch].GetComponent<BranchInfo>().connectedToSetup = true;

        Relationships.connectionsRoad.TryGetValue(currentBranch, out List<int> connectedBranches);

        foreach (int i in connectedBranches)
        {
            Debug.Log("Branch " + i + " connectedToSetup = " + GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().connectedToSetup);
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches.Contains(i) &&
                !GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().connectedToSetup)
            {
                MarkConnectedPieces(i);
            }
        }

        Relationships.connectionsRoadNode.TryGetValue(currentBranch, out List<int> connectedNodes);

        foreach (int i in connectedNodes)
        {
            Debug.Log("Node " + i + " connectedToSetup = " + GameObject.FindObjectOfType<BoardManager>().nodes[i].GetComponent<NodeInfo>().connectedToSetup);
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_nodes.Contains(i))
            {
                GameObject.FindObjectOfType<BoardManager>().nodes[i].GetComponent<NodeInfo>().connectedToSetup = true;
            }
        }
    }

    public bool CanUnplace(int branchToUnplace)
    {
        // ******** ATTEMPT 3 ********

        for (int i = 0; i < 36; i++)
        {
            Debug.Log("Branch " + i + " connectedToSetup = " + GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().connectedToSetup);
        }

        bool canUnplace = true;

        // mark as connected everything still accessible from setupBranches
        GameObject.FindObjectOfType<BoardManager>().allBranches[branchToUnplace].GetComponent<BranchInfo>().connectedToSetup = true;

        if (GameObject.FindObjectOfType<BoardManager>().firstSetupBranch != -1) 
            MarkConnectedPieces(GameObject.FindObjectOfType<BoardManager>().firstSetupBranch);

        if (GameObject.FindObjectOfType<BoardManager>().secondSetupBranch != -1)
            MarkConnectedPieces(GameObject.FindObjectOfType<BoardManager>().secondSetupBranch);

        // loop through player's owned branches and nodes; if any are not marked as connected, set canUnplace = false;
        foreach (int i in GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches)
        {
            if (!GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().connectedToSetup)
            {
                Debug.Log("Found isolated branch : " + i);
                canUnplace = false;
            }
        }

        foreach (int i in GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_nodes)
        {
            if (!GameObject.FindObjectOfType<BoardManager>().nodes[i].GetComponent<NodeInfo>().connectedToSetup)
            {
                Debug.Log("Found isolated node : " + i);
                canUnplace = false;
            }
        }

        return canUnplace;
    }

    void OnMouseDown()
    {
        Debug.Log("Branch Owner: " + GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().branchOwner);

        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode)
        {
            if (GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().branchOwner == GameObject.FindObjectOfType<BoardManager>().activeSide &&
                !GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().placementConfirmed)
            {
                if (CanUnplace(branchOrder))
                {
                    GameObject.FindObjectOfType<BoardManager>().UnplaceBranch(branchOrder);

                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().RemoveBranch(branchOrder);

                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ReimburseForBranch();
                }                
            }
            else if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
            {
                Debug.Log("Can afford branch " + branchOrder);
                
                if (GameObject.FindObjectOfType<BoardManager>().LegalUIBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
                {
                    Debug.Log("Branch " + branchOrder + " is legal move");

                    GameObject.FindObjectOfType<BoardManager>().PiecePlaced.Play(0);

                    GameObject.FindObjectOfType<BoardManager>().ChangeBranchOwner(branchOrder);

                    // Adds branch to player's list of owned branches
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddBranch(branchOrder);

                    // Removes one copper and one lumber from player's resources
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForBranch();
                }
            }

            for (int i = 0; i < 36; i++)
            {
                GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().connectedToSetup = false;
            }

            for (int i = 0; i < 24; i++)
            {
                GameObject.FindObjectOfType<BoardManager>().nodes[i].GetComponent<NodeInfo>().connectedToSetup = false;
            }
        }
    }

    void OnMouseEnter()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalUIBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                if (GameObject.FindObjectOfType<BoardManager>().activeSide == Owner.US)
                {
                    //this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(43, 56, 107, 255);
                    this.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
                    this.GetComponent<SpriteRenderer>().sprite = GameObject.FindObjectOfType<BoardManager>().USBranchSprite;
                }
                else
                {
                    //this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(107, 31, 37, 255);
                    this.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
                    this.GetComponent<SpriteRenderer>().sprite = GameObject.FindObjectOfType<BoardManager>().USSRBranchSprite;
                }
            }
        }
    }

    void OnMouseExit()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalUIBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                this.GetComponent<SpriteRenderer>().sprite = GameObject.FindObjectOfType<BoardManager>().EmptyBranchSprite;
                this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(156, 167, 176, 255);
            }
        }
    }
}
