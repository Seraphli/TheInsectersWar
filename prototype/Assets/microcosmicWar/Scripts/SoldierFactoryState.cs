
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class SoldierFactoryState : MonoBehaviour
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

    void addSoldierFactory(Race race, string pSoldierName)
    {
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


    public Stronghold canCreate(GameObject pGameObject, Race race, out Vector3 position)
    {
        //Hero hero = pGameObject.GetComponentInChildren<Hero>();
        //int face = (int)hero.getFace();
        //Vector3 position = pGameObject.transform.position;

        //position.x += hero.getFaceDirection() * 3;
        //position.y += 2;
        //RaycastHit lHit;
        //Vector3 position;
        Stronghold lStronghold = null;
        if (defenseTowerItem.canBuild(pGameObject, out position))
        {
            //FIXME_VAR_TYPE lRange= Vector2(0,2,0);
            //Debug.Log(""+(lHit.point+Vector3(0,4,0))+(lHit.point+Vector3(0,0.1f,0)));
            //if(!Physics.CheckCapsule  (lHit.point+Vector3(0,3,0), lHit.point+Vector3(0,-1,0), 0.25f ) )

            Collider[] lIsInSelfZone = Physics.OverlapSphere(position + new Vector3(0, 2, 0),
                0.1f, layers.manorValue);
            if(lIsInSelfZone.Length!=0)
            {
                lStronghold = lIsInSelfZone[0].transform.parent.GetComponent<Stronghold>();
            }
            print(lIsInSelfZone.Length);

            if(
                !lStronghold
                || lStronghold.owner != race

                || lStronghold.soldierFactory
               )
                return null;
            //print(lStronghold.owner);

            //if (!defenseTowerItem.haveBoardOverhead(towerPosition))
            //    return true;
            //return true;

        }
        //Debug.Log(""+position+","+lHit.point);
        return lStronghold;
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
            GameObject lBuilding = zzCreatorUtility.Instantiate(
                soldierFactorySystem.getFactoryPrefab(race), lPosition, Quaternion.identity, 0);
            lBuilding.GetComponent<Life>().addDieCallback(buildingDeadCall);

            GameObject lSign = (GameObject)Instantiate(
                lState.info.signPrefab, lPosition, Quaternion.identity );
            lSign.transform.parent = lBuilding.transform;
            lState.building = lBuilding;
            buildToState[lBuilding] = lState;

            var lFactoryInfo = lState.info;
            zzObjectMap.getObject(lFactoryInfo.armyBaseName)
                .GetComponent<ArmyBase>()
                .addFactory(lFactoryInfo.soldierPrefab, lFactoryInfo.produceInterval);

            lStronghold.setSoldierFactory(lBuilding);

            changedCall();
        }
        else
            Debug.Log("can't createFactory");
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