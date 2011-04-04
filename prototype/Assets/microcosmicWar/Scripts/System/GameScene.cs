
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NetworkView))]

public class GameScene : MonoBehaviour
{


    //单实例类,一个场景只许有一个
    static protected GameScene singletonInstance;

    public string lastSceneInfoName = "_info";

    public GameObject sceneData;


    public HeroSpawn pismirePlayerSpawn;
    public HeroSpawn beePlayerSpawn;

    public HeroSpawn playerSpawn;
    public HeroSpawn adversaryPlayerSpawn;

    public bool needCreatePlayer = true;

    public GameObject useSceneData;

    void setSpawn(ref HeroSpawn pSpawn, Race pRace)
    {
        if(!pSpawn)
        {
            foreach (Transform lSpawn in GameSceneManager.Singleton
                .getManager(pRace, GameSceneManager.UnitManagerType.heroSpawn))
            {
                pSpawn = lSpawn.GetComponent<HeroSpawn>();
                break;
            }
        }

    }

    void Awake()
    {
        Random.seed = System.Environment.TickCount;
        singletonInstance = this;


        Network.isMessageQueueRunning = true;
        zzCreatorUtility.resetCreator();

        //}
    }

    void init()
    {

        setSpawn(ref pismirePlayerSpawn, Race.ePismire);
        setSpawn(ref beePlayerSpawn, Race.eBee);
        //print("Awake");
        //if(Network.peerType==NetworkPeerType.Disconnected || Network.peerType==NetworkPeerType.Connecting )
        //	Network.InitializeServer(32, 25000);


        //creat team info
        //{
        rule1 lRule = gameObject.GetComponent<rule1>();
        TeamInfo lTeam1Info = new TeamInfo();
        lTeam1Info.teamName = PlayerInfo.eRaceToString(Race.ePismire);
        TeamInfo lTeam2Info = new TeamInfo();
        lTeam2Info.teamName = PlayerInfo.eRaceToString(Race.eBee);
        lRule.addTeam(lTeam1Info);
        lRule.addTeam(lTeam2Info);
    }

    public static GameScene getSingleton()
    {
        return singletonInstance;
    }

    public static GameScene Singleton
    {
        get { return singletonInstance; }
    }

    public void CreatePlayer()
    {
        playerSpawn.createHeroFirstTime();

        if (Network.connections.Length > 0)
            adversaryPlayerSpawn.createHeroFirstTime();
    }

    void Start()
    {
        init();
        PlayerInfo lPlayerInfo = playerInfo;
        //print(playerInfo.getRace());
        //Race race=playerInfo.getRace();

        if (lPlayerInfo.getRace() == Race.ePismire)
        {
            playerSpawn = pismirePlayerSpawn;
            adversaryPlayerSpawn = beePlayerSpawn;
        }
        else
        {
            playerSpawn = beePlayerSpawn;
            adversaryPlayerSpawn = pismirePlayerSpawn;
        }

        //adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
        if (zzCreatorUtility.isHost())
        {
            if (needCreatePlayer)
            {
                playerSpawn.setOwer(Network.player);

                if (Network.connections.Length > 0)
                {
                    int lIntRace = (int)PlayerInfo.getAdversaryRace(lPlayerInfo.getRace());
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
        //Board.clearList();
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

    PlayerInfo _playerInfo;

    public PlayerInfo playerInfo
    {
        get 
        { 
            if(_playerInfo==null)
            {
                GameObject lLastSceneInfo = GameObject.Find(lastSceneInfoName);
                if (lLastSceneInfo)
                {
                    PlayerInfo playerInfo = sceneData.GetComponent<PlayerInfo>();
                    playerInfo.setData(lLastSceneInfo.GetComponent<PlayerInfo>());
                    Destroy(lLastSceneInfo);
                }
                useSceneData = sceneData;
                _playerInfo = useSceneData.GetComponent<PlayerInfo>();
                _playerInfo.UiRoot = zzObjectMap.getObject("TopUI")
                    .GetComponent<zzSceneObjectMap>()
                    .getObject(PlayerInfo.eRaceToString(_playerInfo.race));
            }
            return _playerInfo;
        }
    }


    [RPC]
    public void ImpGameResult(string pWinerRaceName)
    {
        PlayerInfo playerInfo = useSceneData.GetComponent<PlayerInfo>();

        if (pWinerRaceName == playerInfo.getPlayerName())
            endGameScene("you win");
        else
            endGameScene("you lose");
    }
}