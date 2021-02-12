using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NodeInfo : MonoBehaviour
{
    private int cc;
    public enum Owner
    {
        US = 0,
        USSR = 1,
        Nil = 2
    }

    public Owner nodeOwner;
    public int nodeOrder;
    //public string countryAbreviation;
    //public string countryName;

    void OnMouseDown()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 100);
        this.gameObject.GetComponent<NodeInfo>().nodeOwner = Owner.USSR;
        cc = this.gameObject.GetComponent<NodeInfo>().nodeOrder;
        Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOrder);
        Debug.Log(this.gameObject.GetComponent<NodeInfo>().nodeOwner);
        GameObject.FindObjectOfType<BoardManager>().ChangeNodeOwner(cc);

    }
}
