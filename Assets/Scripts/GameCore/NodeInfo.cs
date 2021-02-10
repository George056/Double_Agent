using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeInfo : MonoBehaviour
{
    public enum Owner
    {
        US = 0,
        USSR = 1,
        Nil = 2
    }

    public Owner nodeOwner;
    public string countryAbreviation;
    public string countryName;
}
