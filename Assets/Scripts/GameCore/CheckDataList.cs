using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    
    //Exhausted
    public GameObject BlueExhausted;
    public GameObject GreenExhausted;
    public GameObject RedExhausted;
    public GameObject YellowExhausted;

    /*
    //Captured
    public GameObject BlueCapturedByOrange;
    public GameObject BlueCapturedByPurple;
    public GameObject GreenCapturedByOrange;
    public GameObject GreenCapturedByPurple;
    public GameObject RedCapturedByOrange;
    public GameObject RedCapturedByPurple;
    public GameObject YellowCapturedByOrange;
    public GameObject YellowCapturedByPurple;
    */
    public GameObject OrangeCaptured;
    public GameObject PurpleCaptured;


    [HideInInspector]
    public ResourceInfo.Color currentColor;
    [HideInInspector]
    public Owner longestNetOwner = Owner.Nil;

    private BoardManager BM;
    private int maxResource;
    private int count = 0;
    //private int existCheck;
    private ArrayList UsedNode = new ArrayList();
    private int longestNetValue;

    List<int> visitedTiles = new List<int>();
    List<int> tilesToVisit = new List<int>(13) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

    public void DepltedResource()
    {

        for (int i = 0; i < 24; i++)
        {
            //Debug.Log(BM.nodeList[i].GetComponent<NodeInfo>().nodeOwner);
            if (BM.nodes[i].GetComponent<NodeInfo>().nodeOwner != Owner.Nil)
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
        GetBoardManager();
    }

    private void GetBoardManager()
    {
        BM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BoardManager>();
    }

    /// <summary>
    /// Check to see if any tiles are depleted
    /// </summary>
    public void DepletedCheck()
    {
        if (BM == null) GetBoardManager();
        for (int i = 0; i < BM.resourceList.Length; i++)
        {
            var tileInfo = BM.resourceList[i].GetComponent<ResourceInfo>();
            int count = 0; //the number of nodes connected to the tile
            foreach(var nd in BM.nodes)
            {
                if (tileInfo.resoureTileOwner != Owner.Nil) break; //cannot be depleted if owned
                if (tileInfo.depleted) break; //skip if already depleted

                var nodeInfo = nd.GetComponent<NodeInfo>();
                if (nodeInfo.nodeOwner != Owner.Nil)
                {
                    count += (nodeInfo.resources.Contains(BM.resourceList[i])) ? 1 : 0;
                    if(count > tileInfo.numOfResource)
                    {
                        tileInfo.depleted = true;
                        int x = BM.ResourceInfoList[i].xLoc;
                        int y = BM.ResourceInfoList[i].yLoc;
                        currentColor = tileInfo.nodeColor;
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
                        break;
                    }
                }
            }
        }
    }

    public void CapturedCheck()
    {
        for (int i = 0; i < BM.resourceList.Length; i++)
        {
            List<int> tempNum;
            Owner[] tempBranchOwner = new Owner[4];
            Relationships.connectionsTilesRoads.TryGetValue(i, out tempNum);
            for (int j = 0; j < tempNum.Count; j++)
            {
                tempBranchOwner[j] = BM.allBranches[tempNum[j]].GetComponent<BranchInfo>().branchOwner;
            }
            if (tempBranchOwner[0] != Owner.Nil && tempBranchOwner[0] == tempBranchOwner[1] && tempBranchOwner[1] == tempBranchOwner[2]
                && tempBranchOwner[2] == tempBranchOwner[3])
            {
                BM.resourceList[i].GetComponent<ResourceInfo>().resoureTileOwner = tempBranchOwner[0];
                int x = BM.ResourceInfoList[i].xLoc;
                int y = BM.ResourceInfoList[i].yLoc;
                if (tempBranchOwner[0] == (Owner)PlayerPrefs.GetInt("Human_Piece", 0))
                {
                    GameObject instance = GameObject.Instantiate((PlayerPrefs.GetInt("Human_Player", 0) == 0) ? OrangeCaptured : PurpleCaptured, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else
                {
                    GameObject instance = GameObject.Instantiate((PlayerPrefs.GetInt("Human_Player", 0) == 0) ? PurpleCaptured : OrangeCaptured, new Vector3(x, y, 0f), Quaternion.identity);
                }
            }
        }
    }

    public void MulticaptureCheck(Owner who)
    {
        for (int i = 0; i < tilesToVisit.Count; i++)
        {
            if (Multicaptured(tilesToVisit[i]))
            {
                Debug.Log("Multicaptured tiles: ");
                // update every visited tile's owner
                foreach (int capturedTile in visitedTiles)
                {
                    Debug.Log(capturedTile);
                    BM.resourceList[capturedTile].GetComponent<ResourceInfo>().resoureTileOwner = who;
                    BM.resourceList[capturedTile].GetComponent<ResourceInfo>().depleted = false;

                    // Update UI
                    int x = BM.ResourceInfoList[capturedTile].xLoc;
                    int y = BM.ResourceInfoList[capturedTile].yLoc;

                    if (who == Owner.US)
                    {
                        GameObject instance = GameObject.Instantiate(PurpleCaptured, new Vector3(x, y, 0f), Quaternion.identity);
                    }
                    else if (who == Owner.USSR)
                    {
                        GameObject instance = GameObject.Instantiate(OrangeCaptured, new Vector3(x, y, 0f), Quaternion.identity);
                    }

                    tilesToVisit.Remove(capturedTile);
                }

            }
            visitedTiles.Clear();
        }
    }

    bool Multicaptured(int currentTile)
    {
        bool isCaptured = true;

        List<int> connectedTiles;
        List<int> connectedBranches;
        Relationships.connectionsTilesRoads.TryGetValue(currentTile, out connectedBranches);
        
        foreach (int branch in connectedBranches)
        {
            Relationships.connectionsRoadTiles.TryGetValue(branch, out connectedTiles);

            if (BM.allBranches[branch].GetComponent<BranchInfo>().branchOwner == BM.activeSide)
            {
                isCaptured = false;
                break;
            }
            else if (BM.allBranches[branch].GetComponent<BranchInfo>().branchOwner == Owner.Nil && connectedTiles.Count == 1)
            {
                isCaptured = false;
                break;
            }
            else if (BM.allBranches[branch].GetComponent<BranchInfo>().branchOwner == Owner.Nil)
            {
                int newTile = 0;
                foreach (int tile in connectedTiles)
                {
                    if (tile != currentTile)
                        newTile = tile;
                }

                if (!visitedTiles.Contains(newTile))
                {
                    if (!visitedTiles.Contains(currentTile))
                    {
                        visitedTiles.Add(currentTile);
                    }

                    isCaptured = Multicaptured(newTile);
                }
                if (!isCaptured)
                    break;
            }
        }

        if (!visitedTiles.Contains(currentTile))
        {
            visitedTiles.Add(currentTile);
        }

        return isCaptured;
    }

    public void LongestNetCheck(Owner who)
    {
        List<int> branches = new List<int>();

        if (BM == null) GetBoardManager();

        foreach(GameObject go in BM.allBranches)
        {
            if(go.GetComponent<BranchInfo>().branchOwner == who)
            {
                branches.Add(go.GetComponent<BranchInfo>().branchOrder);
            }
        }

        List<int> longest = new List<int>();
        List<int> count = new List<int>();
        List<int> old = new List<int>();//store values already checked

        int itr = 0;//total branches visited counter

        if (branches.Count == 0) return;

        do
        {
            if(longest.Count == 0)//first time
            {
                longest.Add(branches[0]);
            }
            else
            {
                old.AddRange(longest);
                List<int> temp = branches;
                foreach(int i in old)
                {
                    temp.Remove(i);
                }
                longest = new List<int>();
                longest.Add(temp[0]);
            }

            for (int i = 0; i < longest.Count; i++)
            {
                List<int> temp;
                Relationships.connectionsRoad.TryGetValue(longest[i], out temp);
                for (int j = 0; j < temp.Count; j++)
                {
                    if (branches.Contains(temp[j]))
                    {
                        longest.Add(temp[j]);
                    }
                }
                longest = longest.Distinct().ToList();
            }
            count.Add(longest.Count);
            foreach(int i in count)
            {
                itr += i;
            }
        } while (itr <= Mathf.FloorToInt(branches.Count / 2.0f));

        int max = 0;
        foreach(int i in count)
        {
            if(i > max)
            {
                max = i;
            }
        }

        if(max > longestNetValue)
        {
            longestNetValue = max;
            longestNetOwner = who;
        }
        else if(longestNetOwner == who)
        {
            //do nothing
        }
        else if(max == longestNetValue)
        {
            longestNetOwner = Owner.Nil;
        }
    }
}
