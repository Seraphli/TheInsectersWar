using UnityEngine;
using System.Collections;

public class SerializeScene:MonoBehaviour
{
    public IEnumerable enumerateObject;

    public delegate void AddObjectEvent(GameObject pObject);

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    public object serializeTo()
    {
        return serializeToArray();
    }

    public ArrayList serializeToArray()
    {
        ArrayList lOut = new ArrayList();
        foreach (Transform lObject in enumerateObject)
        {
            lOut.Add(serializeObjectToTable(lObject.gameObject));
        }
        return lOut;
    }

    public Hashtable serializeObjectToTable(GameObject pObject)
    {
        var lPropertySetting = pObject.GetComponent<ObjectPropertySetting>();
        var lOut = lPropertySetting.serializeToTable();
        lOut["#Type"] = lPropertySetting.TypeName;
        var lTransform = pObject.transform;
        lOut["#position"] = lTransform.localPosition;
        lOut["#rotation"] = lTransform.localRotation;
        lOut["#scale"] = lTransform.localScale;
        return lOut;
    }

    public void serializeFrom(object pObject)
    {
        serializeFromArray(pObject as ArrayList);
    }

    public void serializeFromArray(ArrayList pArray)
    {
        //for (int i = 0; i < pArray.Count;++i )
        //{
        //    serializeObjectFromTable(pArray[i] as Hashtable).loadId = i + 1;
        //}
        int lID = 1;
        foreach (Hashtable lTable in pArray)
        {
            var lObject = serializeObjectFromTable(lTable);
            if (lObject)
            {
                lObject.loadId = lID;
                var lNetworkViews = lObject.GetComponentsInChildren<NetworkView>();
                lID += 1 + lNetworkViews.Length;
                foreach (var lNetworkView in lNetworkViews)
                {
                    lNetworkView.enabled = false;
                }
            }
        }
    }

    public ObjectPropertySetting serializeObjectFromTable(Hashtable pTable)
    {
        var lObject = GameSystem.Singleton
            .createObject((string)pTable["#Type"]);
        if (!lObject)
            return null;
        var lPosition = (Vector3)pTable["#position"];
        var lRotation = (Quaternion)pTable["#rotation"];
        var lScale = (Vector3)pTable["#scale"];
        var lOut = lObject.GetComponent<ObjectPropertySetting>();
        lOut.serializeFromTable(pTable);
        var lTransform = lObject.transform;
        lTransform.localPosition = lPosition;
        lTransform.localRotation = lRotation;
        lTransform.localScale = lScale;

        //必须要设置之后才能调用
        addObjectEvent(lObject);
        return lOut;
    }

}