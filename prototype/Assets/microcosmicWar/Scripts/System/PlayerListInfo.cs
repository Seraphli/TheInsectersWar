using UnityEngine;

[System.Serializable]
public class PlayerElement
{
    public Race race;
    public string playerName;
    public int spawnIndex = -1;
    public NetworkPlayer networkPlayer;

    public bool isNull
    {
        get { return spawnIndex == -1; }
    }

    public static implicit operator bool(PlayerElement pPlayerElement)
    {
        return pPlayerElement != null && !pPlayerElement.isNull;
    }

    public static bool operator true(PlayerElement pPlayerElement)
    {
        return pPlayerElement != null && !pPlayerElement.isNull;
    }

    public static bool operator false(PlayerElement pPlayerElement)
    {
        return pPlayerElement == null || pPlayerElement.isNull;
    }

    public static bool operator !(PlayerElement pPlayerElement)
    {
        return pPlayerElement == null || pPlayerElement.isNull;
    }

    //public readonly PlayerElement nullElement
    //    = new PlayerElement
    //    {
    //        race = Race.eNone,
    //        playerName = string.Empty,
    //        networkPlayer = null,
    //    };
}

public class PlayerListInfo:MonoBehaviour
{
    //public PlayerElement[] pismirePlayers;
    //public PlayerElement[] beePlayers;
    public PlayerElement[] players;
}