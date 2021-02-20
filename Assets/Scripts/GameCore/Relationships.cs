using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationships : MonoBehaviour
{
    [HideInInspector]
    [Tooltip("The different connections between the roads")]
    public static Dictionary<int, List<int>> connectionsRoad = new Dictionary<int, List<int>>();

    [HideInInspector]
    [Tooltip("The different connections to a node")]
    public static Dictionary<int, List<int>> connectionsNode = new Dictionary<int, List<int>>();

    [HideInInspector]
    [Tooltip("The different nodes a connector touches")]
    public static Dictionary<int, List<char>> connectionsRoadNode = new Dictionary<int, List<char>>();

    [HideInInspector]
    [Tooltip("How nodes connect to tiles")]
    public static Dictionary<int, List<int>> connectionsNodeTiles = new Dictionary<int, List<int>>();

    /// <summary>
    /// This sets up the two connection maps (dictionaries)
    /// </summary>
    public static void SetUpConnections()
    {
        connectionsRoad.Add(0, new List<int>() { 1, 2 });
        connectionsRoad.Add(1, new List<int>() { 0, 3, 4, 7 });
        connectionsRoad.Add(2, new List<int>() { 0, 4, 5, 8 });
        connectionsRoad.Add(3, new List<int>() { 1, 4, 6, 7 });
        connectionsRoad.Add(4, new List<int>() { 1, 2, 3, 5, 7, 8 });
        connectionsRoad.Add(5, new List<int>() { 2, 4, 8, 9 });
        connectionsRoad.Add(6, new List<int>() { 3, 10, 11, 16 });
        connectionsRoad.Add(7, new List<int>() { 1, 3, 4, 11, 12, 17 });
        connectionsRoad.Add(8, new List<int>() { 2, 4, 5, 12, 13, 18 });
        connectionsRoad.Add(9, new List<int>() { 5, 13, 14, 19 });
        connectionsRoad.Add(10, new List<int>() { 6, 15, 16 });
        connectionsRoad.Add(11, new List<int>() { 6, 7, 10, 12, 16, 17 });
        connectionsRoad.Add(12, new List<int>() { 7, 8, 11, 13, 17, 18 });
        connectionsRoad.Add(13, new List<int>() { 8, 9, 12, 14, 18, 19 });
        connectionsRoad.Add(14, new List<int>() { 9, 13, 19, 20 });
        connectionsRoad.Add(15, new List<int>() { 10, 21 });
        connectionsRoad.Add(16, new List<int>() { 6, 10, 11, 21, 22, 26 });
        connectionsRoad.Add(17, new List<int>() { 7, 11, 12, 22, 23, 27 });
        connectionsRoad.Add(18, new List<int>() { 8, 12, 13, 23, 24, 28 });
        connectionsRoad.Add(19, new List<int>() { 9, 13, 14, 24, 25, 29 });
        connectionsRoad.Add(20, new List<int>() { 14, 25 });
        connectionsRoad.Add(21, new List<int>() { 15, 16, 22, 26 });
        connectionsRoad.Add(22, new List<int>() { 16, 17, 21, 23, 26, 27 });
        connectionsRoad.Add(23, new List<int>() { 17, 18, 22, 24, 27, 28 });
        connectionsRoad.Add(24, new List<int>() { 18, 19, 23, 25, 28, 29 });
        connectionsRoad.Add(25, new List<int>() { 19, 20, 24, 29 });
        connectionsRoad.Add(26, new List<int>() { 21, 22, 30 });
        connectionsRoad.Add(27, new List<int>() { 17, 22, 23, 30, 31, 33 });
        connectionsRoad.Add(28, new List<int>() { 18, 23, 24, 31, 32, 34 });
        connectionsRoad.Add(29, new List<int>() { 19, 24, 25, 32 });
        connectionsRoad.Add(30, new List<int>() { 26, 27, 31, 33 });
        connectionsRoad.Add(31, new List<int>() { 27, 28, 30, 32, 33, 34 });
        connectionsRoad.Add(32, new List<int>() { 28, 29, 31, 34 });
        connectionsRoad.Add(33, new List<int>() { 27, 30, 21, 35 });
        connectionsRoad.Add(34, new List<int>() { 28, 31, 32, 35 });
        connectionsRoad.Add(35, new List<int>() { 33, 34 });

        connectionsNode.Add(0, new List<int>() { 0, 1 });
        connectionsNode.Add(1, new List<int>() { 0, 2 });
        connectionsNode.Add(2, new List<int>() { 3, 6 });
        connectionsNode.Add(3, new List<int>() { 1, 3, 4, 7 });
        connectionsNode.Add(4, new List<int>() { 2, 4, 5, 8 });
        connectionsNode.Add(5, new List<int>() { 5, 9 });
        connectionsNode.Add(6, new List<int>() { 10, 15 });
        connectionsNode.Add(7, new List<int>() { 6, 10, 11, 16 });
        connectionsNode.Add(8, new List<int>() { 7, 11, 12, 17 });
        connectionsNode.Add(9, new List<int>() { 9, 12, 13, 18 });
        connectionsNode.Add(10, new List<int>() { 9, 13, 14, 19 });
        connectionsNode.Add(11, new List<int>() { 14, 20 });
        connectionsNode.Add(12, new List<int>() { 15, 21 });
        connectionsNode.Add(13, new List<int>() { 16, 21, 22, 26 });
        connectionsNode.Add(14, new List<int>() { 17, 22, 23, 27 });
        connectionsNode.Add(15, new List<int>() { 18, 23, 24, 28 });
        connectionsNode.Add(16, new List<int>() { 19, 24, 25, 29 });
        connectionsNode.Add(17, new List<int>() { 20, 25 });
        connectionsNode.Add(18, new List<int>() { 26, 30 });
        connectionsNode.Add(19, new List<int>() { 27, 30, 31, 33 });
        connectionsNode.Add(20, new List<int>() { 28, 31, 32, 34 });
        connectionsNode.Add(21, new List<int>() { 29, 32 });
        connectionsNode.Add(22, new List<int>() { 33, 35 });
        connectionsNode.Add(23, new List<int>() { 34, 35 });

        connectionsRoadNode.Add(0, new List<char>() { 'a', 'b' });
        connectionsRoadNode.Add(1, new List<char>() { 'a', 'd' });
        connectionsRoadNode.Add(2, new List<char>() { 'b', 'e' });
        connectionsRoadNode.Add(3, new List<char>() { 'c', 'd' });
        connectionsRoadNode.Add(4, new List<char>() { 'd', 'e' });
        connectionsRoadNode.Add(5, new List<char>() { 'e', 'f' });
        connectionsRoadNode.Add(6, new List<char>() { 'c', 'h' });
        connectionsRoadNode.Add(7, new List<char>() { 'd', 'i' });
        connectionsRoadNode.Add(8, new List<char>() { 'e', 'j' });
        connectionsRoadNode.Add(9, new List<char>() { 'f', 'k' });
        connectionsRoadNode.Add(10, new List<char>() { 'g', 'h' });
        connectionsRoadNode.Add(11, new List<char>() { 'h', 'i' });
        connectionsRoadNode.Add(12, new List<char>() { 'i', 'j' });
        connectionsRoadNode.Add(13, new List<char>() { 'j', 'k' });
        connectionsRoadNode.Add(14, new List<char>() { 'k', 'l' });
        connectionsRoadNode.Add(15, new List<char>() { 'g', 'm' });
        connectionsRoadNode.Add(16, new List<char>() { 'h', 'n' });
        connectionsRoadNode.Add(17, new List<char>() { 'i', 'o' });
        connectionsRoadNode.Add(18, new List<char>() { 'j', 'p' });
        connectionsRoadNode.Add(19, new List<char>() { 'k', 'q' });
        connectionsRoadNode.Add(20, new List<char>() { 'l', 'r' });
        connectionsRoadNode.Add(21, new List<char>() { 'm', 'n' });
        connectionsRoadNode.Add(22, new List<char>() { 'n', 'o' });
        connectionsRoadNode.Add(23, new List<char>() { 'o', 'p' });
        connectionsRoadNode.Add(24, new List<char>() { 'p', 'q' });
        connectionsRoadNode.Add(25, new List<char>() { 'q', 'r' });
        connectionsRoadNode.Add(26, new List<char>() { 'n', 's' });
        connectionsRoadNode.Add(27, new List<char>() { 'o', 't' });
        connectionsRoadNode.Add(28, new List<char>() { 'p', 'u' });
        connectionsRoadNode.Add(29, new List<char>() { 'q', 'v' });
        connectionsRoadNode.Add(30, new List<char>() { 's', 't' });
        connectionsRoadNode.Add(31, new List<char>() { 't', 'u' });
        connectionsRoadNode.Add(32, new List<char>() { 'u', 'v' });
        connectionsRoadNode.Add(33, new List<char>() { 't', 'w' });
        connectionsRoadNode.Add(34, new List<char>() { 'u', 'x' });
        connectionsRoadNode.Add(35, new List<char>() { 'w', 'x' });

        connectionsNodeTiles.Add(0, new List<int>() { 0 });
        connectionsNodeTiles.Add(1, new List<int>() { 0 });
        connectionsNodeTiles.Add(2, new List<int>() { 1 });
        connectionsNodeTiles.Add(3, new List<int>() { 0, 1, 2 });
        connectionsNodeTiles.Add(4, new List<int>() { 0, 2, 3 });
        connectionsNodeTiles.Add(5, new List<int>() { 3 });
        connectionsNodeTiles.Add(6, new List<int>() { 4 });
        connectionsNodeTiles.Add(7, new List<int>() { 1, 4, 5 });
        connectionsNodeTiles.Add(8, new List<int>() { 1, 2, 5, 6 });
        connectionsNodeTiles.Add(9, new List<int>() { 2, 3, 6, 7 });
        connectionsNodeTiles.Add(10, new List<int>() { 3, 7, 8 });
        connectionsNodeTiles.Add(11, new List<int>() { 8 });
        connectionsNodeTiles.Add(12, new List<int>() { 4 });
        connectionsNodeTiles.Add(13, new List<int>() { 4, 5, 9 });
        connectionsNodeTiles.Add(14, new List<int>() { 5, 6, 9, 10 });
        connectionsNodeTiles.Add(15, new List<int>() { 6, 7, 10, 11 });
        connectionsNodeTiles.Add(16, new List<int>() { 7, 8, 11 });
        connectionsNodeTiles.Add(17, new List<int>() { 8 });
        connectionsNodeTiles.Add(18, new List<int>() { 9 });
        connectionsNodeTiles.Add(19, new List<int>() { 9, 10, 12 });
        connectionsNodeTiles.Add(20, new List<int>() { 10, 11, 12 });
        connectionsNodeTiles.Add(21, new List<int>() { 11 });
        connectionsNodeTiles.Add(22, new List<int>() { 12 });
        connectionsNodeTiles.Add(23, new List<int>() { 12 });

    }
}