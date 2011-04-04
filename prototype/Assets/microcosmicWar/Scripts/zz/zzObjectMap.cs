
using UnityEngine;
using System.Collections;


public class zzObjectMap : MonoBehaviour
{
    public string objectName;
    static Hashtable mObjectMap = new Hashtable();

    void Awake()
    {
        setObject(objectName,gameObject);
    }

    public static void setObject(string pObjectName,GameObject pObject)
    {
        if ( mObjectMap.Contains(pObjectName) )
            Debug.LogError("same name:" + pObjectName);

        mObjectMap[pObjectName] = pObject;

    }

    public static GameObject getObject(string pName)
    {
        if (mObjectMap.Contains(pName))
        {
            return mObjectMap[pName] as GameObject;
        }
        return GameObject.FindWithTag(pName);

    }
}