
using UnityEngine;
using System.Collections;

public abstract class IitemObject : MonoBehaviour
{


    public abstract bool canUse(GameObject pGameObject);

    public abstract void use();

    void Reset()
    {
        ItemObjectImp lItemObjectImp = (ItemObjectImp)zzUtilities.needComponent(gameObject, typeof(ItemObjectImp));
        lItemObjectImp.setItemObject(this);
    }
}