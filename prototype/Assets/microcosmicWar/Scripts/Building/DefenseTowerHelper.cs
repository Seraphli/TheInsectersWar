
using UnityEngine;
using System.Collections;

public class DefenseTowerHelper : MonoBehaviour
{


    public Race race = Race.eNone;

    void Awake()
    {
        if (race == Race.eNone)
            return;

        GameObject lTowerObject = gameObject;
        //AiMachineGunAI lTowerAI = lTowerObject.GetComponentInChildren<AiMachineGunAI>();
        OldAiMachineGunAI lTowerAI = lTowerObject.transform.Find("turn/enemyDetector").GetComponent<OldAiMachineGunAI>();
        //Transform AIOb = transform.Find("turn/enemyDetector");
        //print("AIOb:"+(AIOb==null));
        //print(AIOb.GetComponent<AiMachineGunAI>()==null);
        //print(AIOb.GetComponent<AiMachineGunAI>().enabled);
        //print(AIOb.active);
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
        //print(gameObject.name);
        //print(lTowerAI==null);
        //print(lTowerObject.layer);
        lTowerAI.setAdversaryLayerMask(PlayerInfo.getAdversaryObjectValue(race));
    }
}