using UnityEngine;

public class GameMessageBox:MonoBehaviour
{
    public PlayerInfo player;
    public PlayerListInfo playerListInfo;
    System.Action<string> playerBoxMessageSender;
    System.Action<string> playerBubbleMessageSender;

    int playerID
    {
        get { return player.playerID; }
    }

    int playerIdToIndex(int pID)
    {
        return pID - 1;
    }

    int indexToPlayerId(int pIndex)
    {
        return pIndex + 1;
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
        //var lPlayer = _playersInfo[playerIdToIndex(pPlayerID)];

        //playerBoxMessageSender(string.Format("{0}.{1} 说:{3}",
        //    pPlayerID, lPlayer.playerName, pMessage));
    }
}