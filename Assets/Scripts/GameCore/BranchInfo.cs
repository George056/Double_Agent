using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    public Owner branchOwner;
    public int branchOrder;

    public bool placementConfirmed = false;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode)
        {
            if (GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().branchOwner == GameObject.FindObjectOfType<BoardManager>().activeSide &&
                !GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().placementConfirmed)
            {

                GameObject.FindObjectOfType<BoardManager>().UnplaceBranch(branchOrder);

                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().RemoveBranch(branchOrder);

                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ReimburseForBranch();

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
        }
    }
}
