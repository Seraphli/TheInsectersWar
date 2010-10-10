
using UnityEngine;
using System.Collections;

public class ItemObjectImp : MonoBehaviour
{

    public IitemObject itemObject;

    public bool canUse(GameObject pGameObject)
    {
        return itemObject.canUse(pGameObject);
    }

    public virtual void use()
    {
        itemObject.use();
    }

    public void setItemObject(IitemObject pItemObject)
    {
        itemObject = pItemObject;
    }
}