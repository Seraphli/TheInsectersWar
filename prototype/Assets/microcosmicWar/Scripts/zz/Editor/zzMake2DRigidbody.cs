using UnityEngine;
using UnityEditor;

public class zzMake2DRigidbody : MonoBehaviour 
{

    [MenuItem("zz/Make 2D Rigidbody")]
    static void Make2DRigidbody()
    {
        Selection.activeTransform.rigidbody.constraints =
                RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezePositionZ;
    }

    [MenuItem("zz/Make 2D Rigidbody",true)]
    static bool ValidateMake2DRigidbody()
    {
        return Selection.activeTransform != null
            && Selection.activeTransform.rigidbody != null;
    }
}