using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzFlatModelPainter : zzModelPainterProcessor
{
    protected override void draw(Mesh pMesh, List<Vector2[]> pSurfaceList,
        List<Vector2[]> pEdgeList, float zThickness, Vector2 pUvScale,
        Vector2 pPointOffset)
    {
        zzFlatMeshUtility.draw(pMesh, pSurfaceList, pEdgeList, zThickness, pUvScale, pPointOffset);
    }
}