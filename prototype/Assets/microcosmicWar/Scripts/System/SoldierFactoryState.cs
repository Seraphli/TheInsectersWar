
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierFactoryState : MonoBehaviour
{
    SoldierFactorySystem soldierFactorySystem;

    public class State
    {
        public State(SoldierFactorySystem.SoldierInfo pInfo)
        {
            info = pInfo;
            //building = null;
        }

        public SoldierFactorySystem.SoldierInfo info;
        //private GameObject _building;

        //public bool netCanBuild;

        //public GameObject building
        //{
        //    get { return _building; }
        //    set 
        //    { 
        //        _building = value;
        //    }
        //}

        //public bool canBuild()
        //{
        //    return !_building;
        //}
    }

    zzUtilities.voidFunction changedCall = zzUtilities.nullFunction;

    public void setChangedCall(zzUtilities.voidFunction pCall)
    {
        changedCall = pCall;
    }

    Dictionary<Race, List<State>> RaceFactoryState = new Dictionary<Race,List<State>>();
    Dictionary<GameObject, State> buildToState;

    //void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{
    //    //if (stream.isWriting)
    //    //{
    //        foreach (var lRaceFactory in RaceFactoryState)
    //        {
    //            foreach (var lState in lRaceFactory.Value)
    //            {
    //                stream.Serialize(ref lState.netCanBuild);
    //            }
    //        }
    //    //}
    //    ////---------------------------------------------------
    //    //stream.Serialize(ref lOwner);
    //    //stream.Serialize(ref nowOccupiedValue);
    //    //stream.Serialize(ref pismireCount); ;
    //    //stream.Serialize(ref beeCount);
    //    ////---------------------------------------------------
    //    //if (stream.isReading)
    //    //{
    //    //    owner = (Race)lOwner;
    //    //    strongholdValueShow.showRace(owner);
    //    //}
    //}

    public System.Collections.ObjectModel.ReadOnlyCollection<State>
        getFactoryStates(Race race)
    {
        return RaceFactoryState[race].AsReadOnly();
    }

    //public void setBuilding(Race race, int index, GameObject pBuilding)
    //{
    //    State lState = RaceFactoryState[race][index];
    //    lState.building = pBuilding;
    //}

    Dictionary<string, int> soldierNameToStateIndex = new Dictionary<string, int>();

    void addSoldierFactory(Race race, string pSoldierName)
    {
        soldierNameToStateIndex[pSoldierName] = RaceFactoryState[race].Count;
        RaceFactoryState[race].Add(
            new State(soldierFactorySystem.getSoldierInfos(race)
                .getDataByKey(pSoldierName)
                )
            );
    }
    static protected SoldierFactoryState singletonInstance = null;

    public static SoldierFactoryState Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    public static SoldierFactoryState getSingleton()
    {
        return singletonInstance;
    }

    [System.Serializable]
    class SoldierListInfo
    {
        public Race race;
        public string[] soldiers;
    }

    [SerializeField]
    SoldierListInfo[] soldierRace;

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");

        singletonInstance = this;
        RaceFactoryState = new Dictionary<Race, List<State>>();
        buildToState = new Dictionary<GameObject, State>();

        for (int i = 0; i < (int)Race.eNone; ++i)
            RaceFactoryState[(Race)i] = new List<State>();
    }


    void Start()
    {
        soldierFactorySystem = SoldierFactorySystem.getSingleton();

        foreach (var lSoldierRace in soldierRace)
        {
            foreach (var lSoldier in lSoldierRace.soldiers)
            {
                addSoldierFactory(lSoldierRace.race, lSoldier);
            }
        }
    }

    /// <summary>
    /// 是否可以创建兵工厂
    /// </summary>
    /// <param name="pGameObject">英雄</param>
    /// <param name="race"></param>
    /// <param name="position">创建位置</param>
    /// <returns></returns>
    public Stronghold canCreate(GameObject pGameObject, Race race, out Vector3 position)
    {
        if (defenseTowerItem.canBuild(pGameObject, out position))
        {
            ////区域内是否有阵地
            //Collider[] lIsInSelfZone = Physics.OverlapSphere(position, 0.1f, layers.manorValue);
            //if(lIsInSelfZone.Length!=0)
            //{
            //    lStronghold = lIsInSelfZone[0].transform.parent.GetComponent<Stronghold>();
            //}

            //if (
            //    lStronghold
            //    && lStronghold.occupied == true//被占领
            //    && lStronghold.owner == race//属于自己的种族
            //    && !lStronghold.soldierFactory//还未建造兵工厂
            //    )
            //    return lStronghold;
            return canCreate(race, position);

        }
        return null;
    }
    
    public Stronghold canCreate(Race race, Vector3 position)
    {
        return canCreate(race, position,true);
    }

    public Stronghold canCreate(Race race, Vector3 position, bool pCheckoCcupied)
    {
        Stronghold lStronghold = null;
        //区域内是否有阵地
        Collider[] lIsInSelfZone = Physics.OverlapSphere(position, 0.1f, layers.manorValue);
        if (lIsInSelfZone.Length != 0)
        {
            lStronghold = lIsInSelfZone[0].transform.parent.GetComponent<Stronghold>();
        }
        //print(lStronghold.occupied);
        //print(lStronghold.owner);
        if (
            lStronghold
            && lStronghold.owner == race//属于自己的种族
            && (!pCheckoCcupied||lStronghold.occupied == true)//被占领
            //&& !lStronghold.soldierFactory//还未建造兵工厂
            )
            return lStronghold;
        return null;

    }
    public void tryCreateFactory(Race race,int index,GameObject onwer)
    {
        if (zzCreatorUtility.isHost())
            _tryCreateFactory(race, index, onwer);
        else
            networkView.RPC("_RPCTryCreateFactory", RPCMode.Others,
                (int)race,index,onwer.networkView.viewID);
    }

    [RPC]
    void _RPCTryCreateFactory(int race,int index,NetworkViewID owner)
    {
        _tryCreateFactory((Race)race, index, NetworkView.Find(owner).gameObject);
    }

    void _tryCreateFactory(Race race,int index,GameObject owner)
    {
        Vector3 lPosition;
        //var lState = RaceFactoryState[race][index];
        //if(!lState.canBuild())
        //{
        //    Debug.LogError("!lStates[index].canBuild()");
        //    return;
        //}
        Stronghold lStronghold = canCreate(owner, race, out lPosition);

        if (lStronghold)
        {
            createFactory(race, lPosition, index, lStronghold);
        }
        else
            Debug.Log("can't createFactory");
    }

    public int getSoldierIndex(Race pRace, string pSoldierName)
    {
        return soldierNameToStateIndex[pSoldierName];
    }

    public void createFactory(Race race, Vector3 lPosition, string lSoldierName, Stronghold lStronghold)
    {
        createFactory(race, lPosition,
            soldierNameToStateIndex[lSoldierName],
            lStronghold);
    }

    public SoldierFactorySystem.SoldierInfo getSoldierInfo(Race pRace, string pSoldierName)
    {
        var lStateIndex = soldierNameToStateIndex[pSoldierName];
        State lState = RaceFactoryState[pRace][lStateIndex];
        return lState.info;
    }


    public void createFactory(Race race, Vector3 lPosition, int lStateIndex, Stronghold lStronghold)
    {
        State lState = RaceFactoryState[race][lStateIndex];
        GameObject lBuilding = zzCreatorUtility.Instantiate(
            soldierFactorySystem.getFactoryPrefab(race), lPosition, Quaternion.identity, 0);


        var lFactoryInfo = lState.info;
        //zzObjectMap.getObject(lFactoryInfo.armyBaseName)
        //    .GetComponent<ArmyBase>()
        //    .addFactory(lFactoryInfo.soldierPrefab,lFactoryInfo.produceInterval,
        //        lFactoryInfo.firstTimeOffset);

        var lFactory = SoldierFactory.addFactory(lBuilding,
            lFactoryInfo.soldierPrefab,
            lFactoryInfo.produceInterval,
            lFactoryInfo.firstTimeOffset);

        lFactory.listener = lStronghold.GetComponent<SoldierFactoryListener>().interfaceObject;
        lStronghold.soldierFactory = lBuilding;

        decorateFactoryBuild(lBuilding, race, lStateIndex);
    }

    //public GameObject createFactorySign(Race pRace, string pSoldierName)
    //{
    //    var lStateIndex = soldierNameToStateIndex[pSoldierName];
    //    State lState = RaceFactoryState[race][lStateIndex];
    //    GameObject lSign = (GameObject)Instantiate(lState.info.signPrefab);
    //    return lSign;
    //}

    void decorateFactoryBuild(GameObject pBuild, Race race, int lStateIndex)
    {
        _decorateFactoryBuild(pBuild, race, lStateIndex);
        if (Network.peerType != NetworkPeerType.Disconnected)
            networkView.RPC("_RPCDecorateFactoryBuild", RPCMode.Others,
                pBuild.networkView.viewID, (int)race, lStateIndex);
    }

    [RPC]
    void _RPCDecorateFactoryBuild(NetworkViewID pBuildID, int race,int lStateIndex)
    {
        _decorateFactoryBuild(NetworkView.Find(pBuildID).gameObject,
            (Race)race, lStateIndex);
    }

    void _decorateFactoryBuild(GameObject lBuilding, Race race, int lStateIndex)
    {
        //lBuilding.GetComponent<Life>().addDieCallback(buildingDeadCall);
        State lState = RaceFactoryState[race][lStateIndex];
        //lState.building = lBuilding;
        //buildToState[lBuilding] = lState;
        GameObject lSign = (GameObject)Instantiate(
            lState.info.signPrefab, lBuilding.transform.position, Quaternion.identity);
        lSign.transform.parent = lBuilding.transform;
        lSign.transform.localPosition = Vector3.zero;
        changedCall();
    }

    //void buildingDeadCall(Life life)
    //{
    //    var lState = buildToState[life.gameObject];
    //    lState.building = null;

    //    if (zzCreatorUtility.isHost())
    //    {
    //        var lFactoryInfo = lState.info;
    //        zzObjectMap.getObject(lFactoryInfo.armyBaseName)
    //            .GetComponent<ArmyBase>()
    //            .removeFactory(lFactoryInfo.soldierPrefab);
    //    }

    //    changedCall();
    //}
}