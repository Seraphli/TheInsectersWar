
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class zzObjectMap : MonoBehaviour
{
    public string objectName;
    static Dictionary<string, GameObject> mObjectMap
        = new Dictionary<string, GameObject>();

    void Awake()
    {
        setObject(objectName,gameObject);
    }

    public static void setObject(string pObjectName,GameObject pObject)
    {
        if ( mObjectMap.ContainsKey(pObjectName) )
            Debug.LogError("same name:" + pObjectName);

        mObjectMap[pObjectName] = pObject;

    }

    void OnDestroy()
    {
        mObjectMap.Remove(objectName);
    }

    public static GameObject getObject(string pName)
    {
        GameObject lOut;
        if (mObjectMap.TryGetValue(pName, out lOut))
        {
            return lOut;
        }
        return GameObject.FindWithTag(pName);

    }
}