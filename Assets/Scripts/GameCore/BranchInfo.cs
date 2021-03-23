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

    List<int> visitedBranches = new List<int>();

    //bool IsConnected(int branch)
    //{
    //    bool connectsToSetup = false;

    //    if (!visitedBranches.Contains(branch))
    //    {
    //        visitedBranches.Add(branch);

    //        Relationships.connectionsRoad.TryGetValue(branchOrder, out List<int> neighborBranches);
    //        foreach (int i in neighborBranches)
    //        {
    //            if (GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().isSetupBranch)
    //            {
    //                connectsToSetup = true;
    //                break;
    //            }
    //            else if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches.Contains(i) &&
    //                !visitedBranches.Contains(i))
    //            {
    //                connectsToSetup = IsConnected(i);
    //            }
    //        }
    //    }

    //    return connectsToSetup;
    //}

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

        // ******** ATTEMPT 2 ********

        //Debug.Log("Calling CanUnplace on Branch " + branchToUnplace);
        //bool canUnplace = false;
        //visitedBranches.Add(branchToUnplace);

        //if (GameObject.FindObjectOfType<BoardManager>().allBranches[branchToUnplace].GetComponent<BranchInfo>().isSetupBranch)
        //{
        //    Debug.Log(branchToUnplace + " isSetupBranch");
        //    canUnplace = true;
        //}
        //else
        //{
        //    // Examine every branch connected to the branch you want to unplace
        //    Relationships.connectionsRoad.TryGetValue(branchToUnplace, out List<int> connectedBranches);

        //    foreach (int i in connectedBranches)
        //    {
        //        Debug.Log("Check branch neighbor " + i);
        //        // If you own the neighboring branch
        //        Debug.Log(i + " owned = " + GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches.Contains(i));
        //        Debug.Log(i + " visited = " + visitedBranches.Contains(i));
        //        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches.Contains(i) && !visitedBranches.Contains(i))
        //        {
        //            Debug.Log("Branch " + i + " owned and not visited");
        //            if (!GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().placementConfirmed)
        //            {
        //                Debug.Log("Branch " + i + " placed on current turn");
        //                canUnplace = CanUnplace(i);
        //                Debug.Log("CanUnplace = " + canUnplace);
        //                if (!canUnplace)
        //                { break; }
        //            }
        //            else
        //            {
        //                Debug.Log("Branch " + i + " placement Confirmed");

        //                canUnplace = true;
        //                //break;
        //            }
        //        }
        //    }
        //}

        //return canUnplace;


        // ******** ATTEMPT 1 ***********

        //Debug.Log("Calling CanUnplace on Branch " + branchToUnplace);

        //bool canUnplace = false;

        //if (GameObject.FindObjectOfType<BoardManager>().allBranches[branchToUnplace].GetComponent<BranchInfo>().isSetupBranch)
        //{
        //    canUnplace = true;
        //}
        //else
        //{
        //    visitedBranches.Add(branchToUnplace);
        //    // Examine every branch connected to the branch you want to unplace
        //    Relationships.connectionsRoad.TryGetValue(branchToUnplace, out List<int> connectedBranches);
        //    foreach (int i in connectedBranches)
        //    {
        //        Debug.Log("Examining Immediate Neighbor Branch " + i);
        //        // If you own the neighboring branch but it is not a setup branch
        //        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches.Contains(i) &&
        //            !GameObject.FindObjectOfType<BoardManager>().allBranches[i].GetComponent<BranchInfo>().isSetupBranch)
        //        {
        //            if (IsConnected(i))
        //            {
        //                Debug.Log("Immediate Neighbor Branch " + i + " is connected to a Setup Branch");
        //                canUnplace = true;
        //                break;
        //            }
        //            else
        //                Debug.Log("Immediate Neighbor Branch " + i + " is NOT connected to a Setup Branch");
        //        }
        //    }

        //    visitedBranches.Clear();
        //}

        //return canUnplace;
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
                if (GameObject.FindObjectOfType<BoardManager>().LegalBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
                {
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
            if (GameObject.FindObjectOfType<BoardManager>().LegalBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
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
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches))
            {
                this.GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(156, 167, 176, 255);
            }
        }
    }
}
