using UnityEngine;

public class BuildingDefensiveSystem:MonoBehaviour
{
    public zzDetectorBase enemyDetector;
    public GameObject owner;

    public System.Action pismireOwnerEvent;
    public System.Action beeOwnerEvent;

    void Start()
    {
        var lRace = PlayerInfo.getRace(owner.layer);
        if (lRace == Race.ePismire && pismireOwnerEvent!=null)
            pismireOwnerEvent();
        else if (lRace == Race.eBee && beeOwnerEvent!=null)
            beeOwnerEvent();
        enemyDetector.setLayerMaskRecursively(1 << PlayerInfo.getAdversaryRaceLayer(lRace));
    }

    //void enabledList(Behaviour[] pList, bool pEnabled)
    //{
    //    foreach (var lEnabled in pList)
    //    {
    //        lEnabled.enabled = pEnabled;
    //    }
    //}
}