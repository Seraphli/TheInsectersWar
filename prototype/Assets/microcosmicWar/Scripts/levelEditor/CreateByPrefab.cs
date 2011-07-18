using UnityEngine;
using System.Collections;

public class CreateByPrefab: MonoBehaviour
{
    public GameObject prefab;

    public Transform createObjectTransform;

    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject){}

    AddObjectEvent addObjectEvent;

    public bool onlyCreateInHost = false;

    void Start()
    {
        if (addObjectEvent == null)
            addObjectEvent = nullAddObjectEvent;
    }

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    public void create()
    {
        if (onlyCreateInHost && Network.isClient)
            return;
        var lObject = (GameObject)Instantiate(prefab,
            createObjectTransform.position, createObjectTransform.rotation);

        addObjectEvent(lObject);
    }
}