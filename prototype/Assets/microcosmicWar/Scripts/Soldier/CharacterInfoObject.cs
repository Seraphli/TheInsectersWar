using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    public WMPurse purse;
    public WMPriceList priceList;
    public WMItemBag itemBag;
    public bool belongToThePlayer;
    public Race race;
    public NetworkPlayer player;
}

public class CharacterInfoObject:MonoBehaviour
{
    public WMPurse purse
    {
        get { return characterInfo.purse; }
    }

    public WMPriceList priceList
    {
        get { return characterInfo.priceList; }
    }

    public WMItemBag itemBag
    {
        get { return characterInfo.itemBag; }
    }

    public bool belongToThePlayer
    {
        get { return characterInfo.belongToThePlayer; }
    }

    public Race race
    {
        get { return characterInfo.race; }
    }

    public CharacterInfo characterInfo = new CharacterInfo();
}