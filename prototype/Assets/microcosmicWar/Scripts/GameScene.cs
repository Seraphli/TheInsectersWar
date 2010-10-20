
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NetworkView))]

public class GameScene : MonoBehaviour
{


    //单实例类,一个场景只许有一个
    static protected GameScene singletonInstance;

    public string lastSceneInfoName = "_info";
    //FIXME_VAR_TYPE sceneDataName= "_sceneData";

    public GameObject sceneData;
    /*
    Transform pismirePlayerSpawn;
    Transform beePlayerSpawn;

    GameObject pismirePlayerPrefab;
    GameObject beePlayerPrefab;

    Transform playerSpawn;
    GameObject playerPrefab;
    int adversaryLayerValue;
    */
    public HeroSpawn pismirePlayerSpawn;
    public HeroSpawn beePlayerSpawn;

    public HeroSpawn playerSpawn;
    public HeroSpawn adversaryPlayerSpawn;

    public bool needCreatePlayer = true;

    static GameObject sSceneData;
    //
    /*
    FIXME_VAR_TYPE winCondition=Hashtable();
    FIXME_VAR_TYPE loseCondition=Hashtable();

    void  addWinCondition (pValue){
        winCondition[pValue]=true;
    }

    void  reachCondition (pValue){
        enemyList.Remove(other.transform);
    }

    void  checkCondition ( Hashtable pList ,  bool isWin  ){
         if(pList.Count==0)
        {
        }
    }
    */
    // Use this for initialization
    void Awake()
    {
        singletonInstance = this;
        //print("Awake");
        //if(Network.peerType==NetworkPeerType.Disconnected || Network.peerType==NetworkPeerType.Connecting )
        //	Network.InitializeServer(32, 25000);
        GameObject lLastSceneInfo = GameObject.Find(lastSceneInfoName);
        if (lLastSceneInfo)
        {
            PlayerInfo playerInfo = sceneData.GetComponent<PlayerInfo>();
            playerInfo.setData(lLastSceneInfo.GetComponent<PlayerInfo>());
            //sceneData.AddComponent( lLastSceneInfo.GetComponent<PlayerInfo>() );
            Destroy(lLastSceneInfo);
        }

        Network.isMessageQueueRunning = true;
        zzCreatorUtility.resetCreator();

        sSceneData = sceneData;


        //creat team info
        //{
        rule1 lRule = gameObject.GetComponent<rule1>();
        TeamInfo lTeam1Info = new TeamInfo();
        lTeam1Info.teamName = PlayerInfo.eRaceToString(Race.ePismire);
        TeamInfo lTeam2Info = new TeamInfo();
        lTeam2Info.teamName = PlayerInfo.eRaceToString(Race.eBee);
        lRule.addTeam(lTeam1Info);
        lRule.addTeam(lTeam2Info);

        //}
    }

    public static GameScene getSingleton()
    {
        return singletonInstance;
    }

    public void CreatePlayer()
    {
        /*
            GameObject lClone = zzCreatorUtility.Instantiate(playerPrefab, playerSpawn.position,playerSpawn.rotation, 0);
            //lClone.GetComponent<SoldierAI>().SetAdversaryLayerValue(adversaryLayerValue);
	
            _2DCameraFollow cameraFollow = GameObject.Find("Main Camera").GetComponent<_2DCameraFollow>();
            cameraFollow.target=lClone.transform;
	
            //Soldier soldier = lClone.GetComponent<Soldier>();
            //#####soldier.userControl=true;
	
            //		SoldierAI soldierAI = lClone.GetComponent<SoldierAI>();
            //		soldierAI.AddFinalAim(playerSpawn);
            //	soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
        */
        playerSpawn.createHeroFirstTime();

        if (Network.connections.Length > 0)
            adversaryPlayerSpawn.createHeroFirstTime();
    }

    void Start()
    {
        //PlayerInfo playerInfo = sceneData.GetComponent<PlayerInfo>();
        PlayerInfo playerInfo = getPlayerInfo();
        //print(playerInfo.getRace());
        //Race race=playerInfo.getRace();

        if (playerInfo.getRace() == Race.ePismire)
        //sif(1)
        {
            playerSpawn = pismirePlayerSpawn;
            adversaryPlayerSpawn = beePlayerSpawn;
            //playerPrefab=pismirePlayerPrefab;
            //adversaryLayerValue = 1 << LayerMask.NameToLayer("pismire");
        }
        else
        {
            playerSpawn = beePlayerSpawn;
            adversaryPlayerSpawn = pismirePlayerSpawn;
            //playerPrefab=beePlayerPrefab;
            //adversaryLayerValue = 1 << LayerMask.NameToLayer("bee");
        }

        //adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
        if (zzCreatorUtility.isHost())
        {
            if (needCreatePlayer)
            {
                playerSpawn.setOwer(Network.player);

                if (Network.connections.Length > 0)
                {
                    int lIntRace = (int)PlayerInfo.getAdversaryRace(playerInfo.getRace());
                    networkView.RPC("RPCSetRace", Network.connections[0], lIntRace);
                    adversaryPlayerSpawn.setOwer(Network.connections[0]);
                }
                CreatePlayer();
            }
        }

    }

    [RPC]
    void RPCSetRace(int pRace)
    {
        PlayerInfo playerInfo = getPlayerInfo();
        playerInfo.setRace((Race)pRace);
    }

    protected bool needOnGUI = false;
    protected string buttonInfo = "";

    void OnGUI()
    {
        if (needOnGUI)
        {

            GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height / 2, 200, Screen.height / 2));
            if (GUILayout.Button(buttonInfo))
            {
                Time.timeScale = 1;
                Application.LoadLevel("LoaderMenu");
            }
            GUILayout.EndArea();
        }
    }

    public void endGameScene(string pInfo)
    {
        //假如已经停止了,则不往下执行
        Board.clearList();
        Time.timeScale = 0;
        playerSpawn.releaseHeroControl();
        if (needOnGUI)
            return;
        needOnGUI = true;
        buttonInfo = pInfo;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        endGameScene("Network Player Disconnection");
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        endGameScene("Network Player Disconnection");
    }

    public void gameResult(string pWinerRaceName)
    {
        ImpGameResult(pWinerRaceName);
        if (Network.peerType == NetworkPeerType.Disconnected)
            ImpGameResult(pWinerRaceName);
        else
            networkView.RPC("ImpGameResult", RPCMode.Others, pWinerRaceName);
    }

    public PlayerInfo getPlayerInfo()
    {
        PlayerInfo playerInfo = sSceneData.GetComponent<PlayerInfo>();
        return playerInfo;
    }


    [RPC]
    public void ImpGameResult(string pWinerRaceName)
    {
        PlayerInfo playerInfo = sSceneData.GetComponent<PlayerInfo>();

        if (pWinerRaceName == playerInfo.getPlayerName())
            endGameScene("you win");
        else
            endGameScene("you lose");
    }
}