
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
            building = null;
        }

        public SoldierFactorySystem.SoldierInfo info;
        public GameObject building;

        public bool canBuild()
        {
            return !building;
        }
    }

    zzUtilities.voidFunction changedCall = zzUtilities.nullFunction;

    public void setChangedCall(zzUtilities.voidFunction pCall)
    {
        changedCall = pCall;
    }

    Dictionary<Race, List<State>> RaceFactoryState = new Dictionary<Race,List<State>>();
    Dictionary<GameObject, State> buildToState;

    public System.Collections.ObjectModel.ReadOnlyCollection<State>
        getFactoryStates(Race race)
    {
        return RaceFactoryState[race].AsReadOnly();
    }

    public void setBuilding(Race race, int index, GameObject pBuilding)
    {
        State lState = RaceFactoryState[race][index];
        lState.building = pBuilding;
    }

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
        Stronghold lStronghold = null;
        if (defenseTowerItem.canBuild(pGameObject, out position))
        {
            //区域内是否有阵地
            Collider[] lIsInSelfZone = Physics.OverlapSphere(position, 0.1f, layers.manorValue);
            if(lIsInSelfZone.Length!=0)
            {
                lStronghold = lIsInSelfZone[0].transform.parent.GetComponent<Stronghold>();
            }

            if (
                lStronghold
                && lStronghold.occupied == true//被占领
                && lStronghold.owner == race//属于自己的种族
                && !lStronghold.soldierFactory//还未建造兵工厂
                )
                return lStronghold;

            //if(
            //    !lStronghold
            //    || lStronghold.owner != race

            //    || lStronghold.soldierFactory
            //   )
            //    return null;

        }
        return null;
    }

    public void createFactory(Race race,int index,GameObject onwer)
    {
        Vector3 lPosition;
        var lState = RaceFactoryState[race][index];
        if(!lState.canBuild())
        {
            Debug.LogError("!lStates[index].canBuild()");
            return;
        }
        Stronghold lStronghold = canCreate(onwer, race, out lPosition);

        if (lStronghold)
        {
            createFactory(race, lPosition, lState, lStronghold);
        }
        else
            Debug.Log("can't createFactory");
    }

    public void createFactory(Race race, Vector3 lPosition, string lSoldierName, Stronghold lStronghold)
    {
        createFactory(race, lPosition,
            RaceFactoryState[race][soldierNameToStateIndex[lSoldierName]],
            lStronghold);
    }

    public void createFactory(Race race, Vector3 lPosition, State lState, Stronghold lStronghold)
    {
        GameObject lBuilding = zzCreatorUtility.Instantiate(
            soldierFactorySystem.getFactoryPrefab(race), lPosition, Quaternion.identity, 0);
        lBuilding.GetComponent<Life>().addDieCallback(buildingDeadCall);

        GameObject lSign = (GameObject)Instantiate(
            lState.info.signPrefab, lPosition, Quaternion.identity);
        lSign.transform.parent = lBuilding.transform;
        lState.building = lBuilding;
        buildToState[lBuilding] = lState;

        var lFactoryInfo = lState.info;
        zzObjectMap.getObject(lFactoryInfo.armyBaseName)
            .GetComponent<ArmyBase>()
            .addFactory(lFactoryInfo.soldierPrefab,lFactoryInfo.produceInterval,
                lFactoryInfo.firstTimeOffset);

        lStronghold.setSoldierFactory(lBuilding);

        changedCall();
    }

    void buildingDeadCall(Life life)
    {
        var lState = buildToState[life.gameObject];
        lState.building = null;


        var lFactoryInfo = lState.info;
        zzObjectMap.getObject(lFactoryInfo.armyBaseName)
            .GetComponent<ArmyBase>()
            .removeFactory(lFactoryInfo.soldierPrefab);

        changedCall();
    }
}