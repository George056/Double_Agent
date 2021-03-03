using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInfo : MonoBehaviour
{
    // TO-DO: change to "Type" and { Copper, Lumber, Loyalist, Coin, Empty }
    public enum Color {Red, Blue, Yellow, Green, Empty }

    // TO-DO: change nodeColor to "resourceType"
    public Color nodeColor;
    public int numOfResource;
    public Owner resoureTileOwner;

    [Tooltip("States if the tile is depleted")]
    public bool depleted = false;
}
