using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    public enum Owner
    {
        US = 0,
        USSR = 1,
        Nil = 2
    }

    public Owner nodeOwner;
}
