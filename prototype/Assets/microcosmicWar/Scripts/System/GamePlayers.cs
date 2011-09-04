using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlayers:MonoBehaviour, IEnumerable<PlayerElement>
{
    [SerializeField]
    PlayerInfo player;
    public PlayerListInfo playerListInfo;

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var lPlayer in playerListInfo.players)
        {
            yield return lPlayer;
        }
    }

    IEnumerator<PlayerElement> IEnumerable<PlayerElement>.GetEnumerator()
    {
        foreach (var lPlayer in playerListInfo.players)
        {
            yield return lPlayer;
        }
    }

    public int playerSpaceCount
    {
        get
        {
            return playerListInfo.players.Length;
        }

        set
        {
            System.Array.Resize(ref playerListInfo.players, value);
        }
    }

    public PlayerElement selfPlayeInfo
    {
        get { return playerListInfo.players[playerIdToIndex(selfID)]; }
    }

    public int selfID
    {
        get { return player.playerID; }

        //必须在setPlayerInfoByIndex前执行
        set { player.playerID = value; }
    }

    public string selfName
    {
        get { return player.playerName; }
        set
        {
            player.playerName = value;
            selfPlayeInfo.playerName = value;
        }
    }

    public Race selfRace
    {
        get { return player.race; }
        set
        {
            player.race = value;
            selfPlayeInfo.race = value;
        }
    }

    public PlayerElement getPlayerInfo(int pPlayerID)
    {
        return getPlayerInfoByIndex(playerIdToIndex(pPlayerID));
    }

    public void setPlayerInfo(int pPlayerID, PlayerElement pValue)
    {
        setPlayerInfoByIndex(playerIdToIndex(pPlayerID),pValue);
    }

    int playerIdToIndex(int pID)
    {
        return pID - 1;
    }

    int indexToPlayerId(int pIndex)
    {
        return pIndex + 1;
    }

    PlayerElement getPlayerInfoByIndex(int pIndex)
    {
        return playerListInfo.players[pIndex];
    }

    void setPlayerInfoByIndex(int pIndex,PlayerElement pValue)
    {
        playerListInfo.players[pIndex] = pValue;
        if(playerIdToIndex(selfID)==pIndex)
        {
            player.race = pValue.race;
            player.playerName = pValue.playerName;
        }
    }
}