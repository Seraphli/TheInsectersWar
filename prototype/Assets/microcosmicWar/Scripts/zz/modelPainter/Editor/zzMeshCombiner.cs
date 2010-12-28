using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class zzMeshCombiner:MonoBehaviour
{
    [MenuItem("zz/Combine Children Mesh")]
    static void meshCombine()
    {
        combineChildrenMesh(Selection.activeTransform.gameObject);
        //foreach (var lSub in Selection.activeTransform)
        //{
        //    MeshFilter  
        //    var lMeshInfo = new  MeshCombineUtility.MeshInstance();
        //    lMeshInfo.subMeshIndex = lIndex++;
        //}
    }

    public static void combineChildrenMesh(GameObject pObject)
    {
        GameObject lObject = pObject;
        Matrix4x4 myTransform = lObject.transform.worldToLocalMatrix;

        var lMeshFilter = lObject.GetComponent<MeshFilter>();
        if (lMeshFilter)
            GameObject.DestroyImmediate(lMeshFilter);

        MeshFilter[] lSubfilters
            = lObject.GetComponentsInChildren<MeshFilter>();
        var lMeshInfoes = new MeshCombineUtility.MeshInstance[lSubfilters.Length];
        for (int i = 0; i < lSubfilters.Length; ++i)
        {
            lMeshInfoes[i].mesh = lSubfilters[i].sharedMesh;
            //lMeshInfoes[i].subMeshIndex = i;
            lMeshInfoes[i].transform = myTransform * lSubfilters[i].transform.localToWorldMatrix;
            if (lSubfilters[i].GetComponent<MeshRenderer>())
                GameObject.DestroyImmediate(lSubfilters[i].GetComponent<MeshRenderer>());
            GameObject.DestroyImmediate(lSubfilters[i]);
        }

        lMeshFilter = lObject.AddComponent<MeshFilter>();
        lMeshFilter.sharedMesh = MeshCombineUtility.Combine(lMeshInfoes, false);

        MeshRenderer lMeshRenderer = lObject.GetComponent<MeshRenderer>();
        if (!lMeshRenderer)
            lObject.AddComponent<MeshRenderer>();

    }

    // Validate the menu item.
    // The item will be disabled if no transform is selected.
    [MenuItem("zz/Combine Children Mesh", true)]
    static bool ValidateMeshCombine()
    {
        return Selection.activeTransform != null
            && Selection.activeTransform.GetChildCount() != 0;
            //|| Selection.activeTransform.GetComponent<MeshFilter>();
    }
}