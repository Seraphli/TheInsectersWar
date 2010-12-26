
using UnityEngine;
using System.Collections;


public class zzObjectMap : MonoBehaviour
{
    public string objectName;
    static Hashtable mObjectMap = new Hashtable();

    void Awake()
    {
        if (
            mObjectMap.Contains(objectName)
            && (mObjectMap[objectName] as GameObject)
            )
            Debug.LogError("same name:" + objectName);

        mObjectMap[objectName] = gameObject;
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