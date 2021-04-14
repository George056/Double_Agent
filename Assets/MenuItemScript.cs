using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemScript : MonoBehaviour
{
    private Vector3 enlargedMenuItem = new Vector3(1.1f, 1.1f, 0);
    private Vector3 regularMenuItem = new Vector3(1f, 1f, 0);

    void OnMouseEnter()
    {
        Debug.Log("MouseEnter");
        this.transform.localScale = enlargedMenuItem;
    }

    void OnMouseExit()
    {
        Debug.Log("MouseExit");
        this.transform.localScale = regularMenuItem;
    }
}
