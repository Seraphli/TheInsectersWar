using UnityEngine;

public class PlayerSoldierFactoryState:MonoBehaviour
{
    public Race race;

    [System.Serializable]
    public class SoldierInfo
    {
        public string name;
        public bool locked;
        public int unlockCost;
    }

    public SoldierInfo[] soldierFactory;

    public WMPurse purse;

    public bool tryUnlockSoldier(int pIndex)
    {
        var lSoldierInfo = soldierFactory[pIndex];
        if(lSoldierInfo.locked
            && lSoldierInfo.unlockCost<=purse.number)
        {
            lSoldierInfo.locked = false;
            purse.cost(lSoldierInfo.unlockCost);
            return true;
        }
        return false;
    }
}