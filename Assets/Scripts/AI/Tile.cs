using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The possible colors of a tile
/// </summary>
public enum Color
{
    red,
    blue,
    yellow,
    green,
    gray   //this is used to denote a depleted square
}

/// <summary>
/// The owner value for a node, branch, or tile, if not owned the value is Nil
/// </summary>
public enum Owner
{
    US,
    USSR,
    Nil
}

/// <summary>
/// Every aspect associated with a single tile.
/// This treats every tile seperatly.
/// </summary>
public class Tile : MonoBehaviour
{
    [Tooltip("The color of the tile")]
    public Color __color;
    [Tooltip("The max node count for the tile")]
    public short __max_nodes;
    [Tooltip("A flag that shows if the tile has been depleted")]
    public bool __depleted;
    [Tooltip("The current node count on the tile")]
    private short __placed_nodes;
    [Tooltip("The owner of the tile, if not owned it is said to be Nil")]
    private Owner __captured;
    [Tooltip("An array that holds who owns the nodes of this tile")]
    private Owner[] __nodes = new Owner[4];
    [Tooltip("An array that holds who owns the connectors of this tile")]
    private Owner[] __sides = new Owner[4];

    /// <summary>
    /// The default constructor for Tile
    /// </summary>
    Tile()
    {
        __color = Color.gray;
        __max_nodes = 0;
        __placed_nodes = 0;
        __depleted = false;

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

    /// <summary>
    /// A constructor of a tile
    /// </summary>
    /// <param name="c">The color of the tile</param>
    /// <param name="node_count">The max number of nodes the tile can hold before depletion</param>
    Tile(Color c, short node_count)
    {
        __color = c;
        __max_nodes = node_count;
        __placed_nodes = 0;
        __depleted = false;

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

    /// <summary>
    /// Set the color of the tile
    /// </summary>
    /// <param name="c">The color of the tile</param>
    void SetColor(Color c)
    {
        if (__color == Color.gray) __color = c;
    }

    /// <summary>
    /// Set the max number of nodes for the tile
    /// </summary>
    /// <param name="n">The max node count</param>
    void SetNodeCap(short n)
    {
        if (__max_nodes == 0) __max_nodes = n;
    }

    /// <summary>
    /// This allows for a tile to be captured
    /// </summary>
    /// <param name="who">The player that captured the tile(s)</param>
    void Capture(Owner who)
    {
        __captured = who;
    }

    /// <summary>
    /// A node is checked to see if it is empty if so it is captured and __placed_nodes incraments
    /// </summary>
    /// <param name="who">The player that placed the node</param>
    /// <param name="position">The location of the node on the tile</param>
    void CaptureNode(Owner who, short position)
    {
        if (position >= 0 && position <= 3)
        {
            Owner temp = __nodes[position];
            __nodes[position] = (__nodes[position] == Owner.Nil) ? who : __nodes[position];
            if (temp != __nodes[position]) __placed_nodes++;
            CheckToSeeIfDepleted();
        }
    }

    /// <summary>
    /// Sets the value of a connector
    /// </summary>
    /// <param name="who">The player that placed the connector</param>
    /// <param name="position">The location of the connector on the tile</param>
    void CaptureBranch(Owner who, short position)
    {
        if (position >= 0 && position <= 3)
        {
            __sides[position] = (__sides[position] == Owner.Nil) ? who : __sides[position];
        }
    }

    /// <summary>
    /// This checks to see if the tile is depleted and stores the result in the class variable __depleted
    /// </summary>
    void CheckToSeeIfDepleted()
    {
        if (__placed_nodes > __max_nodes && __captured == Owner.Nil) __depleted = true;
    }

    /// <summary>
    /// This returns the count of the number of nodes owned by who
    /// </summary>
    /// <param name="who">The player who's node count is being calculated</param>
    /// <returns>The number of nodes the player owns on the tile</returns>
    short GetNodesOwnedBy(Owner who)
    {
        short result = 0;
        for(int i = 0; i < __nodes.Length; i++)
        {
            if (__nodes[i] == who) result++;
        }
        return result;
    }

    /// <summary>
    /// This finds and allocates resources appropriately
    /// </summary>
    /// <param name="who">The owner we are allocating resources for</param>
    /// <returns>An array of length 4 that has the allocated resources (if not allocated returns gray)</returns>
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
