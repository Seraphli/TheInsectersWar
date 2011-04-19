using UnityEngine;
using System.Collections;

public class zzSetObjectMap:MonoBehaviour
{
    public string objectName;
    public GameObject objectToMap;

    public void imp()
    {
        zzObjectMap.setObject(objectName, objectToMap);
    }

}