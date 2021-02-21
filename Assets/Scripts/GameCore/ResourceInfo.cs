using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInfo : MonoBehaviour
{
    public enum Color {Red, Blue, Yellow, Green, Empty }

    public Color nodeColor;
    public int numOfResource;
    public Owner resoureTileOwner;

    [HideInInspector]
    [Tooltip("States if the tile is depleted")]
    public bool depleted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
