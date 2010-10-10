
using UnityEngine;
using System.Collections;

public class zzGameObjectCreator : MonoBehaviour
{


    static protected zzGameObjectCreator singletonInstance = null;

    public static zzGameObjectCreator getSingleton()
    {
        return singletonInstance;
    }

    [System.Serializable]
    public class zzCreatorInfo
    {
        //必须有zzGameObjectInit
        public string name;
        public zzGameObjectInit prefab;
    }

    public zzCreatorInfo[] creatorList;

    //[name] = prefab
    protected Hashtable creatorMap = new Hashtable();


    /*
    ["creatorName"]
    ["position"]
    ["rotation"]
    */
    public void create(Hashtable p)
    {
        Vector3 position = new Vector3();
        Quaternion rotation = new Quaternion();
        if (p.ContainsKey("position"))
            position = (Vector3)p["position"] ;
        if (p.ContainsKey("rotation"))
            rotation = (Quaternion)p["rotation"];
        GameObject clone = zzCreatorUtility.Instantiate((GameObject)creatorMap[p["creatorName"]], position, rotation, 0);
        Debug.Log(p["creatorName"] + " " + clone.name);
        zzGameObjectInit initObject = clone.GetComponent<zzGameObjectInit>();
        initObject.init(p);
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;

        foreach (zzCreatorInfo v in creatorList)
        {
            creatorMap[v.name] = v.prefab.gameObject;
        }
    }

}