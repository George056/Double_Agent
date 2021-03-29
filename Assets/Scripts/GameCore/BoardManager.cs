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
    public GameObject endTurnButton;

    public bool inBuildMode;
    public bool playerTraded = false;

    [HideInInspector]
    public int columns = 11;
    [HideInInspector]
    public int rows = 11;

    bool isSetupTurn = true;
    [HideInInspector]
    public int turnCount = 1;

    private CheckDataList cdl;

    [Tooltip("What piece is the AI")]
    private Owner aiPiece;
    [Tooltip("What piece is the human")]
    private Owner humanPiece;
    [Tooltip("What piece is the first player")]
    private Owner firstPlayer;
    [Tooltip("What piece is the networks")]
    private Owner netPiece;


   
    private GameObject player1;
    private GameObject player2;

    [HideInInspector]
    public static bool end;

    public GameObject gameOverUSWin;
    public GameObject gameOverUSLoss;
    public GameObject gameOverUSSRWin;
    public GameObject gameOverUSSRLoss;

    public GameObject netPlayer;
    public GameObject gameOverWindow;
    public GameObject SetupLegalPopup;

    private static NetworkController networkController = new NetworkController();
    private bool netWorkTurn = true;


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

    public GameObject firstSetupNodeImage;
    public GameObject secondSetupNodeImage;
    public GameObject firstSetupBranchImage;
    public GameObject secondSetupBranchImage;

    public GameObject USImage;
    public GameObject USSRImage;
    public GameObject USMusic;
    public GameObject USSRMusic;
    public float defaultVolume = 0.5f;
    public Slider musicSlider;

    [HideInInspector]
    public static bool new_board = true;

    private string customBoardSeed;
    public GameObject[] tempResourceList = new GameObject[13];

    public Button[] resourceOutButtons = new Button[4];

    public List<int> nodesPlacedThisTurn = new List<int>();
    public List<int> branchesPlacedThisTurn = new List<int>();

    public int firstSetupBranch = -1;
    public int secondSetupBranch = -1;

    public int firstSetupNode = -1;
    public int secondSetupNode = -1;

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
                        nodes[nodeCount].GetComponent<NodeInfo>().isSetupNode = false;
                        nodeCount++;
                        break;
                    case 'H':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        allBranches[branchCount].GetComponent<BranchInfo>().placementConfirmed = false;
                        allBranches[branchCount].GetComponent<BranchInfo>().isSetupBranch = false;
                        //Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    case 'S':
                        instance = Instantiate(allBranches[branchCount], new Vector3(hang + 6 * x, lie + 6 * y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
                        allBranches[branchCount].GetComponent<BranchInfo>().branchOrder = branchCount;
                        allBranches[branchCount].GetComponent<BranchInfo>().placementConfirmed = false;
                        allBranches[branchCount].GetComponent<BranchInfo>().isSetupBranch = false;
                        //Debug.Log(allBranches[branchCount].GetComponent<BranchInfo>().branchOwner);
                        branchCount++;
                        break;
                    default:
                        //Debug.Log(Map[x,y]);
                        break;
                }
            }
        }
        Debug.Log("BoardSetUp complete");
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
        if (PlayerPrefs.GetString("GameType", "") == "net")
        {
            Debug.Log("Network game SetupScene");

            netPiece = (Owner)PlayerPrefs.GetInt("Network_Piece", 0);
            gridPositions.Clear();

            CustomizeBoardLayout();
            BoardSetUp(GameBoard);
            AssignNodeResources();
            cdl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckDataList>();

            inBuildMode = true;
            firstPlayer = netPiece;
            activeSide = firstPlayer;
            end = false;
            turnCount = 1;

            USMusic.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
            USSRMusic.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);

            netPlayer = GameObject.FindGameObjectWithTag("Player");

            if (netPiece == Owner.US)
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

            NetworkGame();
        }
        else
        {
            Debug.Log("Local game SetupScene");
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

            inBuildMode = true;
            end = false;
            turnCount = 1;
            USMusic.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
            USSRMusic.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);

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

        if (activeSide == humanPiece && isSetupTurn)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().isSetupNode = true;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupNode = nodeNum;
                firstSetupNodeImage.SetActive(false);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupNode = nodeNum;
                secondSetupNodeImage.SetActive(false);
            }
        }

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

        if (activeSide == humanPiece && isSetupTurn)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().isSetupBranch = true;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupBranch = branchNum;
                firstSetupBranchImage.SetActive(false);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupBranch = branchNum;
                secondSetupBranchImage.SetActive(false);
            }
        }

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

        if (activeSide == humanPiece && isSetupTurn)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().isSetupNode = false;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupNode = -1;
                firstSetupNodeImage.SetActive(true);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupNode = -1;
                secondSetupNodeImage.SetActive(true);
            }
        }
    }

    public void UnplaceBranch(int branchNum)
    {
        allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
        GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(156, 167, 176, 255);

        if (activeSide == humanPiece && isSetupTurn)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().isSetupBranch = false;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupBranch = -1;
                firstSetupBranchImage.SetActive(true);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupBranch = -1;
                secondSetupBranchImage.SetActive(true);
            }
        }

        branchesPlacedThisTurn.Remove(branchNum);
    }

    public bool LegalUINodeMove(int node, Owner activeSide, List<int> myBranches)
    {
        bool isLegal = true;

        if (nodes[node].GetComponent<NodeInfo>().nodeOwner != Owner.Nil) { isLegal = false; }
        
        if (turnCount < 5)
        {
            // a node on a setup move must have an available adjacent branch that can also be claimed
            bool availableBranchFound = false;
            Relationships.connectionsNode.TryGetValue(node, out List<int> adjacentBranches);
            foreach (int i in adjacentBranches)
            {
                if (allBranches[i].GetComponent<BranchInfo>().branchOwner == Owner.Nil)
                    availableBranchFound = true;
            }
            if (!availableBranchFound) { isLegal = false; }
        }
        else
        {
            Relationships.connectionsNode.TryGetValue(node, out List<int> connectedBranches);
            bool found = false;
            foreach (int i in connectedBranches)
            {
                found = myBranches.Contains(i);
                if (found) break;
            }

            if (!found) { isLegal = false; }
        }

        return isLegal;
    }

    public bool LegalUIBranchMove(int branch, Owner activeSide, List<int> myBranches)
    {
        bool isLegal = true;
        List<int> connectedTiles;

        if (allBranches[branch].GetComponent<BranchInfo>().branchOwner != Owner.Nil) { isLegal = false; }

        //*************************** THIS DOES NOT WORK YET (the if statement) *************************************************** 
        if (turnCount < 5)
        {
            Relationships.connectionsRoadNode.TryGetValue(branch, out List<int> adjacentNodes);
            if (turnCount == 1 || turnCount == 2)
            {
                if (adjacentNodes[0] != firstSetupNode && adjacentNodes[1] != firstSetupNode) { isLegal = false; }
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                if (adjacentNodes[0] != secondSetupNode && adjacentNodes[1] != secondSetupNode) { isLegal = false; }
            }
        }
        else
        {
            // check all the branches connected to the one you want to place
            Relationships.connectionsRoad.TryGetValue(branch, out List<int> connectedBranches);
            bool found = false;
            foreach (int i in connectedBranches)
            {
                // set found to true if any of the connected branches is in your list of owned branches
                found = myBranches.Contains(i);
                if (found) break;
            }

            if (!found) { isLegal = false; }

            // a branch cannot be placed inside a multicaptured square owned by opponent
            Relationships.connectionsRoadTiles.TryGetValue(branch, out connectedTiles);
            foreach (int tile in connectedTiles)
            {
                if (resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != Owner.Nil && resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != activeSide)
                    isLegal = false;
            }
        }

        return isLegal;
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

        // if this is not a Setup Move
        if (myBranches.Count >= 2)
        {
            // check all the branches connected to the one you want to place
            Relationships.connectionsRoad.TryGetValue(branch, out List<int> connectedBranches);
            bool found = false;
            foreach (int i in connectedBranches)
            {
                // set found to true if any of the connected branches is in your list of owned branches
                found = myBranches.Contains(i);
                if (found) break;
            }

            if (!found) { isLegal = false; }

            // a branch on a setup move must have an available adjacent node that can also be claimed
            if(turnCount == 3 || turnCount == 4)
            {
                Relationships.connectionsRoadNode.TryGetValue(branch, out List<int> adjacentNodes);
                if (LegalNodeMove(adjacentNodes[0], activeSide, myBranches) || LegalNodeMove(adjacentNodes[1], activeSide, myBranches)) isLegal = true;
                else isLegal = false;
            }
        }

        // a branch cannot be placed inside a multicaptured square owned by opponent
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

        int traded_for = 0, traded_in = 0;

        List<int> heldResources = (who == aiPiece) ? player2.GetComponent<AI>().__resources : player1.GetComponent<Player>().__resources;
        for(int i = 0; i < 4; i++)
        {
            if(resources[i] < 0)
            {
                if(heldResources[i] >= Math.Abs(resources[i]))
                {
                    makeTrade = true;
                    traded_in += resources[i];
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
                    traded_for += resources[i];
                }
                else
                {
                    makeTrade = false;
                    break;
                }
            }
        }

        if (makeTrade && (traded_for != 1 || Math.Abs(traded_in) != 3)) makeTrade = false;

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

    public void UIEndGame()
    {
        if (humanPiece == Owner.US)
        {
            if (player1.GetComponent<Player>().__human_score >= 10) { gameOverUSWin.SetActive(true); }
            else { gameOverUSLoss.SetActive(true); }
        }
        else
        {
            if (player1.GetComponent<Player>().__human_score >= 10) { gameOverUSSRWin.SetActive(true); }
            else { gameOverUSSRLoss.SetActive(true); }
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
            //buildButton.SetActive(false);
            endTurnButton.SetActive(false);

            if (player1.GetComponent<Player>().__human_score >= 10) player2.GetComponent<AI>().Loss();
            else player2.GetComponent<AI>().Win();

            end = true;
            UIEndGame();
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

    //public void EnterBuildMode()
    //{
    //    inBuildMode = true;
    //}

    public void EndTurnButtonClicked()
    {
        if(turnCount > 4)
            isSetupTurn = false;

        if (end) return;
        
        if (isSetupTurn)
        {
            if (SetupturnlegalCheck()) // if (E & CL have been placed)
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
                SetupLegalPopup.SetActive(true);
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
    private bool SetupturnlegalCheck()
    {
        bool islegal = true;
        // player1.GetComponent<Player>().UpdateResources
        List<int> tempPlayerResources;
        for (int i = 0; i < player1.GetComponent<Player>().__resources.Count; i++)
        {
            if (player1.GetComponent<Player>().__resources[i] != 0)
            {
                islegal = false;
                break;
            }
        }
        return islegal;
    }
    private void BtnToggle()
    {
        tradeButton.SetActive(!tradeButton.activeSelf);
        //buildButton.SetActive(!buildButton.activeSelf);
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

            inBuildMode = !inBuildMode;
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

    public void NetworkGame()
    {
        //Host Turn
        if(activeSide == firstPlayer)
        {

        }

        else if(activeSide != firstPlayer)
        {
            if (netWorkTurn)
            {
                netWorkTurn = false;
            }
            else
            {
                netWorkTurn = true;
            }
            StartCoroutine(networkController.WaitForTurn());
        }
    }

    public void TurnReceived()
    {
        ReceiveMoveFromNetwork();
        NetworkGame();
    }

    public void ReceiveMoveFromNetwork()
    {
        List<int> networkNodes = networkController.GetNodesPlaced();
        List<int> networkBranches = networkController.GetBranchesPlaced();

        foreach(int node in networkNodes)
        {
            NetworkChangeNodeOwner(node);
        }
        foreach(int branch in networkBranches)
        {
            NetworkChangeBranchOwner(branch);
        }
    }

    public void setNetworkManagerReference()
    {
        networkController.SetBoardManagerReference(this);
    }

    public void NetworkChangeNodeOwner(int nodeNum)
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

        /*nodesPlacedThisTurn.Add(nodeNum);*/

        /*if (activeSide == humanPiece && isSetupTurn)
        {
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupNodeImage.SetActive(false);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupNodeImage.SetActive(false);
            }
        }

        if (activeSide == aiPiece)
        {
            Debug.Log("AI placed node " + nodeNum);
        }*/
    }

    public void NetworkChangeBranchOwner(int branchNum)
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

        /*branchesPlacedThisTurn.Add(branchNum);*/

       /* if (activeSide == netPiece && isSetupTurn)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().isSetupBranch = true;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupBranch = branchNum;
                firstSetupBranchImage.SetActive(false);
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupBranch = branchNum;
                secondSetupBranchImage.SetActive(false);
            }
        }

        if (activeSide != netPiece)
        {
            Debug.Log("Network placed branch " + branchNum);
        }*/
    }

    public string GetRandomBoardSeed()
    {
        string boardSeed = "";
        int randNum;
        string temp;
        List<string> boardSeedComponents = new List<string>() 
        {
            "Y1",
            "Y2",
            "Y3",
            "G1",
            "G2",
            "G3",
            "R1",
            "R2",
            "R3",
            "B1",
            "B2",
            "B3",
            "ES"
        };

        for(int i = 0; i < boardSeedComponents.Count; i++)
        {
            temp = boardSeedComponents[i];
            randNum = Random.Range(0, boardSeedComponents.Count);
            boardSeedComponents[i] = boardSeedComponents[randNum];
            boardSeedComponents[randNum] = temp;
        }

        for(int i = 0; i < boardSeedComponents.Count; i++)
        {
            boardSeed = boardSeed + boardSeedComponents[i];
        }

        Debug.Log("Board Seed: " + boardSeed);
        
        return boardSeed;
    }

    public void StartNetworkGame()
    {
        string role = PlayerPrefs.GetInt("Host").ToString();
        Debug.Log("Role: " + role);
        setNetworkManagerReference();
        if (PlayerPrefs.GetInt("Host") == 1)
        {
            customBoardSeed = GetRandomBoardSeed();
            networkController.SetBoardSeed(customBoardSeed);
            networkController.SendSeed();
            SetupScene();
        }
        else
        {
            StartCoroutine(networkController.WaitForSeed());
        }
    }

    public void ReceiveSeedFromNetwork()
    {
        customBoardSeed = networkController.GetBoardSeed();
        Debug.Log("Received Seed from Network: " + customBoardSeed);
        SetupScene();
    }
}


