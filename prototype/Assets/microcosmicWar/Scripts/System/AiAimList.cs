using UnityEngine;

public class AiAimList:MonoBehaviour
{
    [System.Serializable]
    public class AimCountInfo
    {
        public int aimCount;
        public int weight;
    }

    [System.Serializable]
    public class AimInfo
    {
        public Race race;
        public GameSceneManager.UnitManagerType aimUnitType;
        public int weight;
    }

    public AimCountInfo[] aimCountInfo;
    zzRandomObjectByWeight<int> randomAimCount = new zzRandomObjectByWeight<int>();

    public AimInfo[] aimInfo;
    zzRandomObjectByWeight<AimInfo> randomAim = new zzRandomObjectByWeight<AimInfo>();

    void Awake()
    {
        foreach (var lAimCountInfo in aimCountInfo)
        {
            randomAimCount.addRandomObject(lAimCountInfo.aimCount, lAimCountInfo.weight);
        }
        foreach (var lAimInfo in aimInfo)
        {
            randomAim.addRandomObject(lAimInfo,lAimInfo.weight);
        }
    }

    public void addAim(ISoldierAI pSoldierAI)
    {
        int lAimCount = randomAimCount.randomObject();
        for (int i = 0; i < lAimCount;++i )
        {
            var lRandomAim = randomAim.randomObject();
            var lAim = getRandomAim(lRandomAim);
            if (lAim)
                pSoldierAI.AddPresetAim(lAim, getAimType(lRandomAim.aimUnitType));
        }
    }

    public zzAimTranformList.AimType getAimType(GameSceneManager.UnitManagerType pAimUnitType)
    {
        switch(pAimUnitType)
        {
            case GameSceneManager.UnitManagerType.hero:
            case GameSceneManager.UnitManagerType.soldier:
                return zzAimTranformList.AimType.aliveAim;
        }
        return zzAimTranformList.AimType.checkPoint;
    }

    public Transform getRandomAim(AimInfo pAimInfo)
    {
        if(pAimInfo.race== Race.eNone)
        {
            if (pAimInfo.aimUnitType == GameSceneManager.UnitManagerType.stronghold)
            {
                return getRandomAim( GameSceneManager.
                        Singleton.getManager(GameSceneManager.MapManagerType.stronghold).managerRoot
                        );
            }
            else
                return null;
        }
        return getRandomAim(GameSceneManager.
            Singleton.getManager(pAimInfo.race, pAimInfo.aimUnitType).managerRoot
            );
    }

    public Transform getRandomAim(Transform pManagerTransform)
    {
        if (pManagerTransform.childCount > 0)
        {
            return pManagerTransform.GetChild(Random.Range(0, pManagerTransform.childCount));
        }
        return null;
    }
}