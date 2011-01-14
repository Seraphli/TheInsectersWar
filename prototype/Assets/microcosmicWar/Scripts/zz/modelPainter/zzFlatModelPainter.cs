using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class zzFlatModelPainter : MonoBehaviour
{
    public Texture2D picture;
    //Texture2D prePicture;
    public float ignoreDistanceInSweeping = 1.7f;
    public float thickness = 10.0f;

    public GameObject polygonDebugers;
    public GameObject pictureDebuger;

    List<zz2DConcave> concaves;
    List<Dictionary<Vector2, int>> pointToIndexList;
    List<zzSimplyPolygon[]> convexesList;
    //List<GameObject> polygonDebugers;

    enum Step
    {
        nothing=1,
        showPocture,
        pickPicture,
        sweepPicture,
        convexDecompose,
        draw,
        clear,
    }

    public enum SweepMode
    {
        ignoreColor,
        designatedColor,
        ignoreAlphaZero,
    }

    [SerializeField]
    Step step = Step.nothing;

    public int pointNumber = 0;

    public SweepMode sweepMode;

    public Color colorInSweepSetting;

    [ContextMenu("clear")]
    public void clear()
    {
        step = Step.nothing;
        if (polygonDebugers)
            GameObject.DestroyImmediate(polygonDebugers);
        if (pictureDebuger)
            GameObject.DestroyImmediate(pictureDebuger);
    }

    Renderer    getRenderer(Transform pTransform)
    {
        Renderer lOut = null;
        //pTransform = pTransform.parent;
        while(!lOut&&pTransform)
        {
            lOut = pTransform.GetComponent<Renderer>();
            pTransform = pTransform.parent;
        }
        return lOut;
    }

    public Color pickedColor;

    bool    pickColor(Ray pRay,out Color pColor)
    {
        RaycastHit  lRaycastHit;
        pColor = Color.clear;
        if(Physics.Raycast (pRay, out lRaycastHit))
        {
            Renderer lRenderer = getRenderer(lRaycastHit.collider.transform);
            if (
                lRenderer == null
                || lRenderer.material == null
                || lRenderer.material.mainTexture == null
                )
            return false;

            Texture2D  lTexture =(Texture2D) lRenderer.material.mainTexture;

            var lPixelUV = lRaycastHit.textureCoord;

            pColor = lTexture
                .GetPixel((int)(lPixelUV.x * lTexture.width), (int)(lPixelUV.y * lTexture.height));
            return true;
        }
        return false;
    }

    public Camera painterCamera;

    //void Update()
    //{
    //    Color lColor;
    //    if (
    //        Input.GetMouseButton (0)
    //        && pickColor(painterCamera.ScreenPointToRay(Input.mousePosition), out lColor)
    //        )
    //        pickedColor = lColor;
    //}

    void showPicture()
    {
        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(picture, pictureDebuger);
    }

    void pickPicture()
    {
        activeChart = new zzOutlineSweeper.ActiveChart(picture.width, picture.height);
        if (sweepMode== SweepMode.ignoreColor)
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
                    //Color lColor =picture.GetPixel(x, y);
                    //if (lColor != Color.clear && lColor!=Color.black)
                    //    print(lColor);
                }
            }

        }
        else //if (sweepMode == SweepMode.ignoreAlphaZero)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y).a!=0);
                    //Color lColor =picture.GetPixel(x, y);
                    //if (lColor != Color.clear && lColor!=Color.black)
                    //    print(lColor);
                }
            }

        }
        zzOutlineSweeper.removeIsolatedPoint(activeChart);

        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(activeChart.asTexture(), pictureDebuger);
        //lPicObject.transform.parent = pictureDebuger.transform;

    }

    zzOutlineSweeper.ActiveChart activeChart;

    public int polygonNumber = 0;
    public int holeNumber = 0;

    void    sweepPicture()
    {
        pointNumber = 0;
        polygonNumber = 0;
        holeNumber = 0;
        var lSweeperResults = zzOutlineSweeper.sweeper(activeChart, ignoreDistanceInSweeping);
        modelsSize = new Vector2((float)activeChart.width, (float)activeChart.height);
        var lDebugerObject = deleteOldCreateNewDebuger();
        concaves = new List<zz2DConcave>();
        foreach (var lSweeperResult in lSweeperResults)
        {
            //zzSimplyPolygon lPolygon = addSimplyPolygon(lSweeperResult.edge, "Picture" + concaves.Count,
            //    lDebugerObject.transform);

            //if (lPolygon == null)
            //    continue;
            if(lSweeperResult.edge.Length<2)
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
            zz2DConcaveDebuger.createDebuger(lConcave,
                "Picture" + concaves.Count, lDebugerObject.transform);
            concaves.Add(lConcave);
        }
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject)
	{
		return addSimplyPolygon( pPoints,  pName,  pDebugerObject, Color.red);
	}

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject, Color lDebugLineColor)
    {
        if (pPoints.Length < 3)
            return null;

        zzSimplyPolygon lPolygon = new zzSimplyPolygon();
        lPolygon.setShape(pPoints);

        zzSimplyPolygonDebuger
            .createDebuger(lPolygon, pName, pDebugerObject,lDebugLineColor);

        pointNumber += lPolygon.pointNum;
        return lPolygon;
    }

    void    convexDecompose()
    {
        convexesList = new List<zzSimplyPolygon[]>();
        var lDebugerObject = deleteOldCreateNewDebuger();
        int index = 0;
        foreach (var lConcave in concaves)
        {
            string lName = "convex"+(index++)+"Sub";
            zzSimplyPolygon[] ldecomposed = lConcave.decompose();
            zzSimplyPolygonDebuger
                .createDebuger(ldecomposed, lName, lDebugerObject.transform);
            convexesList.Add(ldecomposed);
        }
    }

    void    draw()
    {
        models = new GameObject("PaintModel");
        models.transform.position = new Vector3(modelsSize.x / 2.0f, modelsSize.y / 2.0f, 0.0f);
        int i = 0;
        foreach (var lConvexs in convexesList)
        {
            //var lPointToIndexMap = new Dictionary<Vector2, int>();
            //getPointToIndexMap(concaves[i], lPointToIndexMap, 0);
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
            Vector3 lCenter = lRenderObject.GetComponent<MeshRenderer>().bounds.center;
            lCenter.z = 0;
            //lConvexsObject是lRenderObject的父物体
            lConvexsObject.transform.position += lCenter;
            lRenderObject.transform.position -= lCenter;

            ++i;

            int lSubIndex = 0;
            string lSubName = "Collider";
            foreach (var lConvex in lConvexs)
            {
                //createFlatMesh(lConvex.getShape(),
                //    lSubName + lSubIndex,
                //    lConvexsObject.transform, thickness )
                //        .GetComponent<Renderer>().enabled = false;
                //print(lSubIndex);
                var lColliderObject = createFlatCollider(lConvex.getShape(),
                    lSubName + lSubIndex,
                    lConvexsObject.transform, thickness);
                //因为是先创建,后关联父级的,所以不用移动
                //lColliderObject.transform.position -= lCenter;
                ++lSubIndex;
            }
        }

    }

    public GameObject models;
    public Vector2 modelsSize;

    //public static void pivotToCenter(GameObject pObject)
    //{
    //    var bounds = pObject.GetComponent<MeshFilter>().sharedMesh.bounds;
    //    var lPivotToCenter = bounds.center;
    //    lPivotToCenter.z = 0;
    //    pObject.transform.position = pObject.transform.position + lPivotToCenter;
    //    //var lExtent = bounds.extents;
    //    //lExtent.z = 0;
    //    foreach (Transform lSub in pObject.transform)
    //    {
    //        lSub.position = lSub.position - lPivotToCenter;
    //    }
    //}

    //int getPointToIndexMap(zz2DConcave pConcave, Dictionary<Vector2, int> pOut,int pBeginIndex)
    //{
    //    pBeginIndex = getPointToIndexMap(pConcave.getOutSidePolygon().getShape(), pOut, pBeginIndex);
    //    foreach (var lHoles in pConcave.getHole())
    //    {
    //        pBeginIndex = getPointToIndexMap(lHoles.getShape(), pOut, pBeginIndex);
    //    }
    //    return pBeginIndex;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pPoints"></param>
    /// <param name="pOut"></param>
    /// <param name="pBeginIndex"></param>
    /// <returns>返回结束的未被使用的索引</returns>
    //int getPointToIndexMap(Vector2[] pPoints, Dictionary<Vector2, int> pOut, int pBeginIndex)
    //{
    //    foreach (var lPoint in pPoints)
    //    {
    //        pOut[lPoint] = pBeginIndex;
    //        ++pBeginIndex;
    //    }
    //    return pBeginIndex;
    //}

    [ContextMenu("Step")]
    public bool    doStep()
    {
        int lStepValue = (int)step ;
        if (lStepValue < (int)Step.clear )
            step = (Step)(lStepValue + 1);
        switch(step)
        {
            case Step.showPocture:
                showPicture();
                break;
            case Step.pickPicture:
                pickPicture();
                break;
            case Step.sweepPicture:
                sweepPicture();
                break;
            case Step.convexDecompose:
                convexDecompose();
                break;
            case Step.draw:
                draw();
                break;
            case Step.clear:
                clear();
                return false;
        }
        return true;
    }


    GameObject  deleteOldCreateNewDebuger()
    {
        if (polygonDebugers)
            GameObject.DestroyImmediate(polygonDebugers);
        polygonDebugers = new GameObject("polygonDebugers");
        return polygonDebugers;
    }

    GameObject deleteOldCreateNewDebuger(GameObject pObject,string pName)
    {
        if (pObject)
            GameObject.DestroyImmediate(pObject);
        return new GameObject(pName);
    }

    static GameObject createFlatMesh(Vector2[] points, string pName, Transform parent, float zThickness)
    {
        GameObject lOut = new GameObject(pName);
        lOut.transform.parent = parent;
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        MeshRenderer lMeshRenderer = lOut.AddComponent<MeshRenderer>();
        MeshCollider lMeshCollider = lOut.AddComponent<MeshCollider>();
        Mesh lMesh = new Mesh();
        zzFlatMeshUtility.draw(lMesh, points, zThickness);
        if (Application.isPlaying)
        {
            lMeshFilter.mesh = lMesh;
        }
        else
        {
            lMeshFilter.sharedMesh = lMesh;
        }

        lMeshCollider.sharedMesh = lMesh;

        lMeshCollider.convex = true;
        //MeshFilter lMeshFilter = lOut
        return lOut;
    }


    static GameObject createFlatCollider(Vector2[] points, string pName, Transform parent, float zThickness)
    {
        GameObject lOut = new GameObject(pName);
        lOut.transform.parent = parent;
        MeshCollider lMeshCollider = lOut.AddComponent<MeshCollider>();
        Mesh lMesh = new Mesh();
        zzFlatMeshUtility.draw(lMesh, points, zThickness);
        lMeshCollider.convex = true;
        //print("zzFlatMeshUtility.draw(lMesh, points, zThickness)");
        
        lMeshCollider.sharedMesh = lMesh;

        //print("lMeshCollider.sharedMesh = lMesh");
        //print("lMeshCollider.convex = true");
        //MeshFilter lMeshFilter = lOut
        return lOut;
    }

    static GameObject createFlatMesh(zz2DConcave pConcave, List<Vector2[]> pSurfaceList,
        string pName, Transform parent, float zThickness, Vector2 pUvScale)
    {
        GameObject lOut = new GameObject(pName);
        lOut.transform.parent = parent;
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        MeshRenderer lMeshRenderer = lOut.AddComponent<MeshRenderer>();
        Mesh lMesh = new Mesh();

        if (Application.isPlaying)
            lMeshFilter.mesh = lMesh;
        else
            lMeshFilter.sharedMesh = lMesh;

        var lEdgeList = new List<Vector2[]>(pConcave.getHoleNum()+1);
        lEdgeList.Add(pConcave.getOutSidePolygon().getShape());
        foreach (var lHole in pConcave.getHole())
        {
            lEdgeList.Add(lHole.getShape());
        }
        zzFlatMeshUtility.draw(lMesh, pSurfaceList, lEdgeList, zThickness, pUvScale);
        return lOut;
    }
}