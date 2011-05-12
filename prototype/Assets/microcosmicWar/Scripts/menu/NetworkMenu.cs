
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

    [SerializeField]
    string _playerName = "player";

    public string playerName
    {
        get
        {
            return _playerName;
        }
        set
        {
            _playerName = value;
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

    void Awake()
    {
        Network.Disconnect();
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        //Debug.Log("Could not connect to server: "+ error);
        networkConnectionError = error;
        //Network.useNat = false;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        print("OnPlayerConnected");
        Race lServerRace = raceSelect;
        //networkView.RPC( "LoadMyLevel", RPCMode.AllBuffered, "", 0);

        int lIntRace = (int)Race.ePismire;
        //让联接的客户端选择相反的种族
        if (lServerRace == Race.ePismire)
        {
            lIntRace = (int)Race.eBee;
        }
        else
        {
            lIntRace = (int)Race.ePismire;
        }
        networkView.RPC("LoadMyLevel", RPCMode.Others, lIntRace);
        LoadMyLevel((int)lServerRace);

        networkHost.UnregisterHost();
    }

    //@RPC
    //function setRace

    [RPC]
    void LoadMyLevel(int race)
    {
        //print("LoadMyLevel");
        Network.isMessageQueueRunning = false;

        //PlayerInfo playerInfo = GameObject.Find(savedDataName).GetComponentInChildren<PlayerInfo>();
        //playerInfo.setRace((Race)race);
        //playerInfo.setPlayerName(playerName);
        selectRaceEvent((Race)race);
        selectMapEvent(hostMap);

        //Application.LoadLevel("testScene");
        //Application.LoadLevel("netSewer2");
        loadGameSceneEvent();
    }

    public bool startHost()
    {
        if (mapName.Length>0&&checkMapAvailableEvent(mapName))
        {
            hostInfo = initHost(_playerName, raceSelect, mapName);
            return true;
        }
        return false;
    }

    public bool isServer
    {
        set
        {
            if(Network.isServer!=value)
            {
                if(value)
                {
                    if (startHost())
                        serverEvent("玩家:" + _playerName + " " + "地图:" + mapName);
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
    }

    [SerializeField]
    zzHostInfo hostInfo;

    zzHostInfo initHost(string playName, Race pRace, string pMapName)
    {
        hostMap = pMapName;
        var lHostInfo = new zzHostInfo();
        lHostInfo.gameType = gameTypeName;
        lHostInfo.gameName = gameName;

        Hashtable lTableData = new Hashtable();
        lTableData["playerName"] = playName;
        lTableData["race"] = (int)pRace;
        lTableData["map"] = pMapName;
        string lStringData = zzSerializeString.Singleton.pack(lTableData);
        lHostInfo.comment = lStringData;
        lHostInfo.port = _remotePort;

        networkHost.RegisterHost(lHostInfo);
        return lHostInfo;
    }

    [SerializeField]
    string _mapName;

    [SerializeField]
    string hostMap;

    public string mapName
    {
        get
        {
            return _mapName;
        }

        set
        {
            _mapName = value;
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
            Hashtable lTableData = (Hashtable)zzSerializeString.Singleton
                .unpackToData(lHostInfo.comment);
            hostMap = (string)lTableData["map"];
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
            if (lIdToElement.ContainsKey(lServerDic.Key))
            {
                lElement = lIdToElement[lServerDic.Key];
                lIdToElement.Remove(lServerDic.Key);
            }
            else
            {
                lElement = new zzGUILibTreeElement();
                lElement.stringData = lServerDic.Key;
            }
            //string lHostPlayName = (string)lTableData["playerName"];
            //string lHostMap = (string)lTableData["map"];
            Race lHostRace = (Race)lTableData["race"];
            var lStringInfo =new Dictionary<string, string>();
            lStringInfo["mapName"] = (string)lTableData["map"];
            lStringInfo["playerName"] = (string)lTableData["playerName"];
            lStringInfo["raceName"] = getRaceName(lHostRace);
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