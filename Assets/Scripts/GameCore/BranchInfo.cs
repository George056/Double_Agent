using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    private int cc;
    public enum Owner
    {
        US = 0,
        USSR = 1,
        Nil = 2
    }

    public string id;
    public Owner branchOwner;
    public int branchOrder;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
            this.gameObject.GetComponent<BranchInfo>().branchOwner = Owner.USSR;
            cc = this.gameObject.GetComponent<BranchInfo>().branchOrder;
            Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOrder);
            Debug.Log(this.gameObject.GetComponent<BranchInfo>().branchOwner);
            GameObject.FindObjectOfType<BoardManager>().ChangeBranchOwner(cc);
        }
    }
}
