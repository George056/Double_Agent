using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public enum Ally
    {
        US = 0,
        USSR = 1
    }

    public struct ResourceItemInfo
    {
        public ResourceInfo.Color nodeColor;
        public int nodeNum;
        //safe the location
        public int xLoc;
        public int yLoc;
    }
    public int columns = 11;
    public int rows = 11;
    public GameObject node;
    public GameObject hengBranch;
    public GameObject shuBranch;
    public GameObject[] resourceList;
    public GameObject[] nodes;
    public GameObject[] hengBranches;
    public GameObject[] shuBranches;
    public ResourceItemInfo[] ResourceInfoList = new ResourceItemInfo[13];
    bool isSetupTurn = true;
    public Ally currentPlayer;
    public bool inBuildMode = false;

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
    private int hengBranchCount = 0;
    private int shuBranchCount = 0;
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
        int hang = -27; // -40
        int lie = -25; // -30
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                switch(Map[x ,y])
                {
                    case 'R':
                        //get the resource's location, color, the number of resource
                        instance = Instantiate(resourceList[resourceCount], new Vector3(hang + 5 * x, lie + 5 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        ResourceInfoList[resourceCount].nodeColor = resourceList[resourceCount].GetComponent<ResourceInfo>().nodeColor;
                        ResourceInfoList[resourceCount].nodeNum = resourceList[resourceCount].GetComponent<ResourceInfo>().numOfResource;
                        ResourceInfoList[resourceCount].xLoc = hang + 5 * x;
                        ResourceInfoList[resourceCount].yLoc = lie + 5 * y;
                        resourceCount++;
                        break;
                    case 'N':
                        instance = Instantiate(nodes[nodeCount], new Vector3(hang + 5 * x, lie + 5 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder); 
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOwner = NodeInfo.Owner.Nil;
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOrder = nodeCount;
                        nodeCount++;
                        break;
                    case 'H':
                        instance = Instantiate(shuBranches[hengBranchCount], new Vector3(hang + 5 * x, lie + 5 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        hengBranchCount++;
                        break;
                    case 'S':
                        instance = Instantiate(hengBranches[shuBranchCount], new Vector3(hang + 5 * x, lie + 5 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        shuBranchCount++;
                        break;
                    default:
                        Debug.Log(Map[x,y]);
                        break;
                }
            }
        }
    }
   
    public void SetupScene()
    {
        gridPositions.Clear();
        Shuffle(resourceList);
        BoardSetUp(GameBoard);
    }
    /*
     *  change the owner of the node by clicking
     */
    public void ChangeNodeOwner(int nodeNum)
    {
        if (currentPlayer == Ally.US)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = NodeInfo.Owner.US;
        }
        else
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = NodeInfo.Owner.USSR;
        }
    }

    public void ChangeBranchOwner(int branchNum)
    {
        if (currentPlayer == Ally.US)
        {
            //nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = NodeInfo.Owner.US;
        }
        else
        {
            //nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = NodeInfo.Owner.USSR;
        }
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

    public void EndTurn()
    {
        Debug.Log("End Turn Button clicked");
        if (isSetupTurn)
        {
            if (true) // if (E & CL have been placed)
            {
                // prompt player to confirm submission
                if (true) // user confirmed turn submission
                {
                    Debug.Log("Player confirmed submission; turn ending");

                    if (currentPlayer == Ally.US)
                    {
                        currentPlayer = Ally.USSR;
                    }
                    else
                    {
                        currentPlayer = Ally.US;
                    }

                    inBuildMode = false;

                    // disable Trade, Build, and End Turn buttons

                    // Perform GameBoard Check

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
                // switch internal indication of whose turn it is

                // disable Trade, Build, and End Turn buttons

                // Perform GameBoard Check

                // provide player with indication that opponent is taking turn
            }
        }

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
