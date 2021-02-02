using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    Owner owner;

    Branch(Owner o)
    {
        owner = o;
    }

    Branch()
    {
        owner = Owner.Nil;
    }

    void UpdateOwner(Owner o)
    {
        owner = o;
    }
}
