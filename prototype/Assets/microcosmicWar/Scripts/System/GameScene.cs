
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

    Dictionary<NetworkPlayer, HeroSpawn> playerSpawnList
        = new Dictionary<NetworkPlayer, HeroSpawn>();
    public PlayerSpawn[] pismirePlayerSpawns;
    public PlayerSpawn[] beePlayerSpawns;

    public zzGUIBubbleMessage bubbleMessage;

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
        playerSpawnList[pPlayer] = pHeroSpawn;
    }

    void Start()
    {
        initEvent();
        //init();
        pismirePlayerSpawns = getSortedSpawns(Race.ePismire);
        beePlayerSpawns = getSortedSpawns(Race.eBee);
        PlayerInfo lPlayerInfo = playerInfo;
        if(Network.isServer)
        {
            //var lPlayers = lPlayerInfo.GetComponent<PlayerListInfo>().players;
            //foreach (var lPlayElement in lPlayers)
            //{
            //    if (!lPlayElement)
            //        continue;
            //    var lNetworkPlayer = lPlayElement.networkPlayer;
            //    var lSpawn = getSpawn(lPlayElement).addPlayer(lNetworkPlayer);
            //    addPlayerSpawn(lPlayElement.networkPlayer, lSpawn);
            //    if (lNetworkPlayer == Network.player)
            //        playerSpawn = lSpawn;
            //    else
            //        networkView.RPC("GameSceneSetPlayerSpawn",
            //            lNetworkPlayer,
            //            lSpawn.networkView.viewID);
            //}
            var lGamePlayers = lPlayerInfo.GetComponent<GamePlayers>();
            for (int i = 1; i <= lGamePlayers.playerSpaceCount;++i )
            {
                var lPlayElement = lGamePlayers.getPlayerInfo(i);
                if (lPlayElement)
                {
                    var lNetworkPlayer = lPlayElement.networkPlayer;
                    var lSpawn = getSpawn(lPlayElement).addPlayer(lNetworkPlayer);
                    addPlayerSpawn(lPlayElement.networkPlayer, lSpawn);
                    setPlayerSpawn(lSpawn, i);
                    networkView.RPC("GameSceneSetPlayerSpawn",
                            RPCMode.Others,
                            lSpawn.networkView.viewID,i);

                }
            }
        }
        else if (Network.isClient)
        {
            Network.sendRate = clientNetworkSendRate;
        }
        else
        {
            var lSpawn = getSpawn(getSpawns(lPlayerInfo.race), 0).addPlayer(Network.player);
            var lGamePlayers = lPlayerInfo.GetComponent<GamePlayers>();
            //初始化玩家列表
            lGamePlayers.init();
            addPlayerSpawn(Network.player, lSpawn);
            setPlayerSpawn(lSpawn, 1);
            playerSpawn = lSpawn;

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

    [RPC]
    void GameSceneSetPlayerSpawn(NetworkViewID pSpawn,int pPlayerID)
    {
        var lSpawn = NetworkView.Find(pSpawn).GetComponent<HeroSpawn>();
        setPlayerSpawn(lSpawn, pPlayerID);
    }

    void setPlayerSpawn(HeroSpawn lSpawn, int pPlayerID)
    {
        var lGamePlayers = playerInfo.GetComponent<GamePlayers>();
        var lPlayerInfo = lGamePlayers.getPlayerInfo(pPlayerID);
        lPlayerInfo.spawn = lSpawn;
        lSpawn.bubbleMessage = bubbleMessage;
        if (lGamePlayers.selfID == pPlayerID)
            playerSpawn = lSpawn;
        else
            lSpawn.playerName = pPlayerID.ToString()+"."+lPlayerInfo.playerName;
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
        gameEndEvent();
        gameInterruptedEvent(pInfo);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        interruptGame("Network Player Disconnection");
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        HeroSpawn lSpawn;
        playerSpawnList.TryGetValue(player, out lSpawn);
        if (lSpawn)
        {
            lSpawn.destroyTheSpawn();
            playerSpawnList.Remove(player);
            if (playerSpawnList.Count==1)
            {
                interruptGame("All Network Player Disconnection");
            }
        }
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