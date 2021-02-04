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
    /*List<Node> nodes = new List<Node>(24);
    List<Branch> branches = new List<Branch>(36);
    List<short> max_nodes = new List<short>(13);

    Board(List<Color> cl, List<short> max_value)
    {

    }*/


    List<Tile> __tiles;

    public bool LegalMoveNode(char location)
    {
        int _tile_ = (int)location / 4;
        int _node_ = (int)location % 4;

        Owner owner =__tiles[_tile_].CheckNodeColor(_node_);

        return owner == Owner.Nil;
    }

    public bool LegalMoveConnector(int location)
    {
        int _tile_ = location / 4;
        int _side_ = location % 4;

        Owner owner = __tiles[_tile_].CheckConnectorColor(_side_);

        return owner == Owner.Nil;
    }
}
