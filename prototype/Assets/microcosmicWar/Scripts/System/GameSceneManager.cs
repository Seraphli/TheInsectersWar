using UnityEngine;
using System.Collections;

public class GameSceneManager:MonoBehaviour
{
    //此变量只是为了让ManagerInfo类中的managerType以枚举UI显示
    public ManagerType managerType;

    public enum ManagerType
    {
        none = 0,
        soldier,
        stronghold,
        hero,
        defenseTower,
        flyer,
        typeCount,
    }

    [System.Serializable]
    public class ManagerInfo
    {
        public ManagerType managerType;
        public zzSceneManager sceneManager;
    }

    [System.Serializable]
    public class ObjectRaceManagerInfo
    {
        public Race race;
        public ManagerInfo[] managerInfos;
    }

    public ObjectRaceManagerInfo[] objectRaceManagerInfo;

    public zzSceneManager[][] sceneManagersList;

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

        sceneManagersList = new zzSceneManager[3][];
        foreach (var lRaceManagersInfo in objectRaceManagerInfo)
        {
            var lSceneManagersList = new zzSceneManager[(int)ManagerType.typeCount];
            foreach (var lManagersInfo in lRaceManagersInfo.managerInfos)
            {
                lSceneManagersList[(int)lManagersInfo.managerType] = lManagersInfo.sceneManager;
            }
            sceneManagersList[(int)lRaceManagersInfo.race] = lSceneManagersList;
        }
    }

    public zzSceneManager getManager(Race pRace,ManagerType pManagerName)
    {
        return sceneManagersList[(int)pRace][(int)pManagerName];
    }

    public void addObject(Race pRace, ManagerType pManagerName, GameObject pObject)
    {
        getManager(pRace, pManagerName).addObject(pObject);
    }

    public void addSoldier( GameObject pObject)
    {
        addObject(
                PlayerInfo.getRace(pObject.layer),
                GameSceneManager.ManagerType.soldier,
                pObject);
    }

    public void addHero(GameObject pObject)
    {
        addObject(
                PlayerInfo.getRace(pObject.layer),
                GameSceneManager.ManagerType.hero,
                pObject);
    }
}