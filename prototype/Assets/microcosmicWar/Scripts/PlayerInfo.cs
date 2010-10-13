
using UnityEngine;
using System.Collections;

public enum Race
{
    ePismire = 0,
    eBee,
    eNone,
};

public class PlayerInfo : MonoBehaviour
{


    //保存玩家 选队 等信息,以便场景读取

    public static string eRaceToString(Race race)
    {
        switch (race)
        {
            case Race.ePismire: return "pismire";
            case Race.eBee: return "bee";
            //case Race.ePismire: return "pismire";
        }
        Debug.LogError("no the race");
        return "";
    }

    public static Race getAdversaryRace(Race race)
    {
        switch (race)
        {
            case Race.ePismire: return Race.eBee;
            case Race.eBee: return Race.ePismire;
            //case Race.ePismire: return "pismire";
        }
        Debug.LogError("no the race");
        return Race.eNone;
    }

    public static int getRaceLayer(Race race)
    {
        switch (race)
        {
            case Race.eBee: return layers.bee;
            case Race.ePismire: return layers.pismire;
            //case Race.ePismire: return "pismire";
        }
        Debug.LogError(race);
        return 0;
    }

    public static int getAdversaryRaceLayer(Race race)
    {
        return getRaceLayer(getAdversaryRace(race));
    }

    public static int getAdversaryRaceLayer(int raceLayer)
    {
        //switch (raceLayer)
        //{
            //case layers.pismire: return layers.bee;
            //case layers.bee: return layers.pismire;
            //case Race.ePismire: return "pismire";
        //}
        if (raceLayer == layers.pismire)
            return layers.bee;
        else if (raceLayer == layers.bee)
            return layers.pismire;

        Debug.LogError(raceLayer);
        return 0;
    }

    public static Race getRace(int raceLayer)
    {
        if (raceLayer == layers.pismire)
            return Race.ePismire;
        else if (raceLayer == layers.bee)
            return Race.eBee;

        Debug.LogError(raceLayer);
        return Race.eNone;
    }


    public Race race = Race.eNone;

    public string playerName = "player";

    //FIXME_VAR_TYPE teamName="";

    void Awake()
    {
        //if(teamName=="")
        //	teamName=eRaceToString(race);
    }


    //function Awake()
    //{
    //	gameObject.DontDestroyOnLoad ();
    //}
    public void setData(PlayerInfo pOther)
    {
        this.race = pOther.race;
    }

    public void setRace(Race pRace)
    {
        race = pRace;
    }

    public Race getRace()
    {
        return race;
        //return 1;
    }

    public string getRaceName()
    {
        //return race;
        //return 1;
        return eRaceToString(race);
    }

    public void setPlayerName(string pName)
    {
        playerName = pName;
    }

    public string getPlayerName()
    {
        return eRaceToString(race);
    }

    public string getTeamName()
    {
        //return teamName;
        return getRaceName();
    }

    //void  Update (){
    //}
}