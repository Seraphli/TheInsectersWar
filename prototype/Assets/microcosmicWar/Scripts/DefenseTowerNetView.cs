
using UnityEngine;
using System.Collections;

public class DefenseTowerNetView : MonoBehaviour
{


    public DefenseTower defenseTower;
    public Life life;

    void Awake()
    {
        if (!defenseTower)
            defenseTower = gameObject.GetComponentInChildren<DefenseTower>();
        if (!life)
            life = gameObject.GetComponentInChildren<Life>();

        //print(!zzCreatorUtility.isHost());
        if (!zzCreatorUtility.isHost())
        {
            Destroy(defenseTower.GetComponentInChildren<AiMachineGunAI>());
        }
    }


    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        life.OnSerializeNetworkView(stream, info);

        float lAngle=0;
        bool lFire=false;
        float lAimAngular=0;
        //int lFace;

        //---------------------------------------------------
        if (stream.isWriting)
        {
            lAngle = defenseTower.getAngle();
            lFire = defenseTower.inFiring();
            lAimAngular = defenseTower._getSmoothAngle();
            //	lFace=defenseTower.getFaceDirection();

        }

        //---------------------------------------------------
        stream.Serialize(ref lAngle);
        stream.Serialize(ref lFire);
        stream.Serialize(ref lAimAngular);
        //stream.Serialize(lFace);

        //---------------------------------------------------
        if (stream.isReading)
        {
            defenseTower.setAngle(lAngle);
            defenseTower.setFire(lFire);
            defenseTower._setSmoothAngle(lAimAngular);
            //	defenseTower.setFaceDirection(lFace);

        }
    }
}