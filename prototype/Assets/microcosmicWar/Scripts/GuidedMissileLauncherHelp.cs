
using UnityEngine;
using System.Collections;

public class GuidedMissileLauncherHelp : MonoBehaviour
{

    public Race race = Race.eNone;

    void Awake()
    {
        if (race == Race.eNone)
            return;

        GameObject lTowerObject = gameObject;
        GuidedMissileLauncherAI lTowerAI = lTowerObject.GetComponentInChildren<GuidedMissileLauncherAI>();
        /*
        switch( race )
        {
            case Race.ePismire:
            {
                break;
            }
            case Race.eBee:
            {
                break;
            }
        }
        */
        lTowerObject.layer = PlayerInfo.getRaceLayer(race);
        //print(lTowerObject.layer);
        lTowerAI.setAdversaryLayerMask(PlayerInfo.getAdversaryObjectValue(race));
    }
}