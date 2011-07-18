using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class zzModelPainterProcessor : zzIModelPainterProcessor
{

    public enum SweepMode
    {
        ignoreColor,
        designatedColor,
        ignoreAlphaZero,
    }
    //Texture2D prePicture;
    public float ignoreDistanceInSweeping = 1.7f;
    public float thickness = 10.0f;

    public SweepMode sweepMode;

    public Color colorInSweepSetting;

    public override void showPicture() { }

    public override void sweepPicture()
    {
        pointNumber = 0;
        polygonNumber = 0;
        holeNumber = 0;
        var lPatternResult = zzOutlineSweeper.sweeper(activeChart);
        imagePatterns = new Texture2D[lPatternResult.Count];
        imagePatternBounds = new zzPointBounds[lPatternResult.Count];

        //拾取图块
        for (int i = 0; i < lPatternResult.Count; ++i)
        {
            zzPointBounds lBounds = lPatternResult.sweeperPointResults[i].Bounds;
            imagePatternBounds[i] = lBounds;
            var lBoundMin = lBounds.min;
            var lBoundMax = lBounds.max;
            zzPoint lPatternSize = new zzPoint(
                Mathf.NextPowerOfTwo(lBoundMax.x - lBoundMin.x + 1),
                Mathf.NextPowerOfTwo(lBoundMax.y - lBoundMin.y + 1)
                );
            imagePatterns[i] = zzImagePatternPicker.pick(lPatternResult.patternMark, i + 1,
                picture, lBounds, lPatternSize);
        }
        //var lSweeperResults = zzOutlineSweeper.sweeper(activeChart, ignoreDistanceInSweeping);
        var lSweeperResults = zzOutlineSweeper
            .simplifySweeperResult(lPatternResult.sweeperPointResults, ignoreDistanceInSweeping);
        modelsSize = new Vector2((float)activeChart.width, (float)activeChart.height);

        //存储结果
        concaves = new List<zz2DConcave>();
        var lNewImagePatterns =new List<Texture2D>(lPatternResult.Count);
        var lNewPatternBounds = new List<zzPointBounds>(lPatternResult.Count);
        for (int i = 0; i < lSweeperResults.Count; ++i)
        {
            var lSweeperResult = lSweeperResults[i];
            var lImage = imagePatterns[i];
            if (lSweeperResult.edge.Length < 2 || lImage.width < 3 || lImage.height<3)
                continue;
            lNewImagePatterns.Add(lImage);
            lNewPatternBounds.Add(imagePatternBounds[i]);
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

        imagePatterns = lNewImagePatterns.ToArray();
        imagePatternBounds = lNewPatternBounds.ToArray();
    }

    public override void pickPicture()
    {
        activeChart = new zzActiveChart(picture.width, picture.height);
        if (sweepMode == SweepMode.ignoreColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) != colorInSweepSetting);
                }
            }

        }
        else if (sweepMode == SweepMode.designatedColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) == colorInSweepSetting);
                }
            }

        }
        else //if (sweepMode == SweepMode.ignoreAlphaZero)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y).a != 0);
                }
            }

        }
        zzOutlineSweeper.removeIsolatedPoint(activeChart);

    }


    public override void clear()
    {
        DestroyImmediate(models);
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject)
    {
        return addSimplyPolygon(pPoints, pName, pDebugerObject, Color.red);
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject, Color lDebugLineColor)
    {
        if (pPoints.Length < 3)
            return null;

        zzSimplyPolygon lPolygon = new zzSimplyPolygon();
        lPolygon.setShape(pPoints);

        pointNumber += lPolygon.pointNum;
        return lPolygon;
    }

    public override void convexDecompose()
    {
        convexesList = new List<zzSimplyPolygon[]>();
        int index = 0;
        foreach (var lConcave in concaves)
        {
            zzSimplyPolygon[] ldecomposed = lConcave.decompose();
            convexesList.Add(ldecomposed);
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
            bool lAvailable = false;
            foreach (var lConvex in lConvexs)
            {
                var lConvexShape = lConvex.getShape();
                if (lConvexShape.Length > 2)
                    lAvailable = true;
                lSurfaceList.Add(lConvexShape);
            }
            //不生成不可用的模型,对之前步骤的失败的处理
            if(!lAvailable)
            {
                Debug.LogError("failed model index:" + i);
                ++i;
                continue;
            }
            var lImage = imagePatterns[i];

            string lPolygonName = "polygon" + i;
            GameObject lConvexsObject = new GameObject(lPolygonName);
            lConvexsObject.transform.parent = models.transform;
            var lMin = imagePatternBounds[i].min;
            Vector2 lPointOffset = new Vector2(-lMin.x, -lMin.y);
            var lPlanePos = new Vector3(lMin.x, lMin.y, 0f);
            var lRenderObject = createFlatMesh(concaves[i], lSurfaceList, "Render",
                    lPointOffset,
                    thickness,
                    new Vector2(1.0f / (float)(lImage.width - 1),
                        1.0f / (float)(lImage.height - 1))
                    );

            MeshRenderer lMeshRenderer = lRenderObject.GetComponent<MeshRenderer>();
            var lMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            lMaterial.mainTexture = lImage;
            lMeshRenderer.sharedMaterial = lMaterial;
            Vector3 lCenter = lMeshRenderer.bounds.center;
            lCenter.z = 0;
            //lConvexsObject是lRenderObject的父物体
            lConvexsObject.transform.position += (lCenter + lPlanePos);
            //lPlanePos.z = thickness / 2f;
            lRenderObject.transform.position = lPlanePos;

            ++i;

            int lSubIndex = 0;
            string lSubName = "Collider";
            foreach (var lConvex in lConvexs)
            {

                var lColliderObject = createFlatCollider(lConvex.getShape(),
                    lSubName + lSubIndex,
                    lConvexsObject.transform, thickness);
                ++lSubIndex;
            }
            lRenderObject.transform.parent = lConvexsObject.transform;
        }

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

    //创建的模型位移 -pBounds.min
    GameObject createFlatMesh(zz2DConcave pConcave, List<Vector2[]> pSurfaceList,
        string pName, Vector2 pPointOffset, float zThickness, Vector2 pUvScale)
    {
        Debug.Log(pName+" UvScale:"+pUvScale.ToString("f8")+" PointOffset:"+pPointOffset.ToString("f8"));
        GameObject lOut = new GameObject(pName);
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        MeshRenderer lMeshRenderer = lOut.AddComponent<MeshRenderer>();
        Mesh lMesh = new Mesh();

        if (Application.isPlaying)
            lMeshFilter.mesh = lMesh;
        else
            lMeshFilter.sharedMesh = lMesh;

        var lEdgeList = new List<Vector2[]>(pConcave.getHoleNum() + 1);
        lEdgeList.Add(pConcave.getOutSidePolygon().getShape());
        foreach (var lHole in pConcave.getHole())
        {
            lEdgeList.Add(lHole.getShape());
        }

        draw(lMesh, pSurfaceList, lEdgeList, zThickness, pUvScale, pPointOffset);
        return lOut;
    }

    protected abstract void draw(Mesh pMesh, List<Vector2[]> pSurfaceList,
        List<Vector2[]> pEdgeList, float zThickness, Vector2 pUvScale,
        Vector2 pPointOffset);

}