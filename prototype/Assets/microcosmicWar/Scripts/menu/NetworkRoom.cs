using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkRoom : MonoBehaviour
{
    public string guid;
    public PlayerInfo player;
    public int maxMemberCountEachRace = 4;
    Dictionary<NetworkPlayer, string> playerToGuid
        = new Dictionary<NetworkPlayer,string>();
    Dictionary<string, NetworkPlayer> guidToPlayer
        =new Dictionary<string,NetworkPlayer>();

    Dictionary<string, PlayerElement> _playersInfo
        = new Dictionary<string, PlayerElement>();

    public Dictionary<string, PlayerElement>.ValueCollection playersInfo
    {
        get
        {
            return _playersInfo.Values;
        }
    }

    public string[] pismirePlayer;

    [SerializeField]
    public string[] beePlayer;
    //Dictionary<string,int>

    System.Action roomDataChangedReceiver;

    public void addRoomDataChangedReceiver(System.Action pReceiver)
    {
        roomDataChangedReceiver += pReceiver;
    }

    void Awake()
    {
        guid = System.Guid.NewGuid().ToString();
        pismirePlayer = new string[maxMemberCountEachRace];
        beePlayer = new string[maxMemberCountEachRace];
        clearStringArray(pismirePlayer);
        clearStringArray(beePlayer);
    }

    IEnumerator Start()
    {
        while (Network.peerType == NetworkPeerType.Disconnected)
        {
            yield return null;
        }
        if (Network.isServer)
            registerPlayer(guid, player.playerName, Network.player);
        else
            networkView.RPC("NetworkRoomRegister", RPCMode.Server, guid, player.playerName);
    }

    void clearStringArray(string[] pArray)
    {
        for (int i = 0; i < pArray.Length; ++i)
            pArray[i] = string.Empty;
    }

    void registerPlayer(string pGUID, string pPlayerName, NetworkPlayer pPlayer)
    {
        playerToGuid[pPlayer] = pGUID;
        guidToPlayer[pGUID] = pPlayer;
        _playersInfo[pGUID] = new PlayerElement
        {
            playerName = pPlayerName,
            race = Race.eNone,
            networkPlayer = pPlayer,
        };
        sendData();
    }

    [RPC]
    void NetworkRoomRegister(string pGUID,string pPlayerName,NetworkMessageInfo pInfo)
    {
        registerPlayer(pGUID, pPlayerName, pInfo.sender);
    }

    void sendData()
    {
        roomDataChangedReceiver();
        Hashtable lPlayerList = new Hashtable();
        foreach (var lPlayerInfoDic in _playersInfo)
        {
            Hashtable lPlayerData = new Hashtable();
            var lPlayerInfo = lPlayerInfoDic.Value;
            lPlayerData["name"] = lPlayerInfo.playerName;
            lPlayerData["race"] = (int)lPlayerInfo.race;
            if (lPlayerInfo.race != Race.eNone)
                lPlayerData["spawnIndex"] = lPlayerInfo.spawnIndex;
            lPlayerList[lPlayerInfoDic.Key] = lPlayerData;
        }
        networkView.RPC("NetworkRoomUpdate", RPCMode.Others,
            zzSerializeString.Singleton.pack(lPlayerList));
    }

    //in client
    [RPC]
    void NetworkRoomUpdate(string pData)
    {
        Hashtable lPlayerList = (Hashtable)zzSerializeString.Singleton.unpackToData(pData);
        _playersInfo.Clear();
        clearStringArray(pismirePlayer);
        clearStringArray(beePlayer);
        foreach(DictionaryEntry lPlayerInfoDic in lPlayerList)
        {
            var lPlayerData = (Hashtable)lPlayerInfoDic.Value;
            var lPlayGuid = (string)lPlayerInfoDic.Key;
            var lPlayInfo = new PlayerElement
            {
                playerName = (string)lPlayerData["name"],
                race = (Race)lPlayerData["race"],
            };
            if (lPlayInfo.race != Race.eNone)
            {
                lPlayInfo.spawnIndex = (int)lPlayerData["spawnIndex"];
                if (lPlayInfo.race == Race.ePismire)
                    pismirePlayer[lPlayInfo.spawnIndex] = lPlayGuid;
                else
                    beePlayer[lPlayInfo.spawnIndex] = lPlayGuid;
            }
            _playersInfo[lPlayGuid] = lPlayInfo;
        }
        player.race = _playersInfo[guid].race;
        roomDataChangedReceiver();
    }

    public void selectPismire(int pPos)
    {
        if (pPos < maxMemberCountEachRace)
        {
            if (Network.isServer)
            {
                NetworkRoomSelectPismire(guid, pPos);
            }
            else
                networkView.RPC("NetworkRoomSelectPismire", RPCMode.Server, pPos);
        }
    }

    //in server
    [RPC]
    void NetworkRoomSelectPismire(string pPlayerID, int pPos)
    {
        print("NetworkRoomSelectPismire:" + pPlayerID + pPos);
        if (pismirePlayer[pPos].Length == 0)
        {
            var lPlayerInfo = _playersInfo[pPlayerID];
            if (lPlayerInfo.race == Race.ePismire
                && lPlayerInfo.spawnIndex == pPos)
                return;
            {
                if (lPlayerInfo.race == Race.ePismire)
                    pismirePlayer[lPlayerInfo.spawnIndex] = string.Empty;
                else if(lPlayerInfo.race == Race.eBee)
                    beePlayer[lPlayerInfo.spawnIndex] = string.Empty;
            }
            pismirePlayer[pPos] = pPlayerID;
            var lPlayersInfo = _playersInfo[pPlayerID];
            lPlayersInfo.race = Race.ePismire;
            lPlayersInfo.spawnIndex = pPos;
            sendData();
        }
    }

    public void selectBee(int pPos)
    {
    }

    public void clearSelection()
    {
    }
}