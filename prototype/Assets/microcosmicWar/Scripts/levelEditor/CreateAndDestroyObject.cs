using UnityEngine;
using System.Collections;

public class CreateAndDestroyObject : MonoBehaviour
{
    public GameObject objectToManage;

    public GameObject tempObject;

    public bool clone = true;

    public void createObject()
    {
        if (clone)
            tempObject = (GameObject)Instantiate(objectToManage);
        else
        {
            objectToManage.SetActiveRecursively(true);
            //print("objectToManage:" + objectToManage.transform.childCount);
            //foreach (Transform lSub in objectToManage.transform)
            //{
            //    if (lSub.networkView)
            //        print(lSub.name + " ID:" + lSub.networkView.viewID);
            //}
        }
    }

    public void destoryObject()
    {
        Destroy(tempObject);
    }
}
