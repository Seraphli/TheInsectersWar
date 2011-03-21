using UnityEngine;
using System.Collections;

public class CreateAndDestoryObject : MonoBehaviour
{
    public GameObject objectToManage;

    public GameObject tempObject;

    public void createObject()
    {
        tempObject = (GameObject)Instantiate(objectToManage);
    }

    public void destoryObject()
    {
        Destroy(tempObject);
    }
}
