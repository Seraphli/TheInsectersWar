
using UnityEngine;
using System.Collections;

class healthPackItem : IitemObject
{
    //总共可以恢复的生命值
    public int recoverValue = 100;

    //效果持续时间
    public float duration = 10.0f;

    public GameObject healthPackObjectPrefab;

    public GameObject useObject;

    public override bool canUse(GameObject pGameObject)
    {
        useObject = pGameObject;
        return true;
    }

    public override void use()
    {
        putHealthPack(useObject, duration);
        if (Network.isServer)
        {
            networkView.RPC("RPCPutHealthPack", RPCMode.Others,
                useObject.networkView.viewID, (float)Network.time);
        }
    }

    [RPC]
    void RPCPutHealthPack(NetworkViewID pViewID,float pServerTime)
    {
        var lDuration = duration;
        var lLostTime = (float)Network.time - pServerTime;
        if (lLostTime > 0.5f)
            lDuration -= lLostTime;
        putHealthPack(NetworkView.Find(pViewID).gameObject, lDuration);
        //putHealthPack()
    }

    void putHealthPack(GameObject pObject, float pDuration)
    {
        Life lLife = pObject.GetComponent<Life>();
        var lhealthPack = (GameObject)zzCreatorUtility.Instantiate(healthPackObjectPrefab);
        LifeRecoverConstValue lLifeRecover = lhealthPack.GetComponent<LifeRecoverConstValue>();
        lLifeRecover.setLife(lLife);
        lLifeRecover.recoverValue = recoverValue;
        lLifeRecover.duration = pDuration;

        var lhealthPackTransform = lhealthPack.transform;
        lhealthPackTransform.parent = pObject.transform;
        lhealthPackTransform.localPosition = Vector3.zero;
    }
};