using UnityEngine;

public class StrongholdObject : zzEditableObject
{
    [SerializeField]
    Race _race;

    //使用Stronghold或Manor进行所属族的设置
    public Stronghold stronghold;

    public Manor manor;

    public RestorableValue energyValue;

    public zzSceneTextureNumber energyNumber;

    [SerializeField]
    int _energy = 20;

    [SerializeField]
    int minEnergy = 0;

    [SerializeField]
    int maxEnergy = 999;

    public Transform buildingZone;

    [SerializeField]
    float maxBuildingZoneSize = 30f;

    [SerializeField]
    float minBuildingZoneSize = 5f;

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

    public bool canBeNone = true;

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
            if (_race == value 
                ||(!canBeNone && value== Race.eNone))
                return;

            if (manor)
                manor.race = value;
            if (stronghold)
            {
                stronghold.owner = value;
                if (value == Race.eNone)
                {
                    stronghold.playLostAnimation();
                }
                else if (_race == Race.eNone)
                {
                    stronghold.playOccupiedAimation();
                }
                stronghold.updateRaceShow();

            }
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

    [SerializeField]
    bool _defaultBuildingZone = true;

    [zzSerialize]
    [FieldUI("使用默认范围", verticalDepth = 2)]
    public bool defaultBuildingZone
    {
        get { return _defaultBuildingZone; }
        set 
        { 
            _defaultBuildingZone = value;
            updateBuildingZoneSize();
        }
    }

    [SerializeField]
    float _buildingZoneSize = 20f;

    [zzSerialize]
    [FieldUI("领地范围", verticalDepth = 3)]
    public float buildingZoneSize
    {
        get 
        { return _buildingZoneSize; }
        set 
        {
            _buildingZoneSize = Mathf.Clamp(value, minBuildingZoneSize, maxBuildingZoneSize);
            updateBuildingZoneSize();
        }
    }

    void updateBuildingZoneSize()
    {
        if(!_defaultBuildingZone)
        {
            var lSize = new Vector3(_buildingZoneSize, _buildingZoneSize, 1);
            buildingZone.localScale = lSize;
        }
    }

    [SliderUI(0f, 1f, verticalDepth = 4)]
    public float buildingZoneSizeRate
    {
        get
        {
            return Mathf.InverseLerp(minBuildingZoneSize, maxBuildingZoneSize, buildingZoneSize);
        }
        set 
        {
            buildingZoneSize = Mathf.Lerp(minBuildingZoneSize, maxBuildingZoneSize, value); 
        }
    }

    void Awake()
    {
        if (stronghold)
            stronghold.owner = _race;
        if (manor)
            manor.race = _race;
        energy = _energy;
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