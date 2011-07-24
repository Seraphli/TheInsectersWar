
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NetworkView))]

public class GameScene : MonoBehaviour
{


    //单实例类,一个场景只许有一个
    static protected GameScene singletonInstance;

    public string lastSceneInfoName = "_info";

    public GameObject sceneData;

    List<KeyValuePair<NetworkPlayer, HeroSpawn>> playerSpawnList
        = new List<KeyValuePair<NetworkPlayer,HeroSpawn>>();
    public PlayerSpawn[] pismirePlayerSpawns;
    public PlayerSpawn[] beePlayerSpawns;

    PlayerSpawn[] getSortedSpawns( Race pRace )
    {
        var lSpawnRoot = GameSceneManager.Singleton
            .getManager(pRace, GameSceneManager.UnitManagerType.heroSpawn).managerRoot;
        List<PlayerSpawn> lPlayerSpawns = new List<PlayerSpawn>(lSpawnRoot.childCount);
        foreach (Transform lSpawn in lSpawnRoot)
        {
            lPlayerSpawns.Add(lSpawn.GetComponent<PlayerSpawn>());
        }
        lPlayerSpawns.Sort(
            delegate(PlayerSpawn x, PlayerSpawn y)
            {
                if (x._index == y._index)
                    return 0;
                if (x._index > y._index)
                    return 1;
                return -1;
            }
            );
        return lPlayerSpawns.ToArray();
    }

    PlayerSpawn[] getSpawns(Race pRace)
    {
        if (pRace == Race.ePismire)
            return pismirePlayerSpawns;
        else if(pRace == Race.eBee)
            return beePlayerSpawns;
        return null;
    }

    PlayerSpawn getSpawn(PlayerSpawn[] lPlayerSpawns,int pIndex)
    {
        return lPlayerSpawns[pIndex % lPlayerSpawns.Length];

    }

    PlayerSpawn getSpawn(PlayerElement pPlayerElement)
    {
        return getSpawn(getSpawns(pPlayerElement.race), pPlayerElement.spawnIndex);
    }

    public HeroSpawn pismirePlayerSpawn;
    public HeroSpawn beePlayerSpawn;

    public HeroSpawn playerSpawn;
    public HeroSpawn adversaryPlayerSpawn;

    public bool needCreatePlayer = true;

    //public GameObject useSceneData;

    public System.Action gameWinEvent;
    public System.Action gameLostEvent;
    public System.Action gameEndEvent;
    public System.Action<string> gameInterruptedEvent;

    void initEvent()
    {
        if (gameWinEvent == null)
            gameWinEvent = nullEvent;
        if (gameLostEvent == null)
            gameLostEvent = nullEvent;
        if (gameEndEvent == null)
            gameEndEvent = nullEvent;
    }

    static void nullEvent(){}

    //void setSpawn(ref HeroSpawn pSpawn, Race pRace)
    //{
    //    if(!pSpawn)
    //    {
    //        foreach (Transform lSpawn in GameSceneManager.Singleton
    //            .getManager(pRace, GameSceneManager.UnitManagerType.heroSpawn))
    //        {
    //            pSpawn = lSpawn.GetComponent<HeroSpawn>();
    //            break;
    //        }
    //    }
    //}

    void Awake()
    {
        Random.seed = System.Environment.TickCount;
        singletonInstance = this;


        //zzCreatorUtility.resetCreator();

        //}
    }

    //void init()
    //{

    //    setSpawn(ref pismirePlayerSpawn, Race.ePismire);
    //    setSpawn(ref beePlayerSpawn, Race.eBee);

