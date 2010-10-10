
using UnityEngine;
using System.Collections;

class healthPackItem : IitemObject
{
    public GameObject useObject;

    public override bool canUse(GameObject pGameObject)
    {
        useObject = pGameObject;
        return true;
    }

    public override void use()
    {
        Life lLife = useObject.GetComponent<Life>();
        LifeRecover lLifeRecover = useObject.AddComponent<LifeRecover>();
        lLifeRecover.setLife(lLife);
    }
};