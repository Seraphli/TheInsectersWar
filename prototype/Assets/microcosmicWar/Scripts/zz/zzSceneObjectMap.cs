
using UnityEngine;
using System.Collections;


public class zzSceneObjectMap : MonoBehaviour
{
    [System.Serializable]
    class SceneObjectInfo
    {
        public string name;
        public GameObject gameObject;
    }

    [SerializeField]
    SceneObjectInfo[] objectMap;

    Hashtable mObjectMap = new Hashtable();

    void    Awake()
    {
        foreach(var lObjectInfo in objectMap)
        {
            mObjectMap[lObjectInfo.name] = lObjectInfo.gameObject;
        }
    }

    public GameObject  getObject(string pName)
    {
        return mObjectMap[pName] as GameObject;
        //return null;
    }
}