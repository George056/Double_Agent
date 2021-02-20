using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    private int cc;
    public BoardManager.Owner branchOwner;
    public int branchOrder;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode && GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CanAffordBranch())
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalBranchMove(branchOrder, GameObject.FindObjectOfType<BoardManager>().activeSide, 
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().__owned_branches)) // or localPlayer.Owner for 2nd parameter
            {
                if (GameObject.FindObjectOfType<BoardManager>().activeSide == BoardManager.Owner.US)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
                    branchOwner = BoardManager.Owner.US;
                }
                else if (GameObject.FindObjectOfType<BoardManager>().activeSide == BoardManager.Owner.USSR)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
                    branchOwner = BoardManager.Owner.USSR;
                }

                //cc = this.gameObject.GetComponent<BranchInfo>().branchOrder;
                Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOrder);
                Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOwner);
                //GameObject.FindObjectOfType<BoardManager>().ChangeBranchOwner(cc);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddBranch(branchOrder);

                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PayForBranch();
            }
        }
    }
}
