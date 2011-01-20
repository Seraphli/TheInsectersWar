using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stronghold:MonoBehaviour
{

    public HashSet<Transform> beeList = new HashSet<Transform>();
    public HashSet<Transform> pismireList = new HashSet<Transform>();
    //public Hashtable enemyList = new Hashtable();

    public float fullOccupiedValue = 50.0f;
    public float nowOccupiedValue = 0.0f;

    /// <summary>
    /// 兵离开后,恢复成0的速度
    /// </summary>
    public float recoverSpeed = 5.0f;

    //[SerializeField]
    //Race _owner = Race.eNone;

    public Race owner = Race.eNone;

    public int beeNum = 0;
    public int pismireNum = 0;

    /// <summary>
    /// 被占领时,生成的建筑
    /// </summary>
    public GameObject strongholdBuilding;


    public GameObject pismireBuildingPrefab;
    public GameObject beeBuildingPrefab;

    public StrongholdValueShow strongholdValueShow;

    public GameObject soldierFactory;

    void Start()
    {
        if (owner!=Race.eNone)
        {
            buildRace(owner);
        }
    }

    public void setSoldierFactory(GameObject pSoldierFactory)
    {
        soldierFactory = pSoldierFactory;
    }

    GameObject  getBuilding(Race race)
    {
        switch (race)
        {
            case Race.ePismire: return pismireBuildingPrefab;
            case Race.eBee: return beeBuildingPrefab;
        }
        Debug.LogError("getList(Race race)");
        return null;
    }

    void OnTriggerEnter(Collider pCollider)
    {
        var lValue = getValue(pCollider);
        //print("OnTriggerEnter:" + lValue.name);
        if (lValue.gameObject.layer == layers.pismire)
            pismireList.Add(lValue);
        else
            beeList.Add(lValue);
        refreshDebugInfo();
    }

    void OnTriggerExit(Collider pCollider)
    {
        var lValue = getValue(pCollider);
        //print("OnTriggerExit:" + lValue.name);
        if (lValue.gameObject.layer == layers.pismire)
            pismireList.Remove(lValue);
        else
            beeList.Remove(lValue);
        refreshDebugInfo();
    }

    Transform   getValue(Collider pCollider)
    {
        return pCollider.transform;
    }

    HashSet<Transform> getList(Race race)
    {
        switch(race)
        {
            case Race.ePismire: return pismireList;
            case Race.eBee: return beeList;
        }
        Debug.LogError("getList(Race race)");
        return null;
    }

    void Update()
    {
        //if(
        //    owner==Race.eNone
        //    &&beeList.Count==0
        //    &&pismireList.Count==0)
        //    return;
        if (occupied)
            return;

        if (owner == Race.eNone)
        {
            owner = judgeOwner();
            if (owner == Race.eNone)
                return;
            strongholdValueShow.showRace(owner);
        }

        float lOccupiedValueDelta;

        int lSelfOccupantNum = getList(owner).Count;
        var lAdversaryRace = PlayerInfo.getAdversaryRace(owner);
        int lEnemyOccupantNum = getList(lAdversaryRace).Count;

        int lDeltaOccupantNum = lSelfOccupantNum - lEnemyOccupantNum;
        if (lDeltaOccupantNum <= 0)
            lOccupiedValueDelta = ((float)lDeltaOccupantNum - recoverSpeed) * Time.deltaTime;
        else
            lOccupiedValueDelta = lDeltaOccupantNum * Time.deltaTime;

        nowOccupiedValue += lOccupiedValueDelta;

        if (nowOccupiedValue < 0)
        {
            nowOccupiedValue = 0.0f;
            if (lSelfOccupantNum == lEnemyOccupantNum)
            {
                owner = Race.eNone;
            }
            else
            {
                owner = lAdversaryRace;
                //nowOccupiedValue = -nowOccupiedValue;
            }

            strongholdValueShow.showRace(owner);
        }
        else if (nowOccupiedValue > fullOccupiedValue)
        {
            buildRace(owner);
        }
        else
            strongholdValueShow.rate = nowOccupiedValue/fullOccupiedValue;
    }

    public bool occupied
    {
        get { return strongholdBuilding; }
    }

    public void buildRace(Race pRace)
    {
        if (occupied)
        {
            Debug.LogError("buildRace occupied");
            return;
        }
        strongholdBuilding = zzCreatorUtility
            .Instantiate(getBuilding(pRace), transform.position, transform.rotation, 0);
        sendMessageWhenDie lSendMessageWhenDie
            = strongholdBuilding.GetComponent<sendMessageWhenDie>();
        lSendMessageWhenDie.messageReceiver = gameObject;
        strongholdValueShow.showRace(Race.eNone);
        owner = pRace;

    }

    void buildingDestroied()
    {
        strongholdBuilding = null;
        nowOccupiedValue = 0.0f;
        owner = Race.eNone;

        if (soldierFactory)
            soldierFactory.GetComponent<Life>().makeDead();
    }

    /// <summary>
    /// 在owner == Race.eNone;且有士兵的情况下,判定占有者
    /// </summary>
    Race judgeOwner()
    {
        if (pismireList.Count > beeList.Count)
            return Race.ePismire;
        else if (pismireList.Count < beeList.Count)
            return Race.eBee;
        return Race.eNone;
    }

    void refreshDebugInfo()
    {
        pismireNum = pismireList.Count;
        beeNum = beeList.Count;
    }
}