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

public class Tile : MonoBehaviour
{
    public Color __color;
    public short __max_nodes;

    private short __placed_nodes;

    private Owner __captured;

    private Owner[] __nodes = new Owner[4];
    private Owner[] __sides = new Owner[4];

    Tile()
    {
        __color = Color.gray;
        __max_nodes = 0;
        __placed_nodes = 0;

        __captured = Owner.Nil;

        for (int i = 0; i < __nodes.Length; i++)
        {
            __nodes[i] = Owner.Nil;
        }

        for (int i = 0; i < __sides.Length; i++)
        {
            __sides[i] = Owner.Nil;
        }
    }

    Tile(Color c, short node_count)
    {
        __color = c;
        __max_nodes = node_count;
        __placed_nodes = 0;

        __captured = Owner.Nil;
        
        for(int i = 0; i < __nodes.Length; i++)
        {
            __nodes[i] = Owner.Nil;
        }

        for (int i = 0; i < __sides.Length; i++)
        {
            __sides[i] = Owner.Nil;
        }
    }

    Owner IsCaptured()
    {
        if(__sides[0] == __sides[1] && __sides[0] == __sides[2] && __sides[0] == __sides[3] && __sides[0] != Owner.Nil)
        {
            __captured = __sides[0];
        }
        return __captured;
    }

    void CaptureNode(Owner who, short position)
    {
        if (position >= 0 && position <= 3)
        {
            Owner temp = __nodes[position];
            __nodes[position] = (__nodes[position] == Owner.Nil) ? who : __nodes[position];
            if (temp != __nodes[position]) __placed_nodes++;
        }
    }

    void CaptureBranch(Owner who, short position)
    {
        if (position >= 0 && position <= 3)
        {
            __sides[position] = (__sides[position] == Owner.Nil) ? who : __sides[position];
        }
    }

    short GetNodesOwnedBy(Owner who)
    {
        short result = 0;
        for(int i = 0; i < __nodes.Length; i++)
        {
            if (__nodes[i] == who) result++;
        }
        return result;
    }

    /*
    This gives a resource of gray if none are allocated.
    */
    Color[] GiveResources(Owner who)
    {
        Color[] result = new Color[4] { Color.gray, Color.gray, Color.gray, Color.gray };

        if(__max_nodes > __placed_nodes)
        {
            for (int i = 0; i < GetNodesOwnedBy(who); i++)
            {
                result[i] = __color;
            }
        }
        else
        {
            if(__captured == who)
            {
                for(int i = 0; i < GetNodesOwnedBy(who); i++)
                {
                    result[i] = __color;
                }
            }
        }

        return result;
    }
}
