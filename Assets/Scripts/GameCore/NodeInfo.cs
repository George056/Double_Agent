using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NodeInfo : MonoBehaviour
{
    private int cc;
    public BoardManager.Owner nodeOwner;
    public int nodeOrder;
    //public string countryAbreviation;
    //public string countryName;

    void OnMouseDown()
    {
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
            this.gameObject.GetComponent<NodeInfo>().nodeOwner = BoardManager.Owner.USSR;
            cc = this.gameObject.GetComponent<NodeInfo>().nodeOrder;
            Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOrder);
            Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOwner);
            GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(cc);
            // update resource count accordingly
        }
    }
}
