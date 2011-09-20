using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkRoom : MonoBehaviour
{
    public int playerID
    {
        set { gamePlayers.selfID = value; }
        get { return gamePlayers.selfID; }
    }
    public GamePlayers gamePlayers;
    public PlayerInfo player;
    public PlayerListInfo playerListInfo;
    public int maxMemberCountEachRace = 4;
    public int maxPlayerCount = 4;
    Dictionary<NetworkPlayer, int> playerToID
        = new Dictionary<NetworkPlayer, int>();
    System.Action<string> messageSender;
    System.Action<string> errorMessageSender;
    System.Action<string> playerMessageSender;

    static void nullEventReceiver(){}

    //todo 用属性重写;分清接收和发送
    //in client
    System.Action playerReadyEvent;
    System.Action playerReadyInterruptedEvent;

    public void addPlayerReadyReceiver(System.Action pReceiver)
    {
        playerReadyEvent += pReceiver;
    }

    public void addPlayerReadyInterruptedReceiver(System.Action pReceiver)
    {
        playerReadyInterruptedEvent += pReceiver;
    }

    //in server
    System.Action netGameReadyEvent;
    System.Action netGameInterruptedEvent;
    public void addNetGameReadyReceiver(System.Action pReceiver)
    {
        netGameReadyEvent += pReceiver;
    }

    public void addNetGameInterruptedReceiver(System.Action pReceiver)
    {
        netGameInterruptedEvent += pReceiver;
    }

    void initEvent()
    {
        if(playerReadyEvent==null)
            playerReadyEvent = nullEventReceiver;
        if (playerReadyInterruptedEvent == null)
            playerReadyInterruptedEvent = nullEventReceiver;
        if (netGameReadyEvent == null)
            netGameReadyEvent = nullEventReceiver;
        if (netGameInterruptedEvent == null)
            netGameInterruptedEvent = nullEventReceiver;
    }

    [SerializeField]
    string _mapName;
    public string mapName
    {
        set
        {
            if(_mapName!=value)
            {
                if(Network.isServer)
                {
                    NetworkRoomSetMap(value);
                    networkView.RPC("NetworkRoomSetMap", RPCMode.Others,
                        _mapName);
                    netGameInterruptedEvent();
                }
            }
        }

        get { return _mapName; }
    }

    [RPC]
    void NetworkRoomSetMap(string pMap)
    {
        messageSender("地图:" + pMap);
        if (!WMGameConfig.checkMapAvailable(pMap))
        {
            errorMessageSender("你无此对战地图,可通过其他工具向房主索取");
            makeUnready();
        }
        _mapName = pMap;
    }

    public void addMessageReceiver(System.Action<string> pReceiver)
    {
        messageSender += pReceiver;
    }

    public void addErrorMessageReceiver(System.Action<string> pReceiver)
    {
        errorMessageSender += pReceiver;
    }

    public void addPlayerMessageReceiver(System.Action<string> pReceiver)
    {
        playerMessageSender += pReceiver;
    }

    static void nullMessageReceiver(string p){}

    //[SerializeField]
    //PlayerElement[] _playersInfo
    //{
    //    get
    //    {
    //        return playerListInfo.players;
    //    }
    //    set
    //    {
    //        playerListInfo.players = value;
    //    }
    //}

    public PlayerElement[]  playersInfo
    {
        get
        {
            return gamePlayers.playerListInfo.players;
        }
    }

    bool isExit(Race pRace, int pSpawnIndex)
    {
        foreach (var lPlayerInfo in gamePlayers)
        {
            if (lPlayerInfo
                && lPlayerInfo.race == pRace
                && lPlayerInfo.spawnIndex == pSpawnIndex)
                return true;
        }
        return false;
    }

    //public int[] pismirePlayer;

    //public int[] beePlayer;
    //Dictionary<string,int>

    System.Action roomDataChangedReceiver;

    public void addRoomDataChangedReceiver(System.Action pReceiver)
    {
        roomDataChangedReceiver += pReceiver;
    }

    void Awake()
    {
        gamePlayers.playerSpaceCount = maxPlayerCount;
        //_playersInfo = new PlayerElement[maxPlayerCount];
    }

    IEnumerator Start()
    {
        if (messageSender == null)
            messageSender = nullMessageReceiver;
        if (errorMessageSender == null)
            errorMessageSender = nullMessageReceiver;
        if (playerMessageSender == null)
            playerMessageSender = nullMessageReceiver;
        initEvent();
        while (Network.peerType == NetworkPeerType.Disconnected)
        {
            yield return null;
        }
        if (Network.isServer)
        {
            playerID = registerPlayer(gamePlayers.selfName, Network.player);
            sendData();
        }
        else
        {
            networkView.RPC("NetworkRoomRegister", RPCMode.Server, gamePlayers.selfName);
            do
            {
                yield return new WaitForSeconds(2f);
            } while (playerID == 0);
        }
    }

    void OnPlayerDisconnected(NetworkPlayer pPlayer)
    {
        int lPlayerId;
        if (!playerToID.TryGetValue(pPlayer, out lPlayerId))
            return;
        //int lPlayerIdIndex = playerIdToIndex(lPlayerId);
        playerLeaveMessage(lPlayerId, gamePlayers.getPlayerInfo(lPlayerId));
        Network.RemoveRPCs(pPlayer);
        Network.DestroyPlayerObjects(pPlayer);
        playerToID.Remove(pPlayer);
        gamePlayers.setPlayerInfo(lPlayerId, null);
        //_playersInfo[lPlayerIdIndex] = null;
        sendData();
        netGameInterruptedEvent();
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        errorMessageSender("连接中断,按返回按钮退出房间");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pPlayerName"></param>
    /// <param name="pPlayer"></param>
    /// <returns>player ID,玩家已满时,返回-1</returns>
    int registerPlayer(string pPlayerName, NetworkPlayer pPlayer)
    {
        int lPlayerID = 1;
        while (gamePlayers.getPlayerInfo(lPlayerID)
            && lPlayerID <= gamePlayers.playerSpaceCount)
            ++lPlayerID;
        if (lPlayerID > gamePlayers.playerSpaceCount)
            return -1;
        //int lPlayerID = indexToPlayerId(i);
        playerToID[pPlayer] = lPlayerID;
        //guidToPlayer[pGUID] = pPlayer;
        var lPlayersInfo= new PlayerElement
        {
            playerName = pPlayerName,
            race = Race.eNone,
            networkPlayer = pPlayer,
            spawnIndex = 0,
        };
        gamePlayers.setPlayerInfo(lPlayerID, lPlayersInfo);
        //中断事件,比如游戏倒计时
        netGameInterruptedEvent();
        //playerEnterMessage(lPlayerID, lPlayersInfo);
        return lPlayerID;
    }

    public void renamePlayer(string pNewPlayerName)
    {
        if (gamePlayers.selfName!= pNewPlayerName)
        {
            if (Network.isServer)
            {
                NetworkRoomRenamePlayer(playerID, pNewPlayerName);
                gamePlayers.selfName = pNewPlayerName;
            }
            else
                networkView.RPC("NetworkRoomRenamePlayer", RPCMode.Server, playerID, pNewPlayerName);
        }
    }

    void renamePlayerMessage(int pPlayerID,string pPreName,string pNewName)
    {
        messageSender("玩家" + pPlayerID + "." + pPreName + "更名为 " + pNewName);
    }

    //server
    [RPC]
    void NetworkRoomRenamePlayer(int pPlayerID,string pPlayerName)
    {
        var lPlayersInfo = gamePlayers.getPlayerInfo(pPlayerID);
        renamePlayerMessage(pPlayerID, lPlayersInfo.playerName, pPlayerName);
        lPlayersInfo.playerName = pPlayerName;
        sendData();
    }

    //server
    [RPC]
    void NetworkRoomRegister(string pPlayerName,NetworkMessageInfo pInfo)
    {
        if(!playerToID.ContainsKey(pInfo.sender))
        {
            var lNewPlayerID = registerPlayer(pPlayerName, pInfo.sender);
            if (lNewPlayerID == -1)
            {
                //房间位已满,断开连接
                Network.CloseConnection(pInfo.sender, true);
            }
            else
            {
                playerEnterMessage(lNewPlayerID, gamePlayers.getPlayerInfo(lNewPlayerID));
                networkView.RPC("NetworkRoomSetPlayerID", pInfo.sender,
                    lNewPlayerID);
                networkView.RPC("NetworkRoomSetMap", pInfo.sender,
                    _mapName);
                sendData();
            }
        }
    }

    //client
    [RPC]
    void NetworkRoomSetPlayerID(int pID)
    {
        playerID = pID;
    }

    void sendData()
    {
        roomDataChangedReceiver();
        int lPlayerSpaceCount = gamePlayers.playerSpaceCount;
        Hashtable[] lPlayerList = new Hashtable[lPlayerSpaceCount];
        for (int i = 0; i < lPlayerSpaceCount; ++i)
        {
            var lPlayerInfo = gamePlayers.getPlayerInfo(i+1);
            Hashtable lPlayerData = new Hashtable();
            if (lPlayerInfo)
            {
                lPlayerData["n"] = lPlayerInfo.playerName;
                lPlayerData["r"] = (int)lPlayerInfo.race;
                if (lPlayerInfo.race != Race.eNone)
                {
                    lPlayerData["si"] = lPlayerInfo.spawnIndex;
                    lPlayerData["rd"] = lPlayerInfo.ready;
                }
            }
            lPlayerList[i] = lPlayerData;
        }
        networkView.RPC("NetworkRoomUpdate", RPCMode.Others,
            zzSerializeString.Singleton.pack(lPlayerList));
    }

    void playerLeaveMessage(int pPlayerID, PlayerElement pPlayerElement)
    {
        messageSender(pPlayerID + "." + pPlayerElement.playerName
        + " 离开房间");
    }

    void playerEnterMessage(int pPlayerID, PlayerElement pPlayerElement)
    {
        messageSender(pPlayerID + "." + pPlayerElement.playerName
        + " 进入房间");
    }

    void selectChangedMessage(int pPlayerID,PlayerElement pPlayerElement)
    {
        string lRaceName = string.Empty;
        if (pPlayerElement.race == Race.ePismire)
            lRaceName = "蚂蚁 ";
        else if (pPlayerElement.race == Race.eBee)
            lRaceName = "蜜蜂 ";
        messageSender(pPlayerID + "." + pPlayerElement.playerName
            + "选择" + lRaceName + (pPlayerElement.spawnIndex+1));
    }

    void playerReadyMessage(int pPlayerID, PlayerElement pPlayerElement)
    {
        messageSender(pPlayerID + "." + pPlayerElement.playerName
        + " 准备就绪");
    }

    void playerUnreadyMessage(int pPlayerID, PlayerElement pPlayerElement)
    {
        messageSender(pPlayerID + "." + pPlayerElement.playerName
        + " 离开准备状态");
    }

    public void makeReady()
    {
        var lPlayerInfo = gamePlayers.selfPlayeInfo;
        if (lPlayerInfo.race == Race.eNone)
            errorMessageSender("请选择种族,否则不能进入准备状态");
        else if (!WMGameConfig.checkMapAvailable(mapName))
        {
            errorMessageSender("缺少当前对战地图:" + mapName);
            errorMessageSender("可通过其他工具向房主索取");
        }
        else if (Network.isServer)
        {
            int lClientCount = 0;
            List<int> lUnreadyIDs = new List<int>();
            for (int lPlayerID = 2;
                lPlayerID <= gamePlayers.playerSpaceCount;
                ++lPlayerID)
            {
                var lClient = gamePlayers.getPlayerInfo(lPlayerID);
                if (lClient)
                {
                    if (!lClient.ready)
                        lUnreadyIDs.Add(lPlayerID);
                    ++lClientCount;
                }
            }
            if (lUnreadyIDs.Count > 0)
            {
                errorMessageSender("玩家:");
                foreach (var lUnreadyID in lUnreadyIDs)
                {
                    errorMessageSender("   " + lUnreadyID
                        + "." + gamePlayers.getPlayerInfo(lUnreadyID).playerName);
                }
                errorMessageSender("未准备就绪,不能开始游戏.");
            }
            else if (lClientCount == 0)
                errorMessageSender("没一个加入,玩单机关卡去吧");
            else
            {
                netGameReadyEvent();
            }
        }
        else
        {
            networkView.RPC("NetworkMakeReady", RPCMode.Server, playerID);
        }
    }

    //in server
    [RPC]
    void NetworkMakeReady(int pPlayerID)
    {
        var lPlayerInfo = gamePlayers.getPlayerInfo(pPlayerID);
        if(lPlayerInfo.race!= Race.eNone
            &&!lPlayerInfo.ready)
        {
            lPlayerInfo.ready = true;
            playerReadyMessage(pPlayerID, lPlayerInfo);
            sendData();
        }
    }

    public void makeUnready()
    {
        var lPlayerInfo = gamePlayers.selfPlayeInfo;
        if (Network.isClient && !lPlayerInfo.ready)
            return;
        else if (Network.isServer)
        {
            netGameInterruptedEvent();
        }
        else
        {
            networkView.RPC("NetworkMakeUnready", RPCMode.Server, playerID);
        }

    }

    //in server
    [RPC]
    void NetworkMakeUnready(int pPlayerID)
    {
        var lPlayerInfo = gamePlayers.getPlayerInfo(pPlayerID);
        if (lPlayerInfo.ready)
        {
            lPlayerInfo.ready = false;
            sendData();
            playerUnreadyMessage(pPlayerID, lPlayerInfo);
            netGameInterruptedEvent();
        }
    }

    public void sendPlayerMessage(string pMessage)
    {
        if (pMessage.Length == 0)
            return;
        NetworkSendPlayerMessage(playerID, pMessage);
        networkView.RPC("NetworkSendPlayerMessage", RPCMode.Others, playerID, pMessage);
    }

    [RPC]
    void NetworkSendPlayerMessage(int pPlayerID, string pMessage)
    {
        var lPlayer = gamePlayers.getPlayerInfo(pPlayerID);
        playerMessageSender(pPlayerID + "." + lPlayer.playerName
        + " 说:" + pMessage);
    }

    [SerializeField]
    bool haveReceiverRoomData = false;

    void playerChanged(int lPlayerID, PlayerElement pLastInfo, PlayerElement pNewInfo)
    {
        if (!pLastInfo && pNewInfo)
            playerEnterMessage(lPlayerID, pNewInfo);
        else if (pLastInfo && !pNewInfo)
            playerLeaveMessage(lPlayerID, pNewInfo);
        else
        {
            if (pLastInfo.race != pNewInfo.race
                || pLastInfo.spawnIndex != pNewInfo.spawnIndex)
                selectChangedMessage(lPlayerID, pNewInfo);
            else if (pLastInfo.playerName != pNewInfo.playerName)
                renamePlayerMessage(lPlayerID, pLastInfo.playerName, pNewInfo.playerName);
            if (!pLastInfo.ready && pNewInfo.ready)
                playerReadyMessage(lPlayerID, pNewInfo);
            else if (pLastInfo.ready && !pNewInfo.ready)
                playerUnreadyMessage(lPlayerID, pNewInfo);
        } 
    }

    //in client
    [RPC]
    void NetworkRoomUpdate(string pData)
    {
        var lPlayerList = (Hashtable[])zzSerializeString.Singleton.unpackToData(pData);
        var lPlayerSpaceCount = gamePlayers.playerSpaceCount;
        for (int i = 0; i < lPlayerSpaceCount; ++i)
        {
            var lPlayerID = i + 1;
            var lLastInfo = gamePlayers.getPlayerInfo(lPlayerID);
            var lPlayerData = lPlayerList[i];
            if (lPlayerData.Count == 0)
            {
                if (lLastInfo)
                {
                    playerLeaveMessage(lPlayerID, lLastInfo);
                    gamePlayers.setPlayerInfo(lPlayerID, null);
                }
                continue;
            }
            var lPlayerInfo = new PlayerElement
            {
                playerName = (string)lPlayerData["n"],
                race = (Race)lPlayerData["r"],
                spawnIndex = 0,
            };
            if (lPlayerInfo.race != Race.eNone)
            {
                lPlayerInfo.spawnIndex = (int)lPlayerData["si"];
                lPlayerInfo.ready = (bool)lPlayerData["rd"];
            }
            if (haveReceiverRoomData)
                playerChanged(lPlayerID, lLastInfo, lPlayerInfo);
            gamePlayers.setPlayerInfo(lPlayerID, lPlayerInfo);
        }
        haveReceiverRoomData = true;
        //var lPlayersInfo =_playersInfo[playerIdToIndex(playerID)];
        //if (!lPlayersInfo)
        //    return;
        //player.race = lPlayersInfo.race;
        //player.playerName = lPlayersInfo.playerName;
        if (gamePlayers.selfPlayeInfo.ready)
            playerReadyEvent();
        else
            playerReadyInterruptedEvent();
        roomDataChangedReceiver();
    }

    //int playerIdToIndex(int pID)
    //{
    //    return pID - 1;
    //}

    //int indexToPlayerId(int pIndex)
    //{
    //    return pIndex + 1;
    //}

    public void selectPismire(int pPos)
    {
        if (pPos < maxMemberCountEachRace)
        {
            if (Network.isServer)
            {
                if (NetworkRoomSelectPismire(playerID, pPos))
                    gamePlayers.selfRace = Race.ePismire;
            }
            else
                networkView.RPC("NetworkRoomSelectPismire", RPCMode.Server, playerID, pPos);
        }
    }

    //in server
    [RPC]
    bool NetworkRoomSelectPismire(int pPlayerID, int pPos)
    {
        if (!isExit(Race.ePismire, pPos))
        {
            var lPlayerInfo = gamePlayers.getPlayerInfo(pPlayerID);
            //和原先的一样
            if (lPlayerInfo.race == Race.ePismire
                && lPlayerInfo.spawnIndex == pPos)
                return false;
            lPlayerInfo.race = Race.ePismire;
            lPlayerInfo.spawnIndex = pPos;
            lPlayerInfo.ready = false;
            selectChangedMessage(pPlayerID, lPlayerInfo);
            sendData();
            netGameInterruptedEvent();
            return true;
        }
        return false;
    }

    public void selectBee(int pPos)
    {
        if (pPos < maxMemberCountEachRace)
        {
            if (Network.isServer)
            {
                if (NetworkRoomSelectBee(playerID, pPos))
                    gamePlayers.selfRace = Race.eBee;
            }
            else
                networkView.RPC("NetworkRoomSelectBee", RPCMode.Server, playerID, pPos);
        }
    }

    //in server
    [RPC]
    bool NetworkRoomSelectBee(int pPlayerID, int pPos)
    {
        //如果此位未被选中
        if (!isExit(Race.eBee, pPos))
        {
            var lPlayerInfo = gamePlayers.getPlayerInfo(pPlayerID);
            //和原先的一样
            if (lPlayerInfo.race == Race.eBee
                && lPlayerInfo.spawnIndex == pPos)
                return false;
            lPlayerInfo.race = Race.eBee;
            lPlayerInfo.spawnIndex = pPos;
            lPlayerInfo.ready = false;
            selectChangedMessage(pPlayerID, lPlayerInfo);
            sendData();
            netGameInterruptedEvent();
            return true;
        }
        return false;
    }

    //public void clearSelection()
    //{
    //}

    //[RPC]
    //void NetworkRoomClearSelection(int pPlayerID)
    //{
    //    //var lPlayerInfo = _playersInfo[playerIdToIndex(pPlayerID)];
    //    //if (lPlayerInfo.race == Race.eBee)
    //    //    beePlayer[lPlayerInfo.spawnIndex] = 0;
    //    //else if (lPlayerInfo.race == Race.eBee)
    //    //    beePlayer[lPlayerInfo.spawnIndex] = 0;
    //    ////sendData();
    //}
}