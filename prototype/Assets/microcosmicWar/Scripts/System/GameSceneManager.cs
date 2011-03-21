using UnityEngine;
using System.Collections;

public class GameSceneManager:MonoBehaviour
{
    //此变量只是为了让ManagerInfo类中的managerType以枚举UI显示
    public UnitManagerType managerType;

    public enum UnitManagerType
    {
        none = 0,
        soldier,
        stronghold,
        hero,
        heroSpawn,
        defenseTower,
        flyer,
        raceBase,
        typeCount,
    }

    public enum MapManagerType
    {
        none = 0,
        board,
        ground,
        moveableObject,
        background,
        wayNode,
        typeCount,
    }

    public void clearAllObject()
    {
        foreach (var lUnitSceneManagers in unitSceneManagersList)
        {
            if (lUnitSceneManagers!=null)
                foreach (var lUnitSceneManager in lUnitSceneManagers)
                {
                    if (lUnitSceneManager)
                        lUnitSceneManager.clearObject();
                }
        }
        foreach (var lMapManager in mapManagerList)
        {
            if (lMapManager)
                lMapManager.clearObject();
        }
    }


    [System.Serializable]
    public class MapManagerInfo
    {
        public MapManagerType managerType;
        public zzSceneManager sceneManager;
    }

    public MapManagerInfo[] mapManagerInfos;

    [SerializeField]
    zzSceneManager[] mapManagerList;

    void createMapManagerList()
    {
        //var lMapManagerInfos = new MapManagerInfo[(int)MapManagerType.typeCount];
        var lMapManagerData = new zzSceneManager[(int)MapManagerType.typeCount];
        foreach (var lMapManagerInfo in mapManagerInfos)
        {
            lMapManagerData[(int)lMapManagerInfo.managerType] = lMapManagerInfo.sceneManager;
        }
        mapManagerList = lMapManagerData;
    }

    public void addObject(MapManagerType pManagerName, GameObject pObject)
    {
        mapManagerList[(int)pManagerName].addObject(pObject);
    }

    [System.Serializable]
    public class UnitManagerInfo
    {
        public UnitManagerType managerType;
        public zzSceneManager sceneManager;
    }

    [System.Serializable]
    public class ObjectRaceManagerInfo
    {
        public Race race;
        public UnitManagerInfo[] managerInfos;
    }

    public ObjectRaceManagerInfo[] objectRaceManagerInfo;

    public zzSceneManager[][] unitSceneManagersList;

    static protected GameSceneManager singletonInstance;

    public static GameSceneManager getSingleton()
    {
        return singletonInstance;
    }

    public static GameSceneManager Singleton
    {
        get { return singletonInstance; }
    }

    void Awake()
    {
        if (singletonInstance != null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        createUnitSceneManagerList();
        createMapManagerList();
    }

    private void createUnitSceneManagerList()
    {

        unitSceneManagersList = new zzSceneManager[3][];
        foreach (var lRaceManagersInfo in objectRaceManagerInfo)
        {
            var lSceneManagersList = new zzSceneManager[(int)UnitManagerType.typeCount];
            foreach (var lManagersInfo in lRaceManagersInfo.managerInfos)
            {
                lSceneManagersList[(int)lManagersInfo.managerType] = lManagersInfo.sceneManager;
            }
            unitSceneManagersList[(int)lRaceManagersInfo.race] = lSceneManagersList;
        }
    }

    public zzSceneManager getManager(MapManagerType pManagerName)
    {
        return mapManagerList[(int)pManagerName];
    }

    public zzSceneManager getManager(Race pRace,UnitManagerType pManagerName)
    {
        return unitSceneManagersList[(int)pRace][(int)pManagerName];
    }

    public void addObject(Race pRace, UnitManagerType pManagerName, GameObject pObject)
    {
        getManager(pRace, pManagerName).addObject(pObject);
    }

    public void addSoldier( GameObject pObject)
    {
        addObject(
                PlayerInfo.getRace(pObject.layer),
                GameSceneManager.UnitManagerType.soldier,
                pObject);
    }

    public void addHero(GameObject pObject)
    {
        addObject(
                PlayerInfo.getRace(pObject.layer),
                GameSceneManager.UnitManagerType.hero,
                pObject);
    }
}