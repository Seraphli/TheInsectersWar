using UnityEngine;
using UnityEditor;

public class MakeEditableObject : MonoBehaviour 
{

    [MenuItem("zz/Make Editable")]
    public static void make()
    {
        var lObject = Selection.activeTransform.gameObject;
        checkComponent<Rigidbody>(lObject);
        checkComponent<zz2DRigidbodyObject>(lObject);
        checkComponent<zzEditableObjectContainer>(lObject);
        checkComponent<ObjectPropertySetting>(lObject);
        checkComponent<WMGameObjectType>(lObject);
    }

    [MenuItem("zz/Make Editable", true)]
    public static bool ValidateMake()
    {
        return Selection.activeTransform != null;
    }

    public static void checkComponent<T>(GameObject pObject) where T : Component
    {
        if(!pObject.GetComponent<T>())
            pObject.AddComponent<T>();
    }
}