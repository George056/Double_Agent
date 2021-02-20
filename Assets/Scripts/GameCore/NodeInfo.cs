using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NodeInfo : MonoBehaviour
{

    private int cc;
    public BoardManager.Owner nodeOwner;
    public int nodeOrder;
    public List<ResourceInfo.Color> resources = new List<ResourceInfo.Color>(4) { ResourceInfo.Color.Empty, ResourceInfo.Color.Empty, ResourceInfo.Color.Empty, ResourceInfo.Color.Empty };
    //public string countryAbreviation;
    //public string countryName;

    
    void OnMouseDown()
    {
        
        if (GameObject.FindObjectOfType<BoardManager>().inBuildMode) // && localPlayer.CanAffordNode()
        {
            if (GameObject.FindObjectOfType<BoardManager>().LegalNodeMove(nodeOrder, GameObject.FindObjectOfType<BoardManager>().activeSide)) // localPlayer.Owner)) // or could use GameObject.FindObjectOfType<BoardManager>().activeSide for 2nd parameter
            {
                // do all the work to place the node

                if (GameObject.FindObjectOfType<BoardManager>().activeSide == BoardManager.Owner.US)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
                    this.gameObject.GetComponent<NodeInfo>().nodeOwner = BoardManager.Owner.US;
                }
                else if (GameObject.FindObjectOfType<BoardManager>().activeSide == BoardManager.Owner.USSR)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
                    this.gameObject.GetComponent<NodeInfo>().nodeOwner = BoardManager.Owner.USSR;
                }

                cc = this.gameObject.GetComponent<NodeInfo>().nodeOrder;
                Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOrder);
                Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOwner);
                GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(cc);

                //GameObject.FindObjectWithTag("Player").PayForNode();
            }
        }
        
    }
}