    //}

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
        foreach (var lPlayerSpawn in playerSpawnList)
        {
            lPlayerSpawn.Value.createHeroFirstTime();
        }
    }

    public int clientNetworkSendRate = 30;

    void addPlayerSpawn(NetworkPlayer pPlayer,HeroSpawn pHeroSpawn)
    {
        playerSpawnList.Add(new KeyValuePair<NetworkPlayer, HeroSpawn>(
            pPlayer, pHeroSpawn));
    }

    void Start()
    {
        initEvent();
        //init();
        pismirePlayerSpawns = getSortedSpawns(Race.ePismire);
        beePlayerSpawns = getSortedSpawns(Race.eBee);
        PlayerInfo lPlayerInfo = playerInfo;
        //print(playerInfo.getRace());
        //Race race=playerInfo.getRace();

        //if (lPlayerInfo.getRace() == Race.ePismire)
        //{
        //    playerSpawn = pismirePlayerSpawn;
        //    adversaryPlayerSpawn = beePlayerSpawn;
        //}
        //else
        //{
        //    playerSpawn = beePlayerSpawn;
        //    adversaryPlayerSpawn = pismirePlayerSpawn;
        //}

        //adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
        if(Network.isServer)
        {
            var lPlayers = lPlayerInfo.GetComponent<PlayerListInfo>().players;
            foreach (var lPlayElement in lPlayers)
            {
                var lSpawn = getSpawn(lPlayElement).addPlayer(lPlayElement.networkPlayer);
                addPlayerSpawn(lPlayElement.networkPlayer, lSpawn);
            }
        }
        else if (Network.isClient)
        {
            Network.sendRate = clientNetworkSendRate;
        }
        else
        {
            var lSpawn = getSpawn(getSpawns(lPlayerInfo.race), 0).addPlayer(Network.player);
            addPlayerSpawn(Network.player, lSpawn);

        }
        CreatePlayer();

        //if (zzCreatorUtility.isHost())
        //{
        //    if (needCreatePlayer)
        //    {
        //        playerSpawn.setOwer(Network.player);

        //        if (Network.connections.Length > 0)
        //        {
        //            int lIntRace = (int)PlayerInfo.getAdversaryRace(lPlayerInfo.getRace());
        //            //networkView.RPC("RPCSetRace", Network.connections[0], lIntRace);
        //            adversaryPlayerSpawn.setOwer(Network.connections[0]);
        //        }
        //        CreatePlayer();
        //    }
        //}
        //else
        //    Network.sendRate = clientNetworkSendRate;

    }

    //[RPC]
    //void RPCSetRace(int pRace)
    //{
    //    playerInfo.setRace((Race)pRace);
    //}

    protected bool needOnGUI = false;
    protected string buttonInfo = "";

    //void OnGUI()
    //{
    //    if (needOnGUI)
    //    {

    //        GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height / 2, 200, Screen.height / 2));
    //        if (GUILayout.Button(buttonInfo))
    //        {
    //            Time.timeScale = 1;
    //            Application.LoadLevel("LoaderMenu");
    //        }
    //        GUILayout.EndArea();
    //    }
    //}

    public void endGameScene()
    {
        //假如已经停止了,则不往下执行
        //Board.clearList();
        Time.timeScale = 0;
        playerSpawn.releaseHeroControl();
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        singletonInstance = null;
    }

    void interruptGame(string pInfo)
    {
        endGameScene();
        gameInterruptedEvent(pInfo);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        interruptGame("Network Player Disconnection");
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        interruptGame("Network Player Disconnection");
    }

    public void gameResult(string pRaceName,bool pIsWiner)
    {
        if(zzCreatorUtility.isHost())
        {
            ImpGameResult(pRaceName, pIsWiner);
            if (Network.peerType != NetworkPeerType.Disconnected)
                networkView.RPC("ImpGameResult", RPCMode.Others, pRaceName, pIsWiner);

        }
    }

    PlayerInfo _playerInfo;

    public PlayerInfo playerInfo
    {
        get 
        { 
            if(_playerInfo==null)
            {
                GameObject lLastSceneInfo = GameObject.Find(lastSceneInfoName);
                _playerInfo = sceneData.GetComponent<PlayerInfo>();
                if (lLastSceneInfo)
                {
                    _playerInfo.setData(lLastSceneInfo.GetComponent<PlayerInfo>());
                    //在编辑器中要多次使用,所以不可以删除
                    //Destroy(lLastSceneInfo);
                }
                //useSceneData = sceneData;
                _playerInfo.UiRoot = _playerInfo.topUi
                    .GetComponent<zzSceneObjectMap>()
                    .getObject(PlayerInfo.eRaceToString(_playerInfo.race));
            }
            return _playerInfo;
        }
    }


    [RPC]
    public void ImpGameResult(string pWinerRaceName, bool pIsWiner)
    {
        //PlayerInfo playerInfo = sceneData.GetComponent<PlayerInfo>();

        if (pWinerRaceName == playerInfo.getPlayerName())
        {
            if (pIsWiner)
                gameWinEvent();
            else
                gameLostEvent();
        }
        //endGameScene("you win");
        else
        {
            if (pIsWiner)
                gameLostEvent();
            else
                gameWinEvent();
        }
            //endGameScene("you lose");
        endGameScene();
        gameEndEvent();
    }
}