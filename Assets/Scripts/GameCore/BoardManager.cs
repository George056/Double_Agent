using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

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
    public bool playerTraded = false;

    [HideInInspector]
    public int columns = 11;
    [HideInInspector]
    public int rows = 11;

    bool isSetupTurn = true;
    private int turnCount = 1;

    private CheckDataList cdl;

    [Tooltip("What piece is the AI")]
    private Owner aiPiece;
    [Tooltip("What piece is the human")]
    private Owner humanPiece;
    [Tooltip("What piece is the first player")]
    private Owner firstPlayer;

    private GameObject player1;
    private GameObject player2;

    private bool end;
    public GameObject gameOverWindow;


    /*
     * X = empty; N = node; H = heng branch; S = shu branch; R = resource
     */
    char[,] GameBoard = new char[11, 11]
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


    public TextMeshProUGUI playerCopperCount;
    public TextMeshProUGUI playerLumberCount;
    public TextMeshProUGUI playerLoyalistCount;
    public TextMeshProUGUI playerCoinCount;

    public TextMeshProUGUI opponentCopperCount;
    public TextMeshProUGUI opponentLumberCount;
    public TextMeshProUGUI opponentLoyalistCount;
    public TextMeshProUGUI opponentCoinCount;

    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI opponentScore;

    public GameObject USImage;
    public GameObject USSRImage;
    public GameObject USMusic;
    public GameObject USSRMusic;

    private string customBoardSeed;
    public GameObject[] tempResourceList = new GameObject[13];

    public Button[] resourceOutButtons = new Button[4];

    public List<int> nodesPlacedThisTurn = new List<int>();
    public List<int> branchesPlacedThisTurn = new List<int>();

    int GetTileIndex(string code)
    {
        int index = 0;

        switch (code)
        {
            case "B1":
                index = 0;
                break;
            case "B2":
                index = 1;
                break;
            case "B3":
                index = 2;
                break;
            case "ES":
                index = 3;
                break;
            case "R1":
                index = 4;
                break;
            case "R2":
                index = 5;
                break;
            case "R3":
                index = 6;
                break;
            case "Y1":
                index = 7;
                break;
            case "Y2":
                index = 8;
                break;
            case "Y3":
                index = 9;
                break;
            case "G1":
                index = 10;
                break;
            case "G2":
                index = 11;
                break;
            case "G3":
                index = 12;
                break;
        }

        return index;
    }

    void CustomizeBoardLayout()
    {
        for (int i = 0; i < 13; i++)
        {
            tempResourceList[i] = resourceList[i];
        }

        for (int i = 0; i < 13; i++)
        {
            string tileCode = customBoardSeed[i * 2].ToString() + customBoardSeed[i * 2 + 1].ToString();
            resourceList[i] = tempResourceList[GetTileIndex(tileCode)];
        }
    }
    
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
                        resourceList[resourceCount].GetComponent<ResourceInfo>().depleted = false;
                        resourceList[resourceCount].GetComponent<ResourceInfo>().resoureTileOwner = Owner.Nil;
                        resourceCount++;
                        break;
                    case 'N':
                        instance = Instantiate(nodes[nodeCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOwner = Owner.Nil;
                        nodes[nodeCount].GetComponent<NodeInfo>().nodeOrder = nodeCount;
                        nodes[nodeCount].GetComponent<NodeInfo>().placementConfirmed = false;
                        nodeCount++;
                        break;
                    case 'H':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        allBranches[branchCount].GetComponent<BranchInfo>().placementConfirmed = false;
                        //Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    case 'S':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        allBranches[branchCount].GetComponent<BranchInfo>().placementConfirmed = false;
                        //Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    default:
                        //Debug.Log(Map[x,y]);
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

            nodes[i].GetComponent<NodeInfo>().resources.Clear();
            nodes[i].GetComponent<NodeInfo>().resources.Capacity = temp.Count;

            for (int j = 0; j < temp.Count; j++)
            {
                nodes[i].GetComponent<NodeInfo>().resources.Add(resourceList[temp[j]]);
            }

        }
    }

    public void SetupScene()
    {
        customBoardSeed = PlayerPrefs.GetString("CustomBoardSeed", "");
        Debug.Log("CustomBoardSeed: " + customBoardSeed);
        gridPositions.Clear();
        if (customBoardSeed != "")
        {
            CustomizeBoardLayout();
        }
        else
        {
            Shuffle(resourceList);
        }
        BoardSetUp(GameBoard);
        AssignNodeResources();
        cdl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckDataList>();
        aiPiece = (Owner)PlayerPrefs.GetInt("AI_Piece", 1);
        humanPiece = (Owner)PlayerPrefs.GetInt("Human_Piece", 0);
        firstPlayer = (PlayerPrefs.GetInt("AI_Player", 1) == 0) ? aiPiece : humanPiece;
        activeSide = firstPlayer;

        end = false;
        turnCount = 1;

        //check to see if it is an AI or network game
        player1 = GameObject.FindGameObjectWithTag("Player");
        player2 = GameObject.FindGameObjectWithTag("AI");
        player2.GetComponent<AI>().SetOpener();

        if (humanPiece == Owner.US)
        {
            USImage.SetActive(true);
            USSRImage.SetActive(false);

            USMusic.SetActive(true);
            USSRMusic.SetActive(false);
        }
        else
        {
            USImage.SetActive(false);
            USSRImage.SetActive(true);

            USSRMusic.SetActive(true);
            USMusic.SetActive(false);

        }

        //make sure it is an AI game first
        if (firstPlayer == aiPiece)
        {
            BtnToggle();
            player2.GetComponent<AI>().AIMove(turnCount);
        }
    }

    public void UpdatePlayerScoreInUI(int score)
    {
        playerScore.text = score.ToString();
    }

    public void UpdateOpponentScoreInUI(int score)
    {
        opponentScore.text = score.ToString();
    }

    public void UpdatePlayerResourcesInUI(List<int> resources)
    {
        if (turnCount > 4)
        {
            playerCopperCount.text = resources[0].ToString();
            playerLumberCount.text = resources[1].ToString();
            playerLoyalistCount.text = resources[2].ToString();
            playerCoinCount.text = resources[3].ToString();
        }

        // In trade window, display only resources of which the player currently has at least one
        for (int i = 0; i < 4; i++)
        {
            if (resources[i] == 0)
            {
                resourceOutButtons[i].gameObject.SetActive(false);
            }
            else
            {
                resourceOutButtons[i].gameObject.SetActive(true);
            }
        }
    }

    public void UpdateOpponentResourcesInUI(List<int> resources)
    {
        if (turnCount > 4)
        {
            opponentCopperCount.text = resources[0].ToString();
            opponentLumberCount.text = resources[1].ToString();
            opponentLoyalistCount.text = resources[2].ToString();
            opponentCoinCount.text = resources[3].ToString();
        }
    }

    

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

        nodesPlacedThisTurn.Add(nodeNum);

        if (activeSide == aiPiece)
        {
            Debug.Log("AI placed node " + nodeNum);
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

        branchesPlacedThisTurn.Add(branchNum);

        if (activeSide == aiPiece)
        {
            Debug.Log("AI placed branch " + branchNum);
        }
    }

    public void UnplaceNode(int nodeNum)
    {
        nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.Nil;
        GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(104, 118, 137, 255);

        nodesPlacedThisTurn.Remove(nodeNum);
    }

    public void UnplaceBranch(int branchNum)
    {
        allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
        GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(156, 167, 176, 255);

        branchesPlacedThisTurn.Remove(branchNum);
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
        List<int> connectedTiles;

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

            if(turnCount == 3 || turnCount == 4)
            {
                Relationships.connectionsRoadNode.TryGetValue(branch, out List<int> adjacentNodes);
                if (LegalNodeMove(adjacentNodes[0], activeSide, myBranches) || LegalNodeMove(adjacentNodes[1], activeSide, myBranches)) isLegal = true;
                else isLegal = false;
            }
        }

        Relationships.connectionsRoadTiles.TryGetValue(branch, out connectedTiles);
        foreach (int tile in connectedTiles)
        {
            if (resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != Owner.Nil && resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != activeSide)
                isLegal = false;
        }

        return isLegal;
    }

    public void Trade(List<int> resources, Owner who)
    {
        bool makeTrade = false;

        List<int> heldResources = (who == aiPiece) ? player2.GetComponent<AI>().__resources : player1.GetComponent<Player>().__resources;
        for(int i = 0; i < 4; i++)
        {
            if(resources[i] < 0)
            {
                if(heldResources[i] >= Math.Abs(resources[i]))
                {
                    makeTrade = true;
                }
                else
                {
                    makeTrade = false;
                    break;
                }
            }
            else
            {
                if(resources[i] < 2)
                {
                    makeTrade = true;
                }
                else
                {
                    makeTrade = false;
                    break;
                }
            }
        }

        if (makeTrade)
        {
            //Trade animation ***********************************************************************************************************************************************
            if(who == aiPiece)
            {
                player2.GetComponent<AI>().UpdateResources(resources);
                Debug.Log("AI Traded: " + resources[0] + " red, " + resources[1] + " blue, " + resources[2] + " yellow, " + resources[3] + " green");
            }
            else
                player1.GetComponent<Player>().UpdateResources(resources);
        }
    }

    /// <summary>
    /// Updates board info every turn.
    /// Captured tiles
    /// Checks who has the longest net, and updates the score.
    /// Marks a tile as depleted or captured.
    /// </summary>
    private void BoardCheck()
    {
        Owner who = (activeSide == Owner.US) ? Owner.USSR : Owner.US;

        cdl.DepletedCheck();
        //cdl.CapturedCheck();
        cdl.MulticaptureCheck(who);

        CalculateScore(who);

        if (player2.GetComponent<AI>().__ai_score >= 10 || player1.GetComponent<Player>().__human_score >= 10)
        {
            tradeButton.SetActive(false);
            buildButton.SetActive(false);
            endTurnButton.SetActive(false);

            if (player1.GetComponent<Player>().__human_score >= 10) player2.GetComponent<AI>().Loss();
            else player2.GetComponent<AI>().Win();

            end = true;
            gameOverWindow.SetActive(true);
        }
    }

    /// <summary>
    /// Calculates the score of the person who's turn just finished.
    /// This includes the longest network check. Both players are updated if the longest net changes hands.
    /// </summary>
    /// <param name="who">Who we are calculating the score of</param>
    private void CalculateScore(Owner who)
    {
        Owner oldLongest = cdl.longestNetOwner;
        cdl.LongestNetCheck(who);
        int score = 0;

        //longest net score
        if (cdl.longestNetOwner == who)
        {
            score = 2;
        }

        // tell player of a change in longest net holder
        if (cdl.longestNetOwner != oldLongest)
        {
            if (oldLongest == aiPiece && player2.GetComponent<AI>().trainingMode)
            {
                player2.GetComponent<AI>().LoseLongestNet();
            }
            else if(oldLongest == humanPiece)
            {
                player1.GetComponent<Player>().LoseLongestNet();
            }

            if (cdl.longestNetOwner == aiPiece && GameObject.FindGameObjectWithTag("AI").GetComponent<AI>().trainingMode)
            {
                player2.GetComponent<AI>().GetLongestNet();
            }
            else if(cdl.longestNetOwner == humanPiece)
            {
                player1.GetComponent<Player>().GetLongestNet();
            }
        }

        //node count score
        foreach (var nd in nodes)
        {
            if (nd.GetComponent<NodeInfo>().nodeOwner == who) score++;
        }

        //captured tiles score
        foreach (var tile in resourceList)
        {
            if (tile.GetComponent<ResourceInfo>().resoureTileOwner == who) score++;
        }

        if(who == aiPiece)
        {
            player2.GetComponent<AI>().UpdateScore(score, cdl.longestNetOwner == aiPiece && oldLongest != aiPiece, oldLongest == aiPiece && cdl.longestNetOwner != aiPiece);
        }
        else
        {
            player1.GetComponent<Player>().UpdateScore(score, cdl.longestNetOwner == aiPiece && oldLongest != aiPiece, oldLongest == aiPiece && cdl.longestNetOwner != aiPiece);

            //if an AI game
            player2.GetComponent<AI>().__human_score = score;
        }
    }

    /// <summary>
    /// Finds the proper allocation of resources for the current player (activeSide) and gives them it.
    /// </summary>
    private void AllocateResources()
    {
        List<int> allocation = new List<int>(4) { 0, 0, 0, 0 };

        foreach(var node in nodes)//look at every node
        {
            var temp = node.GetComponent<NodeInfo>();
            if(temp.nodeOwner == activeSide)//if the node is owned by the current player
            {
                foreach(var tile in temp.resources)//look at all the tiles the node touches
                {
                    var tile_temp = tile.GetComponent<ResourceInfo>();
                    if(tile_temp.resoureTileOwner != Owner.Nil)//if someone owns it
                    {
                        if(tile_temp.resoureTileOwner == activeSide && tile_temp.nodeColor != ResourceInfo.Color.Empty)//only allocate if it is owned by the player
                        {
                            allocation[(int)tile_temp.nodeColor] += 1;
                        }
                    }
                    else//if no one owns it
                    {
                        if (!tile_temp.depleted)
                        {
                            allocation[(int)tile_temp.nodeColor] += 1;
                        }
                    }
                }
            }
        }

        //assign the new resources
        if(activeSide == aiPiece)
        {
            player2.GetComponent<AI>().UpdateResources(allocation);
        }
        else
        {
            player1.GetComponent<Player>().UpdateResources(allocation);
        }
    }

    public void EnterBuildMode()
    {
        inBuildMode = true;
    }

    public void EndTurnButtonClicked()
    {
        if(turnCount == 4)
            isSetupTurn = false;

        if (end) return;

        if (isSetupTurn)
        {
            if (true) // if (E & CL have been placed)
            {
                // prompt player to confirm submission
                if (true) // user confirmed turn submission
                {
                    for (int i = 0; i < nodesPlacedThisTurn.Count; i++)
                    {
                        nodes[nodesPlacedThisTurn[i]].GetComponent<NodeInfo>().placementConfirmed = true;
                    }
                    nodesPlacedThisTurn.Clear();
                    for (int i = 0; i < branchesPlacedThisTurn.Count; i++)
                    {
                        allBranches[branchesPlacedThisTurn[i]].GetComponent<BranchInfo>().placementConfirmed = true;
                    }
                    branchesPlacedThisTurn.Clear();

                    EndTurn();
                    // provide player with indication that opponent is taking turn
                    if (turnCount != 3)
                        player2.GetComponent<AI>().AIMove(turnCount);
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

                for (int i = 0; i < nodesPlacedThisTurn.Count; i++)
                {
                    nodes[nodesPlacedThisTurn[i]].GetComponent<NodeInfo>().placementConfirmed = true;
                }
                nodesPlacedThisTurn.Clear();
                for (int i = 0; i < branchesPlacedThisTurn.Count; i++)
                {
                    allBranches[branchesPlacedThisTurn[i]].GetComponent<BranchInfo>().placementConfirmed = true;
                }
                branchesPlacedThisTurn.Clear();

                EndTurn();
                // provide player with indication that opponent is taking turn
                player2.GetComponent<AI>().AIMove(turnCount);

            }
        }
    }

    private void BtnToggle()
    {
        tradeButton.SetActive(!tradeButton.activeSelf);
        buildButton.SetActive(!buildButton.activeSelf);
        endTurnButton.SetActive(!endTurnButton.activeSelf);
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

        turnCount++;

        // Perform GameBoard Check - check for depleted / captured squares, longest network, and update scores
        BoardCheck();

        if (end) return;

        // if it is time for the player's second setup move, allocate resources for one branch and one node
        if (turnCount == 3)
        {
            player1.GetComponent<Player>().UpdateResources(new List<int>(4) { 1, 1, 2, 2 });
        }

        if (turnCount == 5)
        {
            tradeButton.GetComponent<Button>().interactable = true;
        }

        // if not opener move, allocate resources to appropriate player
        if (turnCount >= 5)
            AllocateResources();

        inBuildMode = false;
        playerTraded = false;


        // disable/reenable Trade, Build, and End Turn buttons
        if (turnCount != 3)
        {
            BtnToggle();
        }
        else
        {
            if (firstPlayer == humanPiece)
            {
                player2.GetComponent<AI>().AIMove(turnCount);
            }
            else
            {
                //human move
            }
        }
        
        if (turnCount == 5)
        {
            player2.GetComponent<AI>().EndOpener();
        }
    }
}
