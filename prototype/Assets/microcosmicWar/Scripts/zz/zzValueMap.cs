using UnityEngine;
using System.Collections;

class zzValueMap : MonoBehaviour
{
    [System.Serializable]
    class ValueInfo
    {
        public string name;
        public string value;
    }

    [SerializeField]
    ValueInfo[] objectMap;

    Hashtable mObjectMap = new Hashtable();

    void Awake()
    {
        foreach (var lObjectInfo in objectMap)
        {
            mObjectMap[lObjectInfo.name] = lObjectInfo.value;
        }
    }

    public string getValue(string pName)
    {
        return mObjectMap[pName] as string;
        //return null;
    }

}