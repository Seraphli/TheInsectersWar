
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
        Life lLife = useObject.GetComponent<Life>();
        var lhealthPack = (GameObject)zzCreatorUtility.Instantiate(healthPackObjectPrefab);
        LifeRecoverConstValue lLifeRecover = lhealthPack.GetComponent<LifeRecoverConstValue>();
        lLifeRecover.setLife(lLife);
        lLifeRecover.recoverValue = recoverValue;
        lLifeRecover.duration = duration;

        var lhealthPackTransform = lhealthPack.transform;
        lhealthPackTransform.parent = useObject.transform;
        lhealthPackTransform.localPosition = Vector3.zero;
        if(Network.isServer)
        {
            lhealthPack.GetComponent<zzSetRemoteAttach>().setData(useObject.transform);
        }
    }
};