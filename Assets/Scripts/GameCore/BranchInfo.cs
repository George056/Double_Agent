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
        Debug.Log("Branch Owner: " + GameObject.FindObjectOfType<BoardManager>().allBranches[branchOrder].GetComponent<BranchInfo>().branchOwner);

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
