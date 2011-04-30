using UnityEngine;
using UnityEditor;

public class AutoSetGameObjectType:MonoBehaviour
{
    [MenuItem("Component/WM/Set Sub Object Type")]
    static void setSubGameObjectType()
    {
        foreach (Transform lTransform in Selection.activeTransform)
        {
            setObjectType(lTransform.gameObject);
        }
    }

    [MenuItem("Component/WM/Set Sub Object Type", true)]
    static bool validateSetSubGameObjectType()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("Component/WM/Set Object Type")]
    static void setGameObjectType()
    {
        setObjectType(Selection.activeTransform.gameObject);
    }

    [MenuItem("Component/WM/Set Object Type", true)]
    static bool validateSetGameObjectType()
    {
        return Selection.activeTransform != null;
    }

    static void setObjectType(GameObject lObject)
    {
        if (!lObject.GetComponent<WMGameObjectType>())
            lObject.AddComponent<WMGameObjectType>().mapType
                = GameSceneManager.MapManagerType.other;
        if (!lObject.GetComponent<AutoWMGameObjectType>())
            lObject.AddComponent<AutoWMGameObjectType>();
    }
}