using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class zzFlatMeshEdit:MonoBehaviour
{
    public float UvUnitLength = 10.0f;

    [ContextMenu("calculate and set UV")]
    void resetUV()
    {
        Mesh lMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        Vector3 lScale = transform.lossyScale;
        var lUVs = zzFlatMeshUtility.verticesCoordToUV(lMesh.vertices,
            new Vector2(lScale.x / UvUnitLength, lScale.y / UvUnitLength));

        lMesh.uv = lUVs;
        //gameObject.GetComponent<MeshFilter>().sharedMesh = lMesh;
    }
}