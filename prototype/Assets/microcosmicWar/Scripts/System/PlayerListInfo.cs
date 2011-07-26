using UnityEngine;

public class PlayerElement
{
    public Race race;
    public string playerName;
    public int spawnIndex;
    public NetworkPlayer networkPlayer;
}

public class PlayerListInfo:MonoBehaviour
{
    public PlayerElement[] pismirePlayers;
    public PlayerElement[] beePlayers;
    public PlayerElement[] players;
}