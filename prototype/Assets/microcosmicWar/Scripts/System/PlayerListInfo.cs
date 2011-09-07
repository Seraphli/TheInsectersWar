using UnityEngine;

[System.Serializable]
public class PlayerElement
{
    public Race race = Race.eNone;
    public string playerName;
    public HeroSpawn spawn;
    [SerializeField]
    int _spawnIndex = -1;

    //为了防止Unity的UI更改值,所以用这种形式
    public int spawnIndex
    {
        set { _spawnIndex = value;}
        get { return _spawnIndex; }
    }
    public NetworkPlayer networkPlayer;
    public bool ready = false;

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
    public PlayerElement[] players = new PlayerElement[0]{};
}