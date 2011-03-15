using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzFlatModelPainter : zzModelPainterProcessor
{

    public override void sweepPicture()
    {
        pointNumber = 0;
        polygonNumber = 0;
        holeNumber = 0;
        var lSweeperResults = zzOutlineSweeper.sweeper(activeChart, ignoreDistanceInSweeping);
        modelsSize = new Vector2((float)activeChart.width, (float)activeChart.height);

        concaves = new List<zz2DConcave>();
        foreach (var lSweeperResult in lSweeperResults)
        {

            if (lSweeperResult.edge.Length < 2)
                continue;

            zzSimplyPolygon lPolygon = new zzSimplyPolygon();
            lPolygon.setShape(lSweeperResult.edge);

            zz2DConcave lConcave = new zz2DConcave();
            lConcave.setShape(lPolygon);
            ++polygonNumber;

            foreach (var lHole in lSweeperResult.holes)
            {
                if (lHole.Length < 2)
                    continue;
                zzSimplyPolygon lHolePolygon = new zzSimplyPolygon();
                lHolePolygon.setShape(lHole);
                lConcave.addHole(lHolePolygon);
                ++holeNumber;
            }

            concaves.Add(lConcave);
        }
    }

    public override void draw()
    {
        models = new GameObject("PaintModel");
        models.transform.position = new Vector3(modelsSize.x / 2.0f, modelsSize.y / 2.0f, 0.0f);
        int i = 0;
        foreach (var lConvexs in convexesList)
        {
            var lSurfaceList = new List<Vector2[]>(lConvexs.Length);
            foreach (var lConvex in lConvexs)
            {
                lSurfaceList.Add(lConvex.getShape());
            }

            string lPolygonName = "polygon" + i;
            GameObject lConvexsObject = new GameObject(lPolygonName);
            lConvexsObject.transform.parent = models.transform;

            var lRenderObject = createFlatMesh(concaves[i], lSurfaceList, "Render",
                    lConvexsObject.transform, thickness,
                    new Vector2(1.0f / modelsSize.x, 1.0f / modelsSize.y));

            lRenderObject.AddComponent<zzFlatMeshEdit>();
            Vector3 lCenter = lRenderObject.GetComponent<MeshFilter>().sharedMesh.bounds.center;
            lCenter.z = 0;
            //lConvexsObject是lRenderObject的父物体
            lConvexsObject.transform.position += lCenter;
            lRenderObject.transform.position -= lCenter;

            ++i;

            int lSubIndex = 0;
            string lSubName = "Collider";
            foreach (var lConvex in lConvexs)
            {
                var lColliderObject = createFlatCollider(lConvex.getShape(),
                    lSubName + lSubIndex,
                    lConvexsObject.transform, thickness);
                //因为是先创建,后关联父级的,所以不用移动
                //lColliderObject.transform.position -= lCenter;
                ++lSubIndex;
            }
        }

    }

    [ContextMenu("clear")]
    public override void clear()
    {
        DestroyImmediate(models);
    }

    static GameObject createFlatCollider(Vector2[] points, string pName, Transform parent, float zThickness)
    {
        GameObject lOut = new GameObject(pName);
        //lOut.active = false;
        lOut.transform.parent = parent;
        MeshCollider lMeshCollider = lOut.AddComponent<MeshCollider>();
        Mesh lMesh = new Mesh();
        zzFlatMeshUtility.draw(lMesh, points, zThickness);
        //lMeshCollider.convex = true;

        lMeshCollider.sharedMesh = lMesh;

        return lOut;
    }

    static GameObject createFlatMesh(zz2DConcave pConcave, List<Vector2[]> pSurfaceList,
        string pName, Transform parent, float zThickness, Vector2 pUvScale)
    {
        GameObject lOut = new GameObject(pName);
        //lOut.active = false;
        lOut.transform.parent = parent;
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        Mesh lMesh = new Mesh();

        lMeshFilter.sharedMesh = lMesh;

        var lEdgeList = new List<Vector2[]>(pConcave.getHoleNum() + 1);
        lEdgeList.Add(pConcave.getOutSidePolygon().getShape());
        foreach (var lHole in pConcave.getHole())
        {
            lEdgeList.Add(lHole.getShape());
        }
        zzFlatMeshUtility.draw(lMesh, pSurfaceList, lEdgeList, zThickness, pUvScale);
        return lOut;
    }
}