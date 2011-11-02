using UnityEngine;
using System.Collections;

//用于管理场景物体,把一类物体,添加到一个同一个父物体下
//要获取此类物体时,就比较方便了
public class zzSceneManager:MonoBehaviour , IEnumerable
{
    public Transform managerRoot;
    public string managerName = "SceneManager";

    void Start()
    {
        if (!managerRoot)
            managerRoot = (new GameObject(managerName)).transform;
    }

    public void addObject(GameObject pObject)
    {
        pObject.transform.parent = managerRoot;
    }

    public void addObjects(GameObject[] pObjects)
    {
        foreach (var lObject in pObjects)
        {
            addObject(lObject);
        }
    }

    public void clearObject()
    {
        foreach (Transform lObject in managerRoot)
        {
            Destroy(lObject.gameObject);
        }
    }

    public int objectCount
    {
        get { return managerRoot.GetChildCount(); }
    }

    public IEnumerator GetEnumerator()
    {
        return managerRoot.GetEnumerator();
    }
}