
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NetworkView))]
public class NetworkMenu : MonoBehaviour
{

    public Race race
    {
        get
        {
            return raceSelect;
        }
        set
        {
            raceSelect = value;
        }
    }
    //玩家信息
    public Race raceSelect = Race.ePismire;

    public PlayerInfo playerInfo;

    //[SerializeField]
    //string _playerName = "player";

    public string playerName
    {
        get
        {
            return playerInfo.playerName;
        }
        set
        {
            playerInfo.playerName = value;
        }
    }

    //public string savedDataName = "_info";

    //网络联接

    [SerializeField]
    string _remoteIP = "127.0.0.1";
    public string remoteIP
    {
        get
        {
            return _remoteIP;
        }
        set
        {
            _remoteIP = value;
        }
    }

    [SerializeField]
    int _remotePort = 25000;
    public int remotePort
    {
        get
        {
            return _remotePort;
        }
        set
        {
            _remotePort = value;
        }
    }

    public string stateInfo
    {
        get
        {
            return Network.peerType.ToString();
        }
    }

    public string errorInfo
    {
        get
        {
            return networkConnectionError.ToString();
        }
    }

    public NetworkConnectionError networkConnectionError;

    public string gameTypeName = "CZLGameType";
    public string gameName = "czl game";

    public float hostListRefreshTimeout = 10.0f;
    //public float refreshTimePos = 0.0f;

    public ServerList serverList;
    //public GameObject broadcastObjectPrefab;
    //public GameObject broadcastSentObject;
    public zzNetworkHost networkHost;

    public GameObject broadcastRecieverObject;

    public float autoSentInterval = 0.43f;

    public bool useNat = false;

    public zzGUILibTreeInfo treeInfo;

    public System.Action<Race> selectRaceEvent;

    public System.Action<string> selectMapEvent;

    public System.Action loadGameSceneEvent;

    public System.Func<string, bool> checkMapAvailableEvent;

    public System.Action<string> serverEvent;

    protected Race raceSelectToEnum(int ID)
    {
        if (ID == 0)
            return Race.ePismire;
        if (ID == 1)
            return Race.eBee;
        Debug.LogError("raceSelectToEnum(int ID)");
        return Race.eNone;
    }

    [SerializeField]
    bool disconnectWhenAwake = true;

    void Awake()
    {
        if (disconnectWhenAwake)
            Network.Disconnect();
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        //Debug.Log("Could not connect to server: "+ error);
        networkConnectionError = error;
        //Network.useNat = false;
    }

    public bool useNetworkRoom = true;
    public string networkRoomName;

    void OnPlayerDisconnected()
    {
        print("OnPlayerDisconnected");
    }

    void OnConnectedToServer()
    {
        print("OnConnectedToServer");
        if (useNetworkRoom)
        {
            Network.SetSendingEnabled(0, false);

            Network.isMessageQueueRunning = false;

            //Network.SetLevelPrefix(levelPrefix);
            Application.LoadLevel(networkRoomName);

            Network.isMessageQueueRunning = true;
            Network.SetSendingEnabled(0, true);
        }
    }

    public void loadGame()
    {
        if (mapName.Length == 0)
            return;
        Network.RemoveRPCsInGroup(0);
        networkView.RPC("LoadMyLevel", RPCMode.Others, mapName);
        LoadMyLevel(mapName);

        networkHost.UnregisterHost();
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        print("OnPlayerConnected");
        if (useNetworkRoom)
            return;
        //Race lServerRace = raceSelect;
        //networkView.RPC( "LoadMyLevel", RPCMode.AllBuffered, "", 0);

        //int lIntRace = (int)Race.ePismire;
        ////让联接的客户端选择相反的种族
        //if (lServerRace == Race.ePismire)
        //{
        //    lIntRace = (int)Race.eBee;
        //}
        //else
        //{
        //    lIntRace = (int)Race.ePismire;
        //}
        loadGame();
    }

    //@RPC
    //function setRace
    public int levelPrefix = 1;
    [RPC]
    void LoadMyLevel(string pMapName)
    {
        //print("LoadMyLevel");
        print("Network.isServer:" + Network.isServer);
        // There is no reason to send any more data over the network on the default channel,
        // because we are about to load the level, thus all those objects will get deleted anyway
        Network.SetSendingEnabled(0, false);

        // We need to stop receiving because first the level must be loaded first.
        // Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;

        // All network views loaded from a level will get a prefix into their NetworkViewID.
        // This will prevent old updates from clients leaking into a newly created scene.
        Network.SetLevelPrefix(levelPrefix);

        //PlayerInfo playerInfo = GameObject.Find(savedDataName).GetComponentInChildren<PlayerInfo>();
        //playerInfo.setRace((Race)race);
        //playerInfo.setPlayerName(playerName);
        //selectRaceEvent((Race)race);
        selectMapEvent(pMapName);

        //Application.LoadLevel("testScene");
        //Application.LoadLevel("netSewer2");
        loadGameSceneEvent();

        // Allow receiving data again
        Network.isMessageQueueRunning = true;
        // Now the level has been loaded and we can start sending out data to clients
        Network.SetSendingEnabled(0, true);

    }

    //void updateHostInfo()
    //{
    //    hostInfo = createHostInfo();
    //}

    public bool startHost()
    {
        if (mapName.Length>0&&checkMapAvailableEvent(mapName))
        {
            hostInfo = createHostInfo();
            return true;
        }
        return false;
    }

    public bool _server = false;

    public bool isServer
    {
        set
        {
            if(Network.isServer!=value)
            {
                if(value)
                {
                    if (startHost())
                        serverEvent("玩家:" + playerName + " " + "地图:" + mapName);
                }
                else
                {
                    Network.Disconnect();
                    networkHost.UnregisterHost();
                    serverEvent("服务已关闭");
                }
            }
        }
        get 
        { 
            return Network.isServer; 
        }
    }

    void Start()
    {
        networkHost.addRegisterSucceedReceiver(
            () => Network.InitializeServer(32, _remotePort, useNat));

        networkHost.addRegisterFailReceiver(() => serverEvent("失败"));
        print(Network.peerType);
        if(!Network.isClient)
            isServer = _server;
    }

    [SerializeField]
    zzHostInfo hostInfo;

    zzHostInfo createHostInfo()
    {
        var lHostInfo = new zzHostInfo();
        updateHostInfo(lHostInfo);
        networkHost.RegisterHost(lHostInfo);
        return lHostInfo;
    }

    void updateHostInfo(zzHostInfo lHostInfo)
    {
        lHostInfo.gameType = gameTypeName;
        lHostInfo.gameName = gameName;

        Hashtable lTableData = new Hashtable();
        lTableData["playerName"] = playerName;
        lTableData["map"] = mapName;
        string lStringData = zzSerializeString.Singleton.pack(lTableData);
        lHostInfo.comment = lStringData;
        lHostInfo.port = _remotePort;
    }

    [SerializeField]
    string _mapName;

    //[SerializeField]
    //string hostMap;

    public string mapName
    {
        get
        {
            return _mapName;
        }

        set
        {
            if(_mapName!=value)
            {
                _mapName = value;
                if(Network.isServer)
                    updateHostInfo(hostInfo);
            }
        }
    }

    [SerializeField]
    string hostGUID;

    public void setHostGuid(string pGuid)
    {
        hostGUID = pGuid;
    }

    public void connectHost()
    {
        if(serverList.serverList.ContainsKey(hostGUID))
        {
            var lHostInfo = serverList.serverList[hostGUID];
            //Hashtable lTableData = (Hashtable)zzSerializeString.Singleton
            //    .unpackToData(lHostInfo.comment);
            //hostMap = (string)lTableData["map"];
            networkConnectionError = Network.Connect(lHostInfo.IP, lHostInfo.port);
        }
    }

    string getRaceName(Race pRace)
    {
        switch (pRace)
        {
            case Race.eBee: return "蜜蜂";
            case Race.ePismire: return "蚂蚁";
        }
        return null;
    }

    public void updateHostList()
    {
        var lServerList = serverList.serverList;
        var lElements = new zzGUILibTreeElement[lServerList.Count];
        int i = 0;
        Dictionary<string, zzGUILibTreeElement> lIdToElement
            = new Dictionary<string, zzGUILibTreeElement>();
        foreach (var lElement in treeInfo.treeInfo.elements)
        {
            //var lInfo = lElement.stringInfo;
            lIdToElement[lElement.stringData] = lElement;
        }
        foreach (var lServerDic in lServerList)
        {
            var lServer = lServerDic.Value;
            Hashtable lTableData = (Hashtable)zzSerializeString.Singleton
                .unpackToData(lServer.comment);
            zzGUILibTreeElement lElement;
            if (lIdToElement.TryGetValue(lServerDic.Key, out lElement))
            {
                lIdToElement.Remove(lServerDic.Key);
            }
            else
            {
                lElement = new zzGUILibTreeElement();
                lElement.stringData = lServerDic.Key;
            }
            //string lHostPlayName = (string)lTableData["playerName"];
            //string lHostMap = (string)lTableData["map"];
            var lStringInfo =new Dictionary<string, string>();
            lStringInfo["mapName"] = (string)lTableData["map"];
            lStringInfo["playerName"] = (string)lTableData["playerName"];
            lStringInfo["IP"] = lServer.IP;
            lElement.stringInfo = lStringInfo;
            lElements[i] = lElement;
            ++i;
        }
        zzGUILibTreeNode lNode = new zzGUILibTreeNode();
        lNode.elements = lElements;
        treeInfo.treeInfo.setData(lNode);
    }



}