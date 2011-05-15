using UnityEngine;

public class StrongholdObject : zzEditableObject
{
    [SerializeField]
    Race _race;

    public Stronghold stronghold;

    public RestorableValue energyValue;

    public zzSceneTextureNumber energyNumber;

    [SerializeField]
    int _energy = 20;

    [SerializeField]
    int minEnergy = 0;

    [SerializeField]
    int maxEnergy = 999;

    [zzSerialize]
    [FieldUI("能量", verticalDepth=-1)]
    public int energy
    {
        get 
        { 
            return _energy; 
        }
        set
        {
            _energy = Mathf.Clamp(value, minEnergy, maxEnergy); 
            energyValue.fullValue = _energy;
            energyNumber.number = _energy;
        }
    }

    [LabelUI( horizontalDepth=0)]
    public const string chooseLabel = "所属种族:";

    [EnumUI(new string[] { "无", "蚂蚁", "蜜蜂" } ,
         new int[] { (int)Race.eNone, (int)Race.ePismire, (int)Race.eBee })]
    public Race race
    {
        get 
        { 
            return _race; 
        }

        set 
        {
            if (_race == value)
                return;

            stronghold.owner = value;
            if (value == Race.eNone)
            {
                stronghold.playLostAnimation();
            }
            else if ( _race == Race.eNone)
            {
                stronghold.playOccupiedAimation();
            }
            stronghold.updateRaceShow();
            _race = value;
        }
    }

    [zzSerialize]
    public string raceID
    {
        get
        {
            return PlayerInfo.eRaceToString(race);
        }
        set
        {
            race = PlayerInfo.stringToRace(value);
        }
    }

    void Awake()
    {
        stronghold.owner = _race;
    }


    //public override void applyPlayState()
    //{
    //    print("StrongholdObject:"+_race);
    //    if(_race!= Race.eNone)
    //    {
    //        var lTransform = stronghold.transform;
    //        var lColliders = Physics.OverlapSphere(lTransform.position,
    //            21f, PlayerInfo.getBuildingLayer(race));
    //        //print(lTransform.position);
    //        //print(lTransform.localScale.x);
    //        //print(PlayerInfo.getBuildingLayer(race));
    //        print("lColliders.Length" + lColliders.Length);
    //        foreach (var lCollider in lColliders)
    //        {
    //            var lFactoryObject = lCollider.transform.parent
    //                .GetComponent<SingleSoldierFactoryObject>();
    //            if (lFactoryObject)
    //            {
    //                lFactoryObject.factory.listener = stronghold
    //                    .GetComponent<SoldierFactoryListener>().interfaceObject;
    //                stronghold.strongholdBuilding = lFactoryObject.gameObject;
    //                break;
    //            }
    //        }
    //    }
    //}

}