using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// The owner value for a node, branch, or tile, if not owned the value is Nil
/// </summary>
public enum Owner
{
    US = 0,
    USSR = 1,
    Nil = 2
}

public class BoardManager : MonoBehaviour
{
    public struct ResourceItemInfo
    {
        public ResourceInfo.Color nodeColor;
        public int nodeNum;
        //safe the location
        public int xLoc;
        public int yLoc;
    }
    
    public GameObject[] resourceList;
    public GameObject[] nodes;
    public GameObject[] allBranches;

    public ResourceItemInfo[] ResourceInfoList = new ResourceItemInfo[13];
    public Owner activeSide;
    
    public GameObject tradeButton;
    public GameObject buildButton;
    public GameObject endTurnButton;
    
    public bool inBuildMode = false;
    
    [HideInInspector]
    public int columns = 11;
    [HideInInspector]
    public int rows = 11;

    bool isSetupTurn = false;
    private int turnCount = 1;

    private CheckDataList cdl;

    [Tooltip("What piece is the AI")]
    private Owner aiPiece;
    [Tooltip("What piece is the human")]
    private Owner humanPiece;
    [Tooltip("What piece is the first player")]
    private Owner firstPlayer;


    /*
     * X = empty; N = node; H = heng branch; S = shu branch; R = resource
     */
    char[,] GameBoard = new char[11,11]
    {
        {'X', 'X', 'X', 'X', 'N', 'H', 'N', 'X', 'X', 'X', 'X'},
        {'X', 'X', 'X', 'X', 'S', 'R', 'S', 'X', 'X', 'X', 'X'},
        {'X', 'X', 'N', 'H', 'N', 'H', 'N', 'H', 'N', 'X', 'X'},
        {'X', 'X', 'S', 'R', 'S', 'R', 'S', 'R', 'S', 'X', 'X'},
        {'N', 'H', 'N', 'H', 'N', 'H', 'N', 'H', 'N', 'H', 'N'},
        {'S', 'R', 'S', 'R', 'S', 'R', 'S', 'R', 'S', 'R', 'S'},
        {'N', 'H', 'N', 'H', 'N', 'H', 'N', 'H', 'N', 'H', 'N'},
        {'X', 'X', 'S', 'R', 'S', 'R', 'S', 'R', 'S', 'X', 'X'},
        {'X', 'X', 'N', 'H', 'N', 'H', 'N', 'H', 'N', 'X', 'X'},
        {'X', 'X', 'X', 'X', 'S', 'R', 'S', 'X', 'X', 'X', 'X'},
        {'X', 'X', 'X', 'X', 'N', 'H', 'N', 'X', 'X', 'X', 'X'},
    };

    private int resourceCount = 0;
    private int nodeCount = 0;
    private int branchCount = 0;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    private ResourceInfo.Color tempColor; //store the color
    private int tempNum;    //store
    
    void Shuffle(GameObject[] resourceList)
    {
        int randomNum;
        GameObject temp;
        for(int i = 0; i < resourceList.Length; i++)
        {
            temp = resourceList[i];
            randomNum = Random.Range(0, resourceList.Length);
            resourceList[i] = resourceList[randomNum];
            resourceList[randomNum] = temp;
        }
    }

 

    void BoardSetUp(char[,] Map)
    {
        boardHolder = new GameObject("Board").transform;
        GameObject instance;
        int hang = -32; // -40
        int lie = -32; // -30
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                switch(Map[x ,y])
                {
                    case 'R':
                        //get the resource's location, color, the number of resource
                        instance = Instantiate(resourceList[resourceCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        ResourceInfoList[resourceCount].nodeColor = resourceList[resourceCount].GetComponent<ResourceInfo>().nodeColor;
                        ResourceInfoList[resourceCount].nodeNum = resourceList[resourceCount].GetComponent<ResourceInfo>().numOfResource;
                        ResourceInfoList[resourceCount].xLoc = hang + 6 * x;
                        ResourceInfoList[resourceCount].yLoc = lie + 6 * y;
                        resourceCount++;
                        break;
                    case 'N':
                        instance = Instantiate(nodes[nodeCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOwner = Owner.Nil;
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOrder = nodeCount;
                        nodeCount++;
                        break;
                    case 'H':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    case 'S':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    default:
                        Debug.Log(Map[x,y]);
                        break;
                }
            }
        }
    }

    void AssignNodeResources()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Relationships.connectionsNodeTiles.TryGetValue(i, out List<int> temp);

            nodes[i].GetComponent<NodeInfo>().resources.Capacity = temp.Count;

            for (int j = 0; j < temp.Count; j++)
            {
                nodes[i].GetComponent<NodeInfo>().resources[j] = resourceList[temp[j]];
            }

        }
    }

    public void SetupScene()
    {
        gridPositions.Clear();
        Shuffle(resourceList);
        BoardSetUp(GameBoard);
        AssignNodeResources();
        cdl = new CheckDataList();
        aiPiece = (Owner)PlayerPrefs.GetInt("AI_Piece");
        humanPiece = (Owner)PlayerPrefs.GetInt("Human_Piece");
        firstPlayer = (PlayerPrefs.GetInt("AI_Player") == 0) ? aiPiece : humanPiece;
    }
    /*
     *  change the owner of the node by clicking
     */
    public void ChangeNodeOwner(int nodeNum)
    {
        if (activeSide == Owner.US)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
        }
        else
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
        }
    }

    public void ChangeBranchOwner(int branchNum)
    {
        if (activeSide == Owner.US)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
        }
        else
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
        }
    }

    public bool LegalNodeMove(int node, Owner activeSide, List<int> myBranches)
    {
        bool isLegal = true;

        if (nodes[node].GetComponent<NodeInfo>().nodeOwner != Owner.Nil) { isLegal = false; }

        Relationships.connectionsNode.TryGetValue(node, out List<int> connectedBranches);
        bool found = false;
        foreach (int i in connectedBranches)
        {
            found = myBranches.Contains(i);
            if (found) break;
        }

        if (!found) { isLegal = false; }

        return isLegal;
    }

    public bool LegalBranchMove(int branch, Owner activeSide, List<int> myBranches)
    {
        bool isLegal = true;
        bool ownsNeighborBranch = false;

        if (allBranches[branch].GetComponent<BranchInfo>().branchOwner != Owner.Nil) { isLegal = false; }

        if (myBranches.Count >= 2)
        {
            Relationships.connectionsRoad.TryGetValue(branch, out List<int> connectedBranches);
            bool found = false;
            foreach (int i in connectedBranches)
            {
                found = myBranches.Contains(i);
                if (found) break;
            }

            if (!found) { isLegal = false; }
        }

        // if (on a square multi-captured by opponent) { isLegal = false; }

        return isLegal;
    }

    /// <summary>
    /// Updates board info every turn.
    /// Checks who has the longest net, and updates the score.
    /// Marks a tile as depleted or captured.
    /// </summary>
    private void BoardCheck()
    {
        Owner oldLongest = cdl.longestNetOwner;
        cdl.LongestNetCheck((activeSide == Owner.US) ? Owner.USSR : Owner.US);

        cdl.DepletedCheck();
    }

    public void EnterBuildMode()
    {
        inBuildMode = true;

        // Highlight legal moves
        
        //foreach(GameObject node in nodes)
        //{
        //    // if not owned by anyone, highlight as possible move
        //    if (node.GetComponent<NodeInfo>().nodeOwner == NodeInfo.Owner.Nil)
        //    {
        //        Debug.Log("Unclaimed Node");
        //    }
        //}
    }

    public void EndTurnButtonClicked()
    {
        if (isSetupTurn)
        {
            if (true) // if (E & CL have been placed)
            {
                // prompt player to confirm submission
                if (true) // user confirmed turn submission
                {
                    Debug.Log("Player confirmed submission; turn ending");

                    if (turnCount != 2)
                    {
                        if (activeSide == Owner.US)
                        {
                            activeSide = Owner.USSR;
                        }
                        else
                        {
                            activeSide = Owner.US;
                        }
                    }

                    inBuildMode = false;

                    // disable Trade, Build, and End Turn buttons
                    //tradeButton.SetActive(!tradeButton.activeSelf);
                    //buildButton.SetActive(!buildButton.activeSelf);
                    //endTurnButton.SetActive(!endTurnButton.activeSelf);


                    // Perform GameBoard Check
                    BoardCheck();

                    // provide player with indication that opponent is taking turn
                }
            }
            else
            {

                // prompt player to place E & CL
            }
        }
        else // a regular turn
        {
            // prompt player to confirm submission
            if (true) // user confirmed turn submission
            {
                Debug.Log("Player confirmed submission; turn ending");

                if (activeSide == Owner.US)
                {
                    activeSide = Owner.USSR;
                }
                else
                {
                    activeSide = Owner.US;
                }

                inBuildMode = false;

                // disable Trade, Build, and End Turn buttons

                // Perform GameBoard Check

                // provide player with indication that opponent is taking turn
            }
        }
    }

    public void EndTurn()
    {
        if (turnCount != 2)
        {
            if (activeSide == Owner.US)
            {
                activeSide = Owner.USSR;
            }
            else
            {
                activeSide = Owner.US;
            }
        }

        // Perform GameBoard Check - check for depleted / captured squares, longest network, and update scores
        BoardCheck();

        // if not opener move, allocate resources to appropriate player

        // call AI to make move if AI turn
        // provide player with indication that opponent is taking turn

        inBuildMode = false;

        // disable/reenable Trade, Build, and End Turn buttons
        //tradeButton.SetActive(!tradeButton.activeSelf);
        //buildButton.SetActive(!buildButton.activeSelf);
        //endTurnButton.SetActive(!endTurnButton.activeSelf);

        turnCount++;
    }

    void FirstTurnSequence()
    {
        //Player1.embassy = 2;
        //Player1.commLine = 2;
        //Player2.embassy = 2;
        //Player2.commLine = 2;
        //Player1.MakeFirstMove();
        //Player2.MakeFirstMove();
        //Player2.MakeFirstMove();
        //Player1.MakeFirstMove();
    }

    void MakeFirstMove()
    {
        // pseudo code - Board, Trade, Build available to player
        //pieceSelected = piece;
        //while (pieceSelected != embassy)
        //{
        //    pieceSelected = null;
        //    //pseudo code - Error prompt or some indication that player must select an embassy
        //    pieceSelected = piece;
        //}
        //PlaceFirstEmbassy();
        //pieceSelected = piece;
        //while (pieceSelected != commLine)
        //{
        //    pieceSelected = null;
        //    //pseudo code - Error prompt or some indication that player must select a commLine
        //    pieceSelected = piece;
        //}
        //PlaceFirstCommLine();
        //if (submitButtonClicked())
        //{
        //    //endTurn(){ pseudo code - Board, Trade, Build UNavailable to player}
        //}
    }

    //EmptyNodeLocation doesn't have to be a array just a way to check if location is owned
    void PlaceFirstEmbassy()
    {
        //do
        //{
        //    location = clickedLocation(); //records the location of where the player wishes to place piece
        //    //while (location != EmptyNodeLocations[]) //Checks to see if location is already owned
        //    //{
        //    //    location = EmptyNodeLocations[]
        //    //} 
        //    location = pieceSelected; //piece is placed on the board
        //    embassy--;
        //    NodeInfo.Owner = //player side
        //    moveMade = true;
        //    //if( pseudo code - location is clicked again)
        //    {
        //        moveMade = false;
        //        embassy++;
        //    }
        //} while (moveMade = false);    
    }

    void PlaceFirstCommLine()
    {
        //do
        //{
        //    location = clickedLocation(); //records the location of where the player wishes to place piece
        //    //while (location != EmptyBranchLocations[]) //Checks to see if location is already owned
        //    //{
        //    //    location = clickedLocation();
        //    //} 
        //    location = pieceSelected; //piece is placed on the board
        //    commLine--;
        //    NodeInfo.Owner = //player side
        //    moveMade = true;
        //    //if ( pseudo code - location is clicked again)
        //    {
        //        moveMade = false;
        //        commLine++;
        //    }
        //} while (moveMade = false);    

    }
}
