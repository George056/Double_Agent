using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Position is viewed from the node and goes clockwise starting in the top_left. For a node conected to 4 squares this would be top_left, top_right, bottom_right, and bottom_left.
You start at the smallest value that you have, in that list, and that is index 0 in a position list.

Nodes are counted from the top most square's top_left node and go from left to right and top to bottom.

Branches as counted from the top most one and go from left to right and top to bottom.
 */

public class Board : MonoBehaviour
{
    readonly List<char> OUTER_NODES = new List<char>() { 'a', 'b', 'c', 'f', 'g', 'l', 'm', 'r', 's', 'v', 'w', 'x' };
    readonly List<int> OUTER_SIDES = new List<int>() { 0, 1, 2, 3, 5, 6, 9, 10, 14, 15, 20, 21, 25, 26, 29, 30, 32, 33, 34, 35 };

    List<Tile> __tiles;

    public void MakeRandBoard()
    {

    }

    public void MakeGivenBoard()
    {

    }

    /// <summary>
    /// This function checks to see if it is posible to place a node there
    /// </summary>
    /// <param name="location">The location to place the node</param>
    /// <returns>If it is legal to place a node (true) or not (false)</returns>
    public bool LegalMoveNode(char location)
    {
        LocationSpliterNode(location, out int _tile_, out int _node_);

        Owner owner =__tiles[_tile_].CheckNodeColor(_node_);

        return owner == Owner.Nil;
    }

    /// <summary>
    /// This function checks to see if it is posible to place a connector there
    /// </summary>
    /// <param name="location">The location to place the connector</param>
    /// <returns>If it is legal to place a connector (true) or not (false)</returns>
    public bool LegalMoveConnector(int location)
    {
        int _tile_ = location / 4;
        int _side_ = location % 4;

        Owner owner = __tiles[_tile_].CheckConnectorColor(_side_);

        return owner == Owner.Nil;
    }

    public void PlaceNode(Owner who, char location)
    {
        LocationSpliterNode(location, out int _tile_, out int _node_);

        //the edge nodes
        if (OUTER_NODES.Contains(location))
        {
            __tiles[_tile_].CaptureNode(who, (short)_node_);
        }//**************************************************************************************************************************************
        //***************************************************************************************************************************************
        //***************************************************************************************************************************************
        //***************************************************************************************************************************************
    }

    public void PlaceConnector(int location)
    {

    }

    /// <summary>
    /// This finds the first occurrence of a node
    /// </summary>
    /// <param name="location">The location of the node</param>
    /// <param name="_tile_">The tile the node is on (out)</param>
    /// <param name="_node_">The node of the tile it is on (out)</param>
    private void LocationSpliterNode(char location, out int _tile_, out int _node_)
    {
        if(location == 'a' || location == 'b' || location == 'd' || location == 'e')
        {
            _tile_ = 0;
            _node_ = (location == 'a') ? 0 : (location == 'b') ? 1 : (location == 'd') ? 2 : 3;
        }
        else if(location == 'c' || location == 'h' || location == 'i')
        {
            _tile_ = 1;
            _node_ = (location == 'c') ? 0 : (location == 'h') ? 2 : 3;
        }
        else if(location == 'j')
        {
            _tile_ = 2;
            _node_ = 3;
        }
        else if(location == 'f' || location == 'k')
        {
            _tile_ = 3;
            _node_ = (location == 'f') ? 1 : 3;
        }
        else if(location == 'g' || location == 'm' || location == 'n')
        {
            _tile_ = 4;
            _node_ = (location == 'g') ? 0 : (location == 'm') ? 2 : 3;
        }
        else if(location == 'o')
        {
            _tile_ = 5;
            _node_ = 3;
        }
        else if(location == 'p')
        {
            _tile_ = 6;
            _node_ = 3;
        }
        else if(location == 'q')
        {
            _tile_ = 7;
            _node_ = 3;
        }
        else if(location == 'l' || location == 'r')
        {
            _tile_ = 8;
            _node_ = (location == 'l') ? 1 : 3;
        }
        else if(location == 's' || location == 't')
        {
            _tile_ = 9;
            _node_ = (location == 's') ? 2 : 3;
        }
        else if(location == 'u')
        {
            _tile_ = 10;
            _node_ = 3;
        }
        else if(location == 'v')
        {
            _tile_ = 11;
            _node_ = 3;
        }
        else // location == 'w' || location == 'x'
        {
            _tile_ = 12;
            _node_ = (location == 'w') ? 2 : 3;
        }
    }

    private void OtherTilesWithSameNode(char location, int tile, int node, out List<int> tiles, out List<int> node_pos)
    {
        if(OUTER_NODES.Contains(location))//no other locations
        {
            tiles = new List<int>();
            node_pos = new List<int>();
        }
        else
        {
            if(location == 'i' || location == 'j' || location == 'o' || location == 'p')//part of 4 tiles
            {
                tiles = new List<int>(3);
                node_pos = new List<int>(3);
            }
            else//part of 3 tiles
            {
                tiles = new List<int>(2);
                node_pos = new List<int>(2);
            }
        }

    }
}
