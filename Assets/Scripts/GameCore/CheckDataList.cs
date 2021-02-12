using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDataList : MonoBehaviour
{
    public int[,] nodeAndResourceCheck = new int[13, 4]
   {
        {0, 1, 3, 4},
        {2, 3, 7, 8},
        {3, 4, 8, 9},
        {4, 5, 9, 10},
        {6, 7, 12, 13},
        {7, 8, 13, 14},
        {8, 9, 14, 15},
        {9, 10, 15, 16},
        {10, 11, 16, 17},
        {13, 14, 18, 19},
        {14, 15, 19, 20},
        {15, 16, 20, 21},
        {19, 20, 22, 23}
   };
    public GameObject BlueExhausted;
    public GameObject GreenExhausted;
    public GameObject RedExhausted;
    public GameObject YellowExhausted;
    private BoardManager BM;
    private int maxResource;
    private int count = 0;
    //private int existCheck;
    private ArrayList UsedNode = new ArrayList();
    private ResourceInfo.Color currentColor;

    public void DepltedResource()
    {

        for (int i = 0; i < 24; i++)
        {
            //Debug.Log(BM.nodeList[i].GetComponent<NodeInfo>().nodeOwner);
            if (BM.nodes[i].GetComponent<NodeInfo>().nodeOwner != NodeInfo.Owner.Nil)
            {
                UsedNode.Add(i);
            }
        }

        Debug.Log(UsedNode.Count);

        for (int i = 0; i < 13; i++)
        {
            maxResource = BM.ResourceInfoList[i].nodeNum;
            currentColor = BM.ResourceInfoList[i].nodeColor;
            for (int j = 0; j < 4; j++)
            {
                if (UsedNode.Contains(nodeAndResourceCheck[i, j]))
                {
                    count++;
                }
            }

            if (count > maxResource)
            {
                int x = BM.ResourceInfoList[i].xLoc;
                int y = BM.ResourceInfoList[i].yLoc;
                if (currentColor == ResourceInfo.Color.Blue)
                {
                    GameObject instance = GameObject.Instantiate(BlueExhausted, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (currentColor == ResourceInfo.Color.Green)
                {
                    GameObject instance = GameObject.Instantiate(GreenExhausted, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (currentColor == ResourceInfo.Color.Red)
                {
                    GameObject instance = GameObject.Instantiate(RedExhausted, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (currentColor == ResourceInfo.Color.Yellow)
                {
                    GameObject instance = GameObject.Instantiate(YellowExhausted, new Vector3(x, y, 0f), Quaternion.identity);
                }


            }

            count = 0;
        }

        UsedNode.Clear();

    }
    // Start is called before the first frame update
    void Awake()
    {
        BM = GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
