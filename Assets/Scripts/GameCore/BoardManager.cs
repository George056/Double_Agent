using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

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

    private IEnumerator coroutine;

    public GameObject[] resourceList;
    public GameObject[] nodes;
    public GameObject[] allBranches;

    public ResourceItemInfo[] ResourceInfoList = new ResourceItemInfo[13];
    public Owner activeSide;

    public GameObject tradeButton;
    public GameObject keyButton;
    public GameObject endTurnButton;
    public GameObject tradeWindow;
    public GameObject exchangeWindow;

    public GameObject longestNetworkUS;
    public GameObject longestNetworkUSSR;

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
    [HideInInspector] public Owner netPiece;


   
    private GameObject player1;
    private GameObject player2;

    [HideInInspector]
    public static bool end;

    public GameObject gameOverUSWin;
    public GameObject gameOverUSLoss;
    public GameObject gameOverUSSRWin;
    public GameObject gameOverUSSRLoss;
    public GameObject playerLeftPopup;
    public GameObject playerDisconnectedPopup;
    public GameObject exitConfirmationPopup;
    public GameObject settingsPopup;
  

    public GameObject localPlayer;
    public GameObject gameOverWindow;
    public GameObject SetupLegalPopup;

    public GameObject SetupTurnTelegram;
    public GameObject YourTurnTelegram;

    private static NetworkController networkController = new NetworkController();
   // private static NetworkPlayer networkPlayerClass = new NetworkPlayer();
    private bool netWorkTurn = true;
    public int calculateScoreCount = 1;
    public static BoardManager boardManager;


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

    public GameObject USNode;
    public GameObject USBranch;
    public GameObject USSRNode;
    public GameObject USSRBranch;

    public Sprite EmptyNodeSprite;
    public Sprite EmptyBranchSprite;
    public Sprite USNodeSprite;
    public Sprite USNodeHighlightedSprite;
    public Sprite USBranchSprite;
    public Sprite USBranchHighlightedSprite;
    public Sprite USSRNodeSprite;
    public Sprite USSRNodeHighlightedSprite;
    public Sprite USSRBranchSprite;
    public Sprite USSRBranchHighlightedSprite;


    public GameObject BlackoutPanel;
    public GameObject USImage;
    public GameObject USSRImage;

    public float defaultVolume = 0.5f;
    //public Slider musicSlider;

    public AudioSource USMusic;
    public AudioSource USSRMusic;
    public AudioSource USVictory;
    public AudioSource USLoss;
    public AudioSource USSRVictory;
    public AudioSource USSRLoss;

    public AudioSource lightSwitch;
    public AudioSource doorCreak;
    public AudioSource doorClose;
    public AudioSource footsteps;
    public AudioSource PiecePlaced;
    public AudioSource buttonClicked;

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

    public SceneController SC;
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

    private IEnumerator TurnOnLight(float delay)
    {
        Debug.Log("TurnOnLight called");
        
        //doorCreak.Play(0);
        //yield return new WaitForSeconds(.5f);
        doorClose.Play(0);
        yield return new WaitForSeconds(0.75f);
        footsteps.Play(0);
        yield return new WaitForSeconds(2f);
        lightSwitch.Play(0);

        Debug.Log("Sound effects played");

        if (GameInfo.game_type == "local")
        {
            if (humanPiece == Owner.US)
            {
                USMusic.Play(0);
            }
            yield return new WaitForSeconds(0.5f);
            BlackoutPanel.SetActive(false);

            if (humanPiece == Owner.USSR)
            {
                yield return new WaitForSeconds(0.5f);
                USSRMusic.Play(0);
            }
        }
        else if (GameInfo.game_type == "net")
        {
            if (netPiece == Owner.US)
            {
                USMusic.Play(0);
            }
            yield return new WaitForSeconds(0.5f);
            BlackoutPanel.SetActive(false);

            if (netPiece == Owner.USSR)
            {
                yield return new WaitForSeconds(0.5f);
                USSRMusic.Play(0);
            }
        }
    }
    public void TurnLightsOff()
    {
        coroutine =TurnOffLight(.5f);
        StartCoroutine(coroutine);
    }
    private IEnumerator TurnOffLight(float delay)
    {

        if (GameInfo.game_type == "local")
        {
            if (humanPiece == Owner.US)
            {
                USMusic.Pause();
            }
            else
            {
                USSRMusic.Pause();
            }
        }
        else if (GameInfo.game_type == "net")
        {
            playerDisconnectedPopup.gameObject.SetActive(false);
            playerLeftPopup.gameObject.SetActive(false);
            if (netPiece == Owner.US)
            {
                USMusic.Pause();
            }
            else
            {
                USSRMusic.Pause();
            }
        }
        lightSwitch.Play(0);
        yield return new WaitForSeconds(0.5f);
        BlackoutPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        footsteps.Play(0);
        yield return new WaitForSeconds(2f);
        doorCreak.Play(0);
        yield return new WaitForSeconds(1f);
        doorClose.Play(0);
        yield return new WaitForSeconds(1f);
        //Debug.Log("turning off the lights");
        if(GameInfo.game_type == "net")
        {
            PhotonNetwork.AutomaticallySyncScene = false;

            PhotonNetwork.LeaveRoom();

            PhotonNetwork.Disconnect();
        }
        SC.LoadScene("MainMenuScene");

    }

    public void SetVolume()
    {
        USMusic.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        USSRMusic.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        USVictory.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        USLoss.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        USSRVictory.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        USSRLoss.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);

        lightSwitch.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);
        doorCreak.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);
        doorClose.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);
        footsteps.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);
        PiecePlaced.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);
        buttonClicked.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", defaultVolume);

        Debug.Log("After setting volume: US Win volume = " + USVictory.volume);
        Debug.Log("After setting volume: US Loss volume = " + USLoss.volume);
        Debug.Log("After setting volume: USSR Win volume = " + USSRVictory.volume);
        Debug.Log("After setting volume: USSR Loss volume = " + USSRLoss.volume);

    }

    public void SetUSGame()
    {
        USImage.SetActive(true);
        USSRImage.SetActive(false);

        USNode.SetActive(true);
        USSRNode.SetActive(false);

        USBranch.SetActive(true);
        USSRBranch.SetActive(false);
    }

    public void SetUSSRGame()
    {
        USImage.SetActive(false);
        USSRImage.SetActive(true);

        USNode.SetActive(false);
        USSRNode.SetActive(true);

        USBranch.SetActive(false);
        USSRBranch.SetActive(true);
    }

    public void SetupScene()
    {
        SetVolume();

        GameInfo.first_game = false;

        Debug.Log("Game type: " + GameInfo.game_type); 

        Debug.Log("SetupSceneCalled");
        if (GameInfo.game_type == "net")
        {
            Debug.Log("Network game SetupScene");

            netPiece = (Owner)GameInfo.network_piece;
            Debug.Log("netPice = " + netPiece);
            gridPositions.Clear();

            CustomizeBoardLayout();
            Debug.Log("CustomizeBoardLayoutComplete");
            BoardSetUp(GameBoard);
            AssignNodeResources();
            cdl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckDataList>();

            // https://docs.unity3d.com/ScriptReference/Coroutine.html
            coroutine = TurnOnLight(1.5f);
            StartCoroutine(coroutine);


            if (GameInfo.host == true)
            {
                inBuildMode = true;
                firstPlayer = netPiece;
                activeSide = firstPlayer;
            }
            else
            {
                inBuildMode = false; 
                firstPlayer = (netPiece == Owner.US ? Owner.USSR : Owner.US);
                activeSide = firstPlayer;
                BtnToggle();
                SetupTurnTelegram.SetActive(false);
            }

            Debug.Log("Active Side = " + activeSide);

            end = false;
            turnCount = 1;


            localPlayer = GameObject.FindGameObjectWithTag("Player");

            if (netPiece == Owner.US)
            {
                SetUSGame();
            }
            else
            {
                SetUSSRGame();
            }

            NetworkGame();
        }
        else
        {
            customBoardSeed = GameInfo.custom_board_seed;
            Debug.Log("CustomBoardSeed: " + customBoardSeed);
            new_board = true;
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

            // https://docs.unity3d.com/ScriptReference/Coroutine.html
            coroutine = TurnOnLight(1.5f);
            StartCoroutine(coroutine);

            AssignNodeResources();
            cdl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CheckDataList>();
            aiPiece = (Owner)GameInfo.ai_piece;
            humanPiece = (Owner)GameInfo.human_piece;
            firstPlayer = (GameInfo.ai_player == 0) ? aiPiece : humanPiece;
            activeSide = firstPlayer;

            end = false;
            turnCount = 1;
            inBuildMode = (firstPlayer == humanPiece);

            //check to see if it is an AI or network game
            player1 = GameObject.FindGameObjectWithTag("Player");
            player2 = GameObject.FindGameObjectWithTag("AI");
            player2.GetComponent<AI>().SetOpener();

            if (humanPiece == Owner.US)
            {
                SetUSGame();
            }
            else
            {
                SetUSSRGame();
            }

            //make sure it is an AI game first
            if (firstPlayer == aiPiece)
            {
                BtnToggle();
                player2.GetComponent<AI>().AIMove(turnCount);
            }
        }
    }

 

    #region GameCore

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
        if (GameInfo.game_type == "net")
        {
            if (turnCount > 4)
            {
                playerCopperCount.text = resources[0].ToString();
                playerLumberCount.text = resources[1].ToString();
                playerLoyalistCount.text = resources[2].ToString();
                playerCoinCount.text = resources[3].ToString();
            }
        }
        else if (GameInfo.game_type == "local")
        {
            if (turnCount > 4)
            {
                playerCopperCount.text = resources[0].ToString();
                playerLumberCount.text = resources[1].ToString();
                playerLoyalistCount.text = resources[2].ToString();
                playerCoinCount.text = resources[3].ToString();
            }
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
        if (GameInfo.game_type == "net")
        {
            Debug.Log("Updated Resources: " + resources[0] + ", " + resources[1] + ", " + resources[2] + ", " + resources[3]);
            if (turnCount >= 4)
            {
                opponentCopperCount.text = resources[0].ToString();
                opponentLumberCount.text = resources[1].ToString();
                opponentLoyalistCount.text = resources[2].ToString();
                opponentCoinCount.text = resources[3].ToString();
            }
        }
        else if (GameInfo.game_type == "local")
        {
            if (turnCount > 4)
            {
                opponentCopperCount.text = resources[0].ToString();
                opponentLumberCount.text = resources[1].ToString();
                opponentLoyalistCount.text = resources[2].ToString();
                opponentCoinCount.text = resources[3].ToString();
            }
        }
    }

    

    public void ChangeNodeOwner(int nodeNum)
    {
        if (activeSide == Owner.US)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
            if ((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece))
            {
                GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USNodeHighlightedSprite;
            }
            else
            {
                GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USNodeSprite;
            }
        }
        else
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
            if ((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece))
            {
                GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USSRNodeHighlightedSprite;
            }
            else
            {
                GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USSRNodeSprite;
            }
        }

        nodesPlacedThisTurn.Add(nodeNum);

        if (((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece)) && isSetupTurn)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().isSetupNode = true;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupNode = nodeNum;
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupNode = nodeNum;
            }
        }

        if (GameInfo.game_type == "local")
        {
            if (activeSide == aiPiece)
            {
                Debug.Log("AI placed node " + nodeNum);
            }
        }
        Debug.Log("FirstSetupNode: " + firstSetupNode);
        Debug.Log("SecondSetupNode: " + secondSetupNode);
    }

    public void ChangeBranchOwner(int branchNum)
    {
        if (activeSide == Owner.US)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;

            if ((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece))
            {
                GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USBranchHighlightedSprite;
            }
            else
            {
                GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USBranchSprite;
            }

        }
        else
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;

            if ((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece))
            {
                GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USSRBranchHighlightedSprite;
            }
            else
            {
                GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USSRBranchSprite;
            }
        }

        branchesPlacedThisTurn.Add(branchNum);

        if (((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece)) && isSetupTurn)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().isSetupBranch = true;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupBranch = branchNum;
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupBranch = branchNum;
            }
        }


        if (GameInfo.game_type == "local")
        {
            if (activeSide == aiPiece)
            {
                Debug.Log("AI placed branch " + branchNum);
            }
        }
    }

    public void UnplaceNode(int nodeNum)
    {
        nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.Nil;
        GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = EmptyNodeSprite;
        GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(104, 118, 137, 255);

        nodesPlacedThisTurn.Remove(nodeNum);

        if (((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece)) && isSetupTurn)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().isSetupNode = false;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupNode = -1;
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupNode = -1;
            }
        }
    }

    public void UnplaceBranch(int branchNum)
    {
        allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.Nil;
        GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = EmptyBranchSprite;
        GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color32(156, 167, 176, 255);

        if (((GameInfo.game_type == "local" && activeSide == humanPiece) || (GameInfo.game_type == "net" && activeSide == netPiece)) && isSetupTurn)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().isSetupBranch = false;
            if (turnCount == 1 || turnCount == 2)
            {
                firstSetupBranch = -1;
            }
            else if (turnCount == 3 || turnCount == 4)
            {
                secondSetupBranch = -1;
            }
        }

        branchesPlacedThisTurn.Remove(branchNum);
    }

    public bool LegalUINodeMove(int node, Owner activeSide, List<int> myBranches)
    {
        bool isLegal = true;

        if (nodes[node].GetComponent<NodeInfo>().nodeOwner != Owner.Nil) { isLegal = false; }
        
        if ( turnCount < 5)
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

        //Debug.Log("IsLegal1: " + isLegal);
        //Debug.Log("LegalUIBranch branch: " + branch + ", Active Side: " + activeSide);


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
            //Debug.Log("IsLegal2: " + isLegal);
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
        //Debug.Log("IsLegal3: " + isLegal);
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

        if (allBranches[branch].GetComponent<BranchInfo>().branchOwner != Owner.Nil) { 
            isLegal = false; }

        // if this is not a Setup Move
        if (isLegal && myBranches.Count >= 2)
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
        }

        // a branch on a setup move must have an available adjacent node that can also be claimed
        if (isLegal && (turnCount == 3 || turnCount == 4))
        {
            Relationships.connectionsRoadNode.TryGetValue(branch, out List<int> adjacentNodes);
            if (LegalNodeMove(adjacentNodes[0], activeSide, new List<int>(new int[] { branch })) || LegalNodeMove(adjacentNodes[1], activeSide, new List<int>(new int[] { branch }))) ;
            else isLegal = false;
        }

        // a branch cannot be placed inside a multicaptured square owned by opponent
        Relationships.connectionsRoadTiles.TryGetValue(branch, out connectedTiles);
        foreach (int tile in connectedTiles)
        {
            if (resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != Owner.Nil && resourceList[tile].GetComponent<ResourceInfo>().resoureTileOwner != activeSide)
                isLegal = false;
            if (!isLegal) break;
        }

        return isLegal;
    }

    public void Trade(List<int> resources, Owner who)
    {
        bool makeTrade = false;

        int traded_for = 0, traded_in = 0;

        if (GameInfo.game_type == "net")
        {
            List<int> heldResources = localPlayer.GetComponent<Player>().__resources;
            for (int i = 0; i < 4; i++)
            {
                if (resources[i] < 0)
                {
                    if (heldResources[i] >= Math.Abs(resources[i]))
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
                    if (resources[i] < 2)
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
                localPlayer.GetComponent<Player>().UpdateResources(resources);
                int[] tempResources = localPlayer.GetComponent<Player>().__resources.ToArray();
                networkController.SetResources(tempResources);
                networkController.SendUpdateResourcesInOpponentUI();
            }
        }
        else if (GameInfo.game_type == "local")
        {
            List<int> heldResources = (who == aiPiece) ? player2.GetComponent<AI>().__resources : player1.GetComponent<Player>().__resources;
            for (int i = 0; i < 4; i++)
            {
                if (resources[i] < 0)
                {
                    if (heldResources[i] >= Math.Abs(resources[i]))
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
                    if (resources[i] < 2)
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
                if (who == aiPiece)
                {
                    player2.GetComponent<AI>().UpdateResources(resources);
                    Debug.Log("AI Traded: " + resources[0] + " red, " + resources[1] + " blue, " + resources[2] + " yellow, " + resources[3] + " green");
                }
                else
                    player1.GetComponent<Player>().UpdateResources(resources);
            }
        }
    }

    public void UIEndGame()
    {
        SetVolume();

        inBuildMode = false;
        tradeButton.GetComponent<Button>().interactable = false;
        keyButton.GetComponent<Button>().interactable = false;
        endTurnButton.GetComponent<Button>().interactable = false;

        if (GameInfo.game_type == "local")
        {
            if (humanPiece == Owner.US)
            {
                if (player1.GetComponent<Player>().__human_score >= 10)
                {
                    gameOverUSWin.SetActive(true);
                    USMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USVictory.Play(0);

                }
                else
                {
                    gameOverUSLoss.SetActive(true);
                    USMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USLoss.Play(0);
                }
            }
            else
            {
                if (player1.GetComponent<Player>().__human_score >= 10)
                {
                    gameOverUSSRWin.SetActive(true);
                    USSRMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USSRVictory.Play(0);
                }
                else
                {
                    gameOverUSSRLoss.SetActive(true);
                    USSRMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USSRLoss.Play(0);

                }
            }
        }

        else if (GameInfo.game_type == "net")
        {
            if (netPiece == Owner.US)
            {
                if (localPlayer.GetComponent<Player>().__human_score >= 10)
                {
                    gameOverUSWin.SetActive(true);
                    USMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USVictory.Play(0);

                }
                else
                {
                    gameOverUSLoss.SetActive(true);
                    USMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USLoss.Play(0);
                }
            }
            else
            {
                if (localPlayer.GetComponent<Player>().__human_score >= 10)
                {
                    gameOverUSSRWin.SetActive(true);
                    USSRMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USSRVictory.Play(0);
                }
                else
                {
                    gameOverUSSRLoss.SetActive(true);
                    USSRMusic.GetComponent<AudioSource>().volume = 0;
                    GameObject.FindObjectOfType<BoardManager>().USSRLoss.Play(0);

                }
            }
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

        if (GameInfo.game_type == "net")
        {
            if(localPlayer.GetComponent<Player>().__network_score >= 10 || localPlayer.GetComponent<Player>().__human_score >= 10)
            {
                //tradeButton.SetActive(false);
                //endTurnButton.SetActive(false);

                end = true;
                UIEndGame();
            }
        }

        else if (GameInfo.game_type == "local")
        {
            if (player2.GetComponent<AI>().__ai_score >= 10 || player1.GetComponent<Player>().__human_score >= 10)
            {
                //tradeButton.SetActive(false);
                //endTurnButton.SetActive(false);

                if (player1.GetComponent<Player>().__human_score >= 10) player2.GetComponent<AI>().Loss();
                else player2.GetComponent<AI>().Win();

                end = true;
                UIEndGame();
            }
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

        if (GameInfo.game_type == "net") {
            if (cdl.longestNetOwner != oldLongest)
            {
                if (oldLongest == netPiece)
                {
                    localPlayer.GetComponent<Player>().LoseLongestNet();
                }
                else if(oldLongest != Owner.Nil)
                {
                    localPlayer.GetComponent<Player>().NetworkLoseLongestNet();
                   
                }

                if (cdl.longestNetOwner == netPiece)
                {
                    localPlayer.GetComponent<Player>().GetLongestNet();
                }
                else
                {
                    localPlayer.GetComponent<Player>().NetworkGetLongestNet();
                }
            }
        }

        else if (GameInfo.game_type == "local")
        {
        // tell player of a change in longest net holder
            if (cdl.longestNetOwner != oldLongest) 
            {
                if (oldLongest == aiPiece && player2.GetComponent<AI>().trainingMode)
                {
                    player2.GetComponent<AI>().LoseLongestNet();
                }
                else if (oldLongest == humanPiece)
                {
                    player1.GetComponent<Player>().LoseLongestNet();
                }

                if (cdl.longestNetOwner == aiPiece && GameObject.FindGameObjectWithTag("AI").GetComponent<AI>().trainingMode)
                {
                    player2.GetComponent<AI>().GetLongestNet();
                }
                else if (cdl.longestNetOwner == humanPiece)
                {
                    player1.GetComponent<Player>().GetLongestNet();
                }
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


        if (GameInfo.game_type == "net") 
        {
            if (who == netPiece)
            {
                localPlayer.GetComponent<Player>().UpdateScore(score);
            }
            else
            {
                localPlayer.GetComponent<Player>().UpdateNetScore(score);
            }

        }

        else if (GameInfo.game_type == "local")
        {
            if (who == aiPiece)
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

       if(cdl.longestNetOwner == Owner.Nil)
        {
            longestNetworkUS.SetActive(false);
            longestNetworkUSSR.SetActive(false);
        }
       else if(cdl.longestNetOwner == Owner.US)
        {
            longestNetworkUS.SetActive(true);
            longestNetworkUSSR.SetActive(false);
        }
       else if(cdl.longestNetOwner == Owner.USSR)
        {
            longestNetworkUS.SetActive(false);
            longestNetworkUSSR.SetActive(true);
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
        if (GameInfo.game_type == "net")
        {
            localPlayer.GetComponent<Player>().UpdateResources(allocation);
        }

        else if (GameInfo.game_type == "local")
        {
            if (activeSide == aiPiece)
            {
                player2.GetComponent<AI>().UpdateResources(allocation);
            }
            else
            {
                player1.GetComponent<Player>().UpdateResources(allocation);
            }
        }
    }

    //public void EnterBuildMode()
    //{
    //    inBuildMode = true;
    //}

    public void ConfirmPiecePlacement()
    {
        for (int i = 0; i < nodesPlacedThisTurn.Count; i++)
        {
            nodes[nodesPlacedThisTurn[i]].GetComponent<NodeInfo>().placementConfirmed = true;

            if (GameInfo.game_type == "local")
            {
                if (player1.GetComponent<Player>().__owned_nodes.Contains(nodesPlacedThisTurn[i]) && activeSide == Owner.US)
                {
                    GameObject.FindGameObjectsWithTag("Node")[nodesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USNodeSprite;
                }
                else if (player1.GetComponent<Player>().__owned_nodes.Contains(nodesPlacedThisTurn[i]) && activeSide == Owner.USSR)
                {
                    GameObject.FindGameObjectsWithTag("Node")[nodesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USSRNodeSprite;
                }
            }
            else if (GameInfo.game_type == "net")
            {
                if (localPlayer.GetComponent<Player>().__owned_nodes.Contains(nodesPlacedThisTurn[i]) && activeSide == Owner.US)
                {
                    GameObject.FindGameObjectsWithTag("Node")[nodesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USNodeSprite;
                }
                else if (localPlayer.GetComponent<Player>().__owned_nodes.Contains(nodesPlacedThisTurn[i]) && activeSide == Owner.USSR)
                {
                    GameObject.FindGameObjectsWithTag("Node")[nodesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USSRNodeSprite;
                }
            }
        }
        for (int i = 0; i < branchesPlacedThisTurn.Count; i++)
        {
            allBranches[branchesPlacedThisTurn[i]].GetComponent<BranchInfo>().placementConfirmed = true;

            if (GameInfo.game_type == "local")
            {
                if (player1.GetComponent<Player>().__owned_branches.Contains(branchesPlacedThisTurn[i]) && activeSide == Owner.US)
                {
                    GameObject.FindGameObjectsWithTag("Branch")[branchesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USBranchSprite;
                }
                else if (player1.GetComponent<Player>().__owned_branches.Contains(branchesPlacedThisTurn[i]) && activeSide == Owner.USSR)
                {
                    GameObject.FindGameObjectsWithTag("Branch")[branchesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USSRBranchSprite;
                }
            }
            else if (GameInfo.game_type == "net")
            {
                if (localPlayer.GetComponent<Player>().__owned_branches.Contains(branchesPlacedThisTurn[i]) && activeSide == Owner.US)
                {
                    GameObject.FindGameObjectsWithTag("Branch")[branchesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USBranchSprite;
                }
                else if (localPlayer.GetComponent<Player>().__owned_branches.Contains(branchesPlacedThisTurn[i]) && activeSide == Owner.USSR)
                {
                    GameObject.FindGameObjectsWithTag("Branch")[branchesPlacedThisTurn[i]].GetComponent<SpriteRenderer>().sprite = USSRBranchSprite;
                }
            }
        }
    }

    public void EndTurnButtonClicked()
    {
        if (turnCount > 4)
            isSetupTurn = false;

        if (end) return;
        
        if (isSetupTurn)
        {
            if (SetupturnlegalCheck()) // if (E & CL have been placed)
            {
                // prompt player to confirm submission
                if (true) // user confirmed turn submission
                {
                    ConfirmPiecePlacement();
                    
                    if(GameInfo.game_type == "net")
                    {
                        networkController.SetNodesPlaced(nodesPlacedThisTurn.ToArray());
                        networkController.SetBranchesPlaced(branchesPlacedThisTurn.ToArray());
                        if (turnCount != 2)
                        {
                            branchesPlacedThisTurn.Clear();
                            nodesPlacedThisTurn.Clear();
                        }
                    }
                    else if (GameInfo.game_type == "local")
                    {
                        branchesPlacedThisTurn.Clear();
                        nodesPlacedThisTurn.Clear();
                    }

                    EndTurn();
                    // provide player with indication that opponent is taking turn
                    if (GameInfo.game_type == "local")
                    {
                        if (turnCount != 3)
                        {
                            // player2.GetComponent<AI>().AIMove(turnCount);
                            StartCoroutine(TimeStop());
                        }
                    }                     
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

                ConfirmPiecePlacement();
                
                if (GameInfo.game_type == "net")
                {
                    networkController.SetNodesPlaced(nodesPlacedThisTurn.ToArray());
                    networkController.SetBranchesPlaced(branchesPlacedThisTurn.ToArray());
                    networkController.SetResources(localPlayer.GetComponent<Player>().__resources.ToArray());
                }
                branchesPlacedThisTurn.Clear();
                nodesPlacedThisTurn.Clear();

                EndTurn();
                // provide player with indication that opponent is taking turn

                if (GameInfo.game_type == "local")
                {
                    //player2.GetComponent<AI>().AIMove(turnCount);
                    StartCoroutine(TimeStop());
                }
            }
        }
    }
    private bool SetupturnlegalCheck()
    {
        bool islegal = true;
        // player1.GetComponent<Player>().UpdateResources
        List<int> tempPlayerResources;

        if (GameInfo.game_type == "net")
        {
            for (int i = 0; i < localPlayer.GetComponent<Player>().__resources.Count; i++)
            {
                if (localPlayer.GetComponent<Player>().__resources[i] != 0)
                {
                    islegal = false;
                    break;
                }
            }
        }
        else if (GameInfo.game_type == "local")
        {
            for (int i = 0; i < player1.GetComponent<Player>().__resources.Count; i++)
            {
                if (player1.GetComponent<Player>().__resources[i] != 0)
                {
                    islegal = false;
                    break;
                }
            }
        }
        return islegal;
    }
    private void BtnToggle()
    {
        if ((GameInfo.game_type == "local" && activeSide == aiPiece && !isSetupTurn) || (GameInfo.game_type == "net" && activeSide == (netPiece == Owner.US ? Owner.USSR : Owner.US) && !isSetupTurn))
        {
            tradeButton.GetComponent<Button>().interactable = false;
        }
        else if((GameInfo.game_type == "local" && activeSide == humanPiece && !isSetupTurn) || (GameInfo.game_type == "net" && activeSide == (netPiece == Owner.US ? Owner.US : Owner.USSR) && !isSetupTurn))
        {
            tradeButton.GetComponent<Button>().interactable = true;
        }
        //endTurnButton.SetActive(!endTurnButton.activeSelf);
        //tradeButton.GetComponent<Button>().interactable = !tradeButton.GetComponent<Button>().interactable;
        endTurnButton.GetComponent<Button>().interactable = !endTurnButton.GetComponent<Button>().interactable;
    }

    public void EndTurn()
    {
        tradeWindow.SetActive(false);
        exchangeWindow.SetActive(false);
        
        if (GameInfo.game_type == "net")
        {
            if (turnCount == 2)
            {
                localPlayer.GetComponent<Player>().UpdateResources(new List<int>(4) { 1, 1, 2, 2 });
                turnCount++;
                BoardCheck();
            }
            else
            {
                turnCount++;

                

                if (activeSide == Owner.US)
                {
                    activeSide = Owner.USSR;
                }
                else
                {
                    activeSide = Owner.US;
                }


                SetupTurnTelegram.SetActive(false);
                YourTurnTelegram.SetActive(false);

                inBuildMode = !inBuildMode;
                BoardCheck();
                networkController.SendMove();
                if (end) return;
                playerTraded = false;
                BtnToggle();
                NetworkGame();
            }

        }
        else if (GameInfo.game_type == "local")
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

            Debug.Log("TurnCount = " + turnCount);

            if (activeSide == humanPiece)
            {
                if (turnCount < 5)
                {
                    Debug.Log("Should display Setup Turn");
                    SetupTurnTelegram.SetActive(true);
                }
                else
                {
                    Debug.Log("Should display Your Turn");

                    YourTurnTelegram.SetActive(true);
                }
            }
            else
            {
                SetupTurnTelegram.SetActive(false);
                YourTurnTelegram.SetActive(false);
            }

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
    }
    #endregion
    public void NetworkGame()
    {
        networkController.SetPlayerTurn(false);

        if (turnCount > 1 || firstPlayer != netPiece)
        {
            StartCoroutine(networkController.WaitForTurn());

        }

    }

    public void TurnReceived()
    {
        Debug.Log("BM.TurnReceived() Called");
        if(turnCount == 2)
        {
            turnCount++;
        }
        turnCount++;
        
        Debug.Log("Turn count: " + turnCount);
        
        if (activeSide == Owner.US)
        {
           activeSide = Owner.USSR;
        }
        else
        {
           activeSide = Owner.US;
        }

        inBuildMode = !inBuildMode;
        Debug.Log("Build mode: " + inBuildMode);
        Debug.Log("Active Side: " + activeSide);

        if(turnCount == 4)
        {
            localPlayer.GetComponent<Player>().UpdateResources(new List<int>(4) { 1, 1, 2, 2 });
        }

        if (turnCount >= 5)
        {
            tradeButton.GetComponent<Button>().interactable = true;
        }


        if (turnCount < 5)
        {
            SetupTurnTelegram.SetActive(true);
        }
        else
        {
            YourTurnTelegram.SetActive(true);
        }


        ReceiveMoveFromNetwork();
        BtnToggle();
        BoardCheck();
    }

    public void ReceiveMoveFromNetwork()
    {
        Debug.Log("BM.ReceiveMoveFromNetwork() called");
        int[] tempNetworkNodes = networkController.GetNodesPlaced();
        int[] tempNetworkBranches = networkController.GetBranchesPlaced();
        

        List<int> networkNodes = new List<int>(tempNetworkNodes);
        List<int> networkBranches = new List<int>(tempNetworkBranches);

        if (turnCount > 4)
        {
            int[] tempResources = networkController.GetResources();
            List<int> networkResources = new List<int>(tempResources);
            UpdateOpponentResourcesInUI(networkResources);
        }

        foreach (int node in networkNodes)
        {
            NetworkChangeNodeOwner(node);
        }
        foreach(int branch in networkBranches)
        {
            NetworkChangeBranchOwner(branch);
        }
        
        networkController.ClearBranchesandNodesandResources();
        if (turnCount >= 5)
        {
            AllocateResources();
            int[] tempResources = localPlayer.GetComponent<Player>().__resources.ToArray();
            networkController.SetResources(tempResources);
            networkController.SendUpdateResourcesInOpponentUI();
        }
       
    }

    IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(1.0f);
        player2.GetComponent<AI>().AIMove(turnCount);
        Debug.Log("wait for 1s");
    }

    IEnumerator NetTimeStop()
    {
        yield return new WaitForSeconds(3.0f);
    }

    public void setNetworkManagerReference()
    {
        networkController.SetBoardManagerReference(this);
    }

    public void NetworkChangeNodeOwner(int nodeNum)
    {
        GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
        if (activeSide == Owner.US)
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USSRNodeSprite;
            //GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0);
        }
        else
        {
            nodes[nodeNum].GetComponent<NodeInfo>().nodeOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().sprite = USNodeSprite;
            //GameObject.FindGameObjectsWithTag("Node")[nodeNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150); 
        }

    }

    public void NetworkChangeBranchOwner(int branchNum)
    {
        GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
        if (activeSide == Owner.US)
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.USSR;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USSRBranchSprite;
            //GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(200, 0, 0); 
        }
        else
        {
            allBranches[branchNum].GetComponent<BranchInfo>().branchOwner = Owner.US;
            GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().sprite = USBranchSprite;
            //GameObject.FindGameObjectsWithTag("Branch")[branchNum].GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 150);
        }
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
        return boardSeed;
    }

    public void StartNetworkGame()
    {
        boardManager = this;
        setNetworkManagerReference();
        if (GameInfo.host == true)
        {
            networkController.InstantiateNetworkPlayer();
            Debug.Log("zz Waiting for guest player to load");
            StartCoroutine(networkController.WaitForOtherPlayersLoaded());          

        }
        else
        {
            Debug.Log("qq Starting to instantiate network player");
            networkController.InstantiateNetworkPlayer();
            Debug.Log("qq Player has loaded");
            networkController.PlayerHasLoaded();          
            Debug.Log("qq Waiting for seed");
            StartCoroutine(networkController.WaitForSeed());
        }
    }

    public void ReceiveSeedFromNetwork()
    {    
        customBoardSeed = networkController.GetBoardSeed();
        networkController.SetBoardSeed("");
        Debug.Log("qq Received Seed: " + customBoardSeed);
        SetupScene();
    }

    public void OtherPlayersHaveLoaded()
    {
        Debug.Log("zz OtherPlayersHaveLoaded called");
        customBoardSeed = GetRandomBoardSeed();
        Debug.Log("zz Generated board seed: " + customBoardSeed);
        networkController.SetBoardSeed(customBoardSeed);
        Debug.Log("zz Board manager has set network controller seed");
        networkController.SendSeed();
        Debug.Log("zz Board manager has sent seed");
        networkController.SetBoardSeed("");
        SetupScene();
    }

    public void PlayerDisconnected()
    {
        if(activeSide == netPiece)
        {
            BtnToggle();
            playerDisconnectedPopup.gameObject.SetActive(true);
        }
        else
        {
            playerDisconnectedPopup.gameObject.SetActive(true);
        }
    }

    public void PlayerLeft()
    {
        if (activeSide == netPiece)
        {
            BtnToggle();
            playerLeftPopup.gameObject.SetActive(true);
        }
        else
        {
            playerLeftPopup.gameObject.SetActive(true);
        }
    }

    public void toggleBackButtonConfirmation()
    {
        exitConfirmationPopup.SetActive(!exitConfirmationPopup.activeSelf);
        settingsPopup.SetActive(false);
    }

    public bool getEnd()
    {
        return end;
    }
}


