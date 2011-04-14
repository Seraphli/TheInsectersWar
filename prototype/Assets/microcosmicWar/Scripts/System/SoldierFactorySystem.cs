
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierFactorySystem:MonoBehaviour
{
    [System.Serializable]
    public class SoldierInfo
    {
        public string name;
        public string showName;
        public Texture image;
        public GameObject soldierPrefab;
        public float produceInterval;
        public float firstTimeOffset;
        public Texture activeImage;
        public Texture inactivityImage;
        public GameObject signPrefab;

        [System.NonSerialized]
        public string armyBaseName;
    }

    [System.Serializable]
    class RaceSoldierInfo
    {
        public Race race;
        public GameObject factoryBuildingPrefab;
        public string armyBaseName;
        public SoldierInfo[] SoldierInfoes;
    }

    [SerializeField]
    RaceSoldierInfo[] info;

    public SoldierInfo[] getSoldierInfoList(Race pRace)
    {
        foreach (var lRaceSoldierInfo in info)
        {
            if (lRaceSoldierInfo.race == pRace)
                return lRaceSoldierInfo.SoldierInfoes;
        }
        return null;
    }

    Dictionary<Race, GameObject> factoryPrefabs;

    void    setFactoryPrefabs(Race race, GameObject prefab)
    {
        factoryPrefabs[race] = prefab;
    }

    Dictionary<Race, zzGenericIndexTable<string,SoldierInfo>> raceToSoldierInfos;

    public zzGenericIndexTable<string,SoldierInfo> getSoldierInfos(Race race)
    {
        return raceToSoldierInfos[race];
    }

    public GameObject   getFactoryPrefab(Race race)
    {
        return factoryPrefabs[race];
    }
    
    public static SoldierFactorySystem Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    static protected SoldierFactorySystem singletonInstance = null;

    public static SoldierFactorySystem getSingleton()
    {
        return singletonInstance;
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        createData();
    }

    void createData()
    {
        factoryPrefabs = new Dictionary<Race, GameObject>();
        raceToSoldierInfos = new Dictionary<Race, zzGenericIndexTable<string, SoldierInfo>>();
        foreach (var lInfo in info)
        {
            var lSoldierInfos = new zzGenericIndexTable<string, SoldierInfo>();
            foreach (var lSoldierInfo in lInfo.SoldierInfoes)
            {
                lSoldierInfo.armyBaseName = lInfo.armyBaseName;
                lSoldierInfos.addData(lSoldierInfo.name, lSoldierInfo);
            }
            raceToSoldierInfos[lInfo.race] = lSoldierInfos;
            setFactoryPrefabs(lInfo.race, lInfo.factoryBuildingPrefab);
        }
    }

}