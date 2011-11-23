using UnityEngine;

public class GameMessageBox:MonoBehaviour
{
    public GamePlayers gamePlayers;
    System.Action<string> playerBoxMessageSender;
    System.Action<string> playerBubbleMessageSender;

    public void addPlayerBoxMessageReceiver(System.Action<string> pReceiver)
    {
        playerBoxMessageSender += pReceiver;
    }

    public void sendPlayerMessage(string pMessage)
    {
        if (pMessage.Length == 0)
            return;
        sendPlayerMessage(gamePlayers.selfID, pMessage);
        if(Network.peerType!= NetworkPeerType.Disconnected)
            networkView.RPC("NetworkSendPlayerMessage", RPCMode.Others, gamePlayers.selfID, pMessage);
    }

    void sendPlayerMessage(int pPlayerID, string pMessage)
    {
        var lPlayerInfo = gamePlayers.getPlayerInfo(pPlayerID);
        //只发送己方的信息
        if (!gamePlayers.isEnemy(pPlayerID))
        {
            playerBoxMessageSender(string.Format("[{0}.{1}]说:{2}",
                pPlayerID, lPlayerInfo.playerName, pMessage));
        }
        lPlayerInfo.spawn.writeBubbleMessage(string.Format("{0}:{1}",
                lPlayerInfo.playerName, pMessage));
    }

    [RPC]
    void NetworkSendPlayerMessage(int pPlayerID, string pMessage)
    {
        sendPlayerMessage(pPlayerID, pMessage);
    }
}