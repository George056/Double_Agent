using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCheck : MonoBehaviour
{
    private Relationships RS;
    private BoardManager BM;

    // Start is called before the first frame update
    void Start()
    {
        RS = GetComponent<Relationships>();
        BM = GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
