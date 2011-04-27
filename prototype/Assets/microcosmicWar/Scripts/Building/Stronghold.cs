﻿using UnityEngine;
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

    public int beeCount = 0;
    public int pismireCount = 0;

    /// <summary>
    /// 被占领时,生成的建筑
    /// </summary>
    public GameObject strongholdBuilding;


    public GameObject pismireBuildingPrefab;
    public GameObject beeBuildingPrefab;

    public StrongholdValueShow strongholdValueShow;

    [SerializeField]
    GameObject _soldierFactory;

    public GameObject soldierFactory
    {
        get
        {
            return _soldierFactory;
        }
        set
        {
            if(_soldierFactory && _soldierFactory!=value)
            {
                _soldierFactory.GetComponent<Life>().makeDead();
            }
            _soldierFactory = value;
        }
    }

    public Animation strongholdAnimation;

    public delegate void RaceChangedEvent(Race pRace);

    public RaceChangedEvent raceChangedEvent;

    [System.Serializable]
    public class RaceShow
    {
        public Race race;
        public Material lightMaterial;
        public Material waterMaterial;
    }

    public RaceShow[] raceShowInfo;

    RaceShow[] raceShowData;

    public Animation[] occupyingAnimations;
    public Renderer[] lightRenderers;
    public Renderer[] waterRenderers;

    void toAnimationState(string pAniName)
    {
        var lAniState = strongholdAnimation[pAniName];
        lAniState.time = lAniState.length;
        lAniState.weight = 1f;
        lAniState.enabled = true;
        strongholdAnimation.Sample();
        lAniState.time = 0;
        lAniState.enabled = false;
    }

    void setMaterial(Renderer[] pRenderers, Material pMaterial)
    {
        foreach (var lRenderer in pRenderers)
        {
            lRenderer.material = pMaterial;
        }
    }

    public void updateRaceShow()
    {
        bool lEnableOccupyingAnimations = (owner != Race.eNone);

        foreach (var lAinmation in occupyingAnimations)
        {
            lAinmation.enabled = lEnableOccupyingAnimations;
        }

        var lRaceShow = raceShowData[(int)owner];
        setMaterial(lightRenderers, lRaceShow.lightMaterial);
        setMaterial(waterRenderers, lRaceShow.waterMaterial);
    }

    void Awake()
    {
        raceShowData = new RaceShow[raceShowInfo.Length];
        foreach (var lRaceShow in raceShowInfo)
        {
            raceShowData[(int)lRaceShow.race] = lRaceShow;
        }
    }

    void Start()
    {
        //print("stronghold:"+name);
        if (owner != Race.eNone)
        {
            buildRace(owner);
            toAnimationState("occupied");
        }
        else
        {
            updateRaceShow();
            toAnimationState("lost");
            GameSceneManager.Singleton
                .addObject(GameSceneManager.MapManagerType.stronghold, gameObject);
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
        if(zzCreatorUtility.isHost())
        {
            var lValue = getValue(pCollider);
            //print("OnTriggerEnter:" + lValue.name);
            if (lValue.gameObject.layer == layers.pismire)
                pismireList.Add(lValue);
            else
                beeList.Add(lValue);
        }
    }

    void OnTriggerExit(Collider pCollider)
    {
        if (zzCreatorUtility.isHost())
        {
            var lValue = getValue(pCollider);
            //print("OnTriggerExit:" + lValue.name);
            if (lValue.gameObject.layer == layers.pismire)
                pismireList.Remove(lValue);
            else
                beeList.Remove(lValue);
        }
    }

    void refreshTriggerInfo(HashSet<Transform>  pList)
    {
        var lRemoveList = new List<Transform>();
        foreach (var lTransform in pList)
        {
            if(!collisionLayer.isAliveFullCheck(lTransform))
                lRemoveList.Add(lTransform);
        }
        foreach (var lTransform in lRemoveList)
        {
            pList.Remove(lTransform);
        }
    }

    void refreshTriggerInfo()
    {
        if (zzCreatorUtility.isHost())
        {
            refreshTriggerInfo(pismireList);
            refreshTriggerInfo(beeList);
            refreshDebugInfo();
        }
    }

    Transform   getValue(Collider pCollider)
    {
        return pCollider.transform;
    }

    int getSoldierCount(Race race)
    {
        switch(race)
        {
            case Race.ePismire: return pismireCount;
            case Race.eBee: return beeCount;
        }
        Debug.LogError("getList(Race race)");
        return 0;
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

        refreshTriggerInfo();

        //更新占领所属哪方的图标,从none变为占领
        if (owner == Race.eNone)
        {
            owner = judgeOwner();
            if (owner == Race.eNone)
                return;
            strongholdValueShow.showRace(owner);
        }

        float lOccupiedValueDelta;

        int lSelfOccupantNum = getSoldierCount(owner);
        var lAdversaryRace = PlayerInfo.getAdversaryRace(owner);
        int lEnemyOccupantNum = getSoldierCount(lAdversaryRace);

        int lDeltaOccupantNum = lSelfOccupantNum - lEnemyOccupantNum;
        if (lDeltaOccupantNum <= 0)
            lOccupiedValueDelta = ((float)lDeltaOccupantNum - recoverSpeed) * Time.deltaTime;
        else
            lOccupiedValueDelta = lDeltaOccupantNum * Time.deltaTime;

        nowOccupiedValue += lOccupiedValueDelta;

        //被扭转
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
        if (zzCreatorUtility.isHost())
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

            if (Network.peerType != NetworkPeerType.Disconnected)
                networkView.RPC("RPCBuildRace", RPCMode.Others,
                    strongholdBuilding.networkView.viewID,(int)owner);
            occupiedEvent();
            raceChangedEvent(pRace);
        }

    }

    [RPC]
    public void RPCBuildRace(NetworkViewID pID,int pOwner)
    {
        owner = (Race)pOwner;
        strongholdBuilding = NetworkView.Find(pID).gameObject;
        occupiedEvent();

    }

    public void playOccupiedAimation()
    {
        strongholdAnimation.CrossFade("occupied");
    }

    void occupiedEvent()
    {
        playOccupiedAimation();
        GameSceneManager.Singleton
            .addObject(owner, GameSceneManager.UnitManagerType.stronghold, gameObject);
        var strongholdTransform = transform;
        var lBuildingTransform = strongholdBuilding.transform;

        lBuildingTransform.position = strongholdTransform.position;
        lBuildingTransform.rotation = strongholdTransform.rotation;
        lBuildingTransform.localScale = strongholdTransform.localScale;

        lBuildingTransform.parent = strongholdTransform;
        updateRaceShow();
    }

    public void playLostAnimation()
    {
        strongholdAnimation.CrossFade("lost");
    }

    void lostEvent()
    {
        strongholdBuilding = null;
        playLostAnimation();
        updateRaceShow();
        GameSceneManager.Singleton
            .addObject(GameSceneManager.MapManagerType.stronghold, gameObject);
    }

    [RPC]
    public void RPCBuildingDestroied()
    {
        lostEvent();
    }

    void buildingDestroied()
    {
        //strongholdBuilding = null;
        nowOccupiedValue = 0.0f;
        owner = Race.eNone;

        if (soldierFactory)
            soldierFactory.GetComponent<Life>().makeDead();

        if (Network.peerType != NetworkPeerType.Disconnected)
            networkView.RPC("RPCBuildingDestroied", RPCMode.Others);

        lostEvent();
    }

    /// <summary>
    /// 在owner == Race.eNone;且有士兵的情况下,判定占有者
    /// </summary>
    Race judgeOwner()
    {
        if (pismireCount > beeCount)
            return Race.ePismire;
        else if (pismireCount < beeCount)
            return Race.eBee;
        return Race.eNone;
    }

    void refreshDebugInfo()
    {
        pismireCount = pismireList.Count;
        beeCount = beeList.Count;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int lOwner = 0;
        //---------------------------------------------------
        if (stream.isWriting)
        {
            lOwner = (int)owner;
        }
        //---------------------------------------------------
        stream.Serialize(ref lOwner);
        stream.Serialize(ref nowOccupiedValue);
        stream.Serialize(ref pismireCount); ;
        stream.Serialize(ref beeCount);
        //---------------------------------------------------
        if (stream.isReading)
        {
            owner = (Race)lOwner;
            strongholdValueShow.showRace(owner);
            strongholdValueShow.rate = nowOccupiedValue / fullOccupiedValue;
        }
    }
}