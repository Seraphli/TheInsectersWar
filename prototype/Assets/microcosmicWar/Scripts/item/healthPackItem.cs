
using UnityEngine;
using System.Collections;

class healthPackItem : IitemObject
{
    //总共可以恢复的生命值
    public int recoverValue = 100;

    //效果持续时间
    public float duration = 10.0f;

    public GameObject useObject;

    public override bool canUse(GameObject pGameObject)
    {
        useObject = pGameObject;
        return true;
    }

    public override void use()
    {
        Life lLife = useObject.GetComponent<Life>();
        LifeRecoverConstValue lLifeRecover = useObject.AddComponent<LifeRecoverConstValue>();
        lLifeRecover.setLife(lLife);
        lLifeRecover.recoverValue = recoverValue;
        lLifeRecover.duration = duration;
    }
};