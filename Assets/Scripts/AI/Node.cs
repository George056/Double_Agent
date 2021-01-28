using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color
{
    red,
    blue,
    yellow,
    green,
    gray   //this is used to denote a depleted square
}

public enum Owner
{
    US,
    USSR,
    Nil
}

public class Node : MonoBehaviour
{
    int number_of_sqrs;
    Owner owner;
    List<Color> colors;

    Node(int number_of_sqrs, Owner owner, List<Color> cl)
    {
        this.number_of_sqrs = number_of_sqrs;
        this.owner = owner;
        colors = new List<Color>(number_of_sqrs);
        foreach(Color c in cl)
        {
            colors.Add(c);
        }
    }

    Node(int number_of_sqrs, List<Color> cl)
    {
        this.number_of_sqrs = number_of_sqrs;
        this.owner = Owner.Nil;
        colors = new List<Color>(number_of_sqrs);
        foreach (Color c in cl)
        {
            colors.Add(c);
        }
    }

    void UpdateColor(Color c, int position)
    {
        colors[position] = c;
    }

    void UpdateOwner(Owner o)
    {
        owner = o;
    }
}
