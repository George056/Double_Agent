using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    private int cc;
    public Owner branchOwner;
    public int branchOrder;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide, 
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches)) // or localPlayer.Owner for 2nd parameter
            {
                //Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOrder);
                //Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOwner);

                GameObject.FindObjectOfType<BoardManager>().ChangeBranchOwner(branchOrder);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddBranch(branchOrder);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForBranch();
            }
        }
    }
}
