using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    // DELETE: private int cc;
    public Owner branchOwner;
    public int branchOrder;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
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
