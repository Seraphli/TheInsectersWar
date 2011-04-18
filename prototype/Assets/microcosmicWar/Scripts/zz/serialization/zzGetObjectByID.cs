using UnityEngine;
using System.Collections.Generic;

public class zzGetObjectByID:MonoBehaviour
{
    static Dictionary<int, GameObject> idToObject 
        = new Dictionary<int, GameObject>();


    public static void addObject(int pID,GameObject pObject)
    {
        idToObject[pID] = pObject;
    }

    struct SetObjectMethod
    {
        public SetObjectMethod(System.Action<GameObject> pSet, int pId)
        {
            setMethod = pSet;
            wantObjectId = pId;
        }
        public System.Action<GameObject> setMethod;
        public int wantObjectId;
    }

    static List<SetObjectMethod> setObjectMethodList
        = new List<SetObjectMethod>();

    public static void addSetMethod(System.Action<GameObject> pSet,int pId)
    {
        setObjectMethodList.Add(new SetObjectMethod(pSet, pId));
        //singletonInstance.enabled = true;
    }

    public static void impSetObject()
    {
        foreach (var lSetObjectMethod in setObjectMethodList)
        {
            lSetObjectMethod.setMethod(idToObject[lSetObjectMethod.wantObjectId]);
        }
        setObjectMethodList.Clear();
        idToObject.Clear();
    }

    static zzGetObjectByID singletonInstance;

    void Awake()
    {
        //enabled = false;
        singletonInstance = this;
    }

    public void setObject()
    {
        impSetObject();
    }

    //void Update()
    //{
    //    setObject();
    //    setObjectMethodList.Clear();
    //    idToObject.Clear();
    //    enabled = false;
    //}
}