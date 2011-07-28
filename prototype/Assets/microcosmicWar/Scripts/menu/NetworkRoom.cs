using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkRoom : MonoBehaviour
{
    public int playerID;
    public string guid;
    public PlayerInfo player;
    public PlayerListInfo playerListInfo;
    public int maxMemberCountEachRace = 4;
    public int maxPlayerCount = 4;
    Dictionary<NetworkPlayer, int> playerToID
        = new Dictionary<NetworkPlayer, int>();
    System.Action<string> messageSender;

    public void addMessageReceiver(System.Action<string> pReceiver)
    {
        messageSender += pReceiver;
    }

    static void nullMessageReceiver(string p){}
    //Dictionary<string, NetworkPlayer> guidToPlayer
    //    =new Dictionary<string,NetworkPlayer>();

    //Dictionary<string, PlayerElement> _playersInfo
    //    = new Dictionary<string, PlayerElement>();
    [SerializeField]
    PlayerElement[] _playersInfo
    {
        get
        {
            return playerListInfo.players;
        }
        set
        {
            playerListInfo.players = value;
        }
    }

    public PlayerElement[]  playersInfo
    {
        get
        {
            return playerListInfo.players;
        }
    }

    bool isExit(Race pRace, int pSpawnIndex)
    {
        foreach (var lPlayerInfo in playersInfo)
        {
            if (lPlayerInfo != null
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
        //guid = System.Guid.NewGuid().ToString();
        //pismirePlayer = new int[maxMemberCountEachRace];
        //beePlayer = new int[maxMemberCountEachRace];
        _playersInfo = new PlayerElement[maxPlayerCount];
        //clearIntArray(pismirePlayer);
        //clearIntArray(beePlayer);
    }

    IEnumerator Start()
    {
        if (messageSender == null)
            messageSender = nullMessageReceiver;

        while (Network.peerType == NetworkPeerType.Disconnected)
        {
            yield return null;
        }
        if (Network.isServer)
        {
            playerID = registerPlayer(player.playerName, Network.player);
            sendData();
        }
        else
            networkView.RPC("NetworkRoomRegister", RPCMode.Server, player.playerName);
    }

    void OnPlayerDisconnected(NetworkPlayer pPlayer)
    {
        int lPlayerId;
        if (!playerToID.TryGetValue(pPlayer, out lPlayerId))
            return;
        int lPlayerIdIndex = playerIdToIndex(lPlayerId);
        playerLeaveMessage(lPlayerId, playersInfo[lPlayerIdIndex]);
        //messageSender("玩家断开连接: " + lPlayerId + "."
        //    + playersInfo[lPlayerIdIndex].playerName);
        //print("OnPlayerDisconnected id:" + lPlayerId + " name:"
        //    + _playersInfo[lPlayerIdIndex].playerName);
        Network.RemoveRPCs(pPlayer);
        Network.DestroyPlayerObjects(pPlayer);
        //NetworkRoomClearSelection(lPlayerId);
        playerToID.Remove(pPlayer);
        _playersInfo[lPlayerIdIndex] = null;
        sendData();
    }


    void clearIntArray(int[] pArray)
    {
        System.Array.Clear(pArray, 0, pArray.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pPlayerName"></param>
    /// <param name="pPlayer"></param>
    /// <returns>player ID</returns>
    int registerPlayer(string pPlayerName, NetworkPlayer pPlayer)
    {
        int i = 0;
        while (_playersInfo[i] != null)
            ++i;
        int lPlayerID = indexToPlayerId(i);
        playerToID[pPlayer] = lPlayerID;
        //guidToPlayer[pGUID] = pPlayer;
        var lPlayersInfo= new PlayerElement
        {
            playerName = pPlayerName,
            race = Race.eNone,
            networkPlayer = pPlayer,
            spawnIndex = 0,
        };
        _playersInfo[i] = lPlayersInfo;
        playerEnterMessage(lPlayerID, lPlayersInfo);
        return lPlayerID;
    }

    public void renamePlayer(string pNewPlayerName)
    {
        if(player.playerName!=pNewPlayerName)
        {
            if (Network.isServer)
            {
                NetworkRoomRenamePlayer(playerID, pNewPlayerName);
                player.playerName = pNewPlayerName;
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
        var lPlayersInfo = _playersInfo[playerIdToIndex(pPlayerID)];
        renamePlayerMessage(pPlayerID, lPlayersInfo.playerName, pPlayerName);
        lPlayersInfo.playerName = pPlayerName;
        //playerEnterMessage(lID, _playersInfo[lID]);
        sendData();
    }

    //server
    [RPC]
    void NetworkRoomRegister(string pPlayerName,NetworkMessageInfo pInfo)
    {
        var lNewPlayerID = registerPlayer(pPlayerName, pInfo.sender);
        playerEnterMessage(lNewPlayerID, _playersInfo[playerIdToIndex(lNewPlayerID)]);
        networkView.RPC("NetworkRoomSetPlayerID",pInfo.sender,
            lNewPlayerID);
        sendData();
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
        Hashtable[] lPlayerList = new Hashtable[_playersInfo.Length];
        for (int i = 0; i < _playersInfo.Length;++i )
        {
            var lPlayerInfo = _playersInfo[i];
            Hashtable lPlayerData = new Hashtable();
            if (lPlayerInfo)
            {
                lPlayerData["n"] = lPlayerInfo.playerName;
                lPlayerData["r"] = (int)lPlayerInfo.race;
                if (lPlayerInfo.race != Race.eNone)
                    lPlayerData["si"] = lPlayerInfo.spawnIndex;
            }
            lPlayerList[i] = lPlayerData;
        }
        //foreach (var lPlayerInfoDic in _playersInfo)
        //{
        //    Hashtable lPlayerData = new Hashtable();
        //    var lPlayerInfo = lPlayerInfoDic.Value;
        //    lPlayerData["name"] = lPlayerInfo.playerName;
        //    lPlayerData["race"] = (int)lPlayerInfo.race;
        //    if (lPlayerInfo.race != Race.eNone)
        //        lPlayerData["spawnIndex"] = lPlayerInfo.spawnIndex;
        //    lPlayerList[lPlayerInfoDic.Key] = lPlayerData;
        //}
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

    //in client
    [RPC]
    void NetworkRoomUpdate(string pData)
    {
        var lPlayerList = (Hashtable[])zzSerializeString.Singleton.unpackToData(pData);
        //System.Array.Clear(_playersInfo, 0, _playersInfo.Length);
        //var lPrePlayersInfo = _playersInfo;
        //_playersInfo = new PlayerElement[lPrePlayersInfo.Length];
        //clearIntArray(pismirePlayer);
        //clearIntArray(beePlayer);
        for (int i = 0; i < lPlayerList.Length;++i )
        {
            var lLastInfo = _playersInfo[i];
            var lPlayerData = lPlayerList[i];
            var lPlayerID = indexToPlayerId(i);
            if (lPlayerData.Count == 0)
            {
                if (lLastInfo)
                {
                    playerLeaveMessage(lPlayerID, lLastInfo);
                    _playersInfo[i] = null;
                }
                continue;
            }

            var lPlayInfo = new PlayerElement
            {
                playerName = (string)lPlayerData["n"],
                race = (Race)lPlayerData["r"],
            };
            if (lPlayInfo.race != Race.eNone)
            {
                lPlayInfo.spawnIndex = (int)lPlayerData["si"];
            }
            if (!lLastInfo && lPlayInfo)
                playerEnterMessage(lPlayerID, lPlayInfo);
            else if (lLastInfo.race != lPlayInfo.race
                ||lLastInfo.spawnIndex != lPlayInfo.spawnIndex)
                selectChangedMessage(lPlayerID, lPlayInfo);
            else if (lLastInfo.playerName != lPlayInfo.playerName)
                renamePlayerMessage(lPlayerID, lLastInfo.playerName, lPlayInfo.playerName);
            _playersInfo[i] = lPlayInfo;
        }
        //foreach(DictionaryEntry lPlayerInfoDic in lPlayerList)
        //{
        //    var lPlayerData = (Hashtable)lPlayerInfoDic.Value;
        //    var lPlayGuid = (string)lPlayerInfoDic.Key;
        //    var lPlayInfo = new PlayerElement
        //    {
        //        playerName = (string)lPlayerData["name"],
        //        race = (Race)lPlayerData["race"],
        //    };
        //    if (lPlayInfo.race != Race.eNone)
        //    {
        //        lPlayInfo.spawnIndex = (int)lPlayerData["spawnIndex"];
        //        if (lPlayInfo.race == Race.ePismire)
        //            pismirePlayer[lPlayInfo.spawnIndex] = lPlayGuid;
        //        else
        //            beePlayer[lPlayInfo.spawnIndex] = lPlayGuid;
        //    }
        //    _playersInfo[lPlayGuid] = lPlayInfo;
        //}
        var lPlayersInfo =_playersInfo[playerIdToIndex(playerID)];
        player.race = lPlayersInfo.race;
        player.playerName = lPlayersInfo.playerName;
        roomDataChangedReceiver();
    }

    int playerIdToIndex(int pID)
    {
        return pID - 1;
    }

    int indexToPlayerId(int pIndex)
    {
        return pIndex + 1;
    }

    public void selectPismire(int pPos)
    {
        if (pPos < maxMemberCountEachRace)
        {
            if (Network.isServer)
            {
                NetworkRoomSelectPismire(playerID, pPos);
            }
            else
                networkView.RPC("NetworkRoomSelectPismire", RPCMode.Server, playerID, pPos);
        }
    }

    //in server
    [RPC]
    void NetworkRoomSelectPismire(int pPlayerID, int pPos)
    {
        //print("NetworkRoomSelectPismire:" + pPlayerID + pPos);
        if (!isExit(Race.ePismire, pPos))
        {
            var lPlayerInfo = _playersInfo[playerIdToIndex(pPlayerID)];
            //和原先的一样
            if (lPlayerInfo.race == Race.ePismire
                && lPlayerInfo.spawnIndex == pPos)
                return;
            //{
                //if (lPlayerInfo.race == Race.ePismire)
                //    pismirePlayer[lPlayerInfo.spawnIndex] = 0;
                //else if(lPlayerInfo.race == Race.eBee)
                //    beePlayer[lPlayerInfo.spawnIndex] = 0;
            //}
            //pismirePlayer[pPos] = pPlayerID;
            //var lPlayerInfo = _playersInfo[playerIdToIndex(pPlayerID)];
            lPlayerInfo.race = Race.ePismire;
            lPlayerInfo.spawnIndex = pPos;
            selectChangedMessage(pPlayerID, lPlayerInfo);
            sendData();
        }
    }

    public void selectBee(int pPos)
    {
        if (pPos < maxMemberCountEachRace)
        {
            if (Network.isServer)
            {
                NetworkRoomSelectBee(playerID, pPos);
            }
            else
                networkView.RPC("NetworkRoomSelectBee", RPCMode.Server, playerID, pPos);
        }
    }

    //in server
    [RPC]
    void NetworkRoomSelectBee(int pPlayerID, int pPos)
    {
        //print("NetworkRoomSelectBee:" + pPlayerID + pPos);
        if (!isExit(Race.eBee, pPos))
        {
            var lPlayerInfo = _playersInfo[playerIdToIndex(pPlayerID)];
            //和原先的一样
            if (lPlayerInfo.race == Race.eBee
                && lPlayerInfo.spawnIndex == pPos)
                return;
            //{
            //    if (lPlayerInfo.race == Race.eBee)
            //        beePlayer[lPlayerInfo.spawnIndex] = 0;
            //    else if (lPlayerInfo.race == Race.eBee)
            //        beePlayer[lPlayerInfo.spawnIndex] = 0;
            //}
            //beePlayer[pPos] = pPlayerID;
            //var lPlayersInfo = _playersInfo[playerIdToIndex(pPlayerID)];
            lPlayerInfo.race = Race.eBee;
            lPlayerInfo.spawnIndex = pPos;
            selectChangedMessage(pPlayerID, lPlayerInfo);
            sendData();
        }
    }

    public void clearSelection()
    {
    }

    //[RPC]
    void NetworkRoomClearSelection(int pPlayerID)
    {
        //var lPlayerInfo = _playersInfo[playerIdToIndex(pPlayerID)];
        //if (lPlayerInfo.race == Race.eBee)
        //    beePlayer[lPlayerInfo.spawnIndex] = 0;
        //else if (lPlayerInfo.race == Race.eBee)
        //    beePlayer[lPlayerInfo.spawnIndex] = 0;
        ////sendData();
    }
}