using UnityEngine;

public class Manor:MonoBehaviour
{
    public Race owner = Race.eNone;
    public RestorableValue energyValue;

    public MeshRenderer manorRenderer;
    public Material pismireMaterial;
    public Material beeMaterial;

    //[zzSerialize]
    //public string raceID
    //{
    //    get
    //    {
    //        return PlayerInfo.eRaceToString(race);
    //    }
    //    set
    //    {
    //        race = PlayerInfo.stringToRace(value);
    //    }
    //}

    //[EnumUI(new string[] { "蚂蚁", "蜜蜂" },
    //     new int[] { (int)Race.ePismire, (int)Race.eBee })]
    public Race race
    {
        get
        {
            return owner;
        }

        set
        {
            if (owner == value || value == Race.eNone)
                return;
            if (value == Race.ePismire)
            {
                manorRenderer.material = pismireMaterial;
            }
            else if (value == Race.eBee)
            {
                manorRenderer.material = beeMaterial;
            }
            else
                Debug.LogError("owner == none");
            owner = value;
        }
    }
}