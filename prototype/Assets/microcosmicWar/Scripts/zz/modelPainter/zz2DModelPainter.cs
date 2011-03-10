using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class zz2DModelPainter : MonoBehaviour
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
        nothing = 1,
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

    Renderer getRenderer(Transform pTransform)
    {
        Renderer lOut = null;
        //pTransform = pTransform.parent;
        while (!lOut && pTransform)
        {
            lOut = pTransform.GetComponent<Renderer>();
            pTransform = pTransform.parent;
        }
        return lOut;
    }

    public Camera painterCamera;

    void showPicture()
    {
        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(picture, pictureDebuger);
    }

    void pickPicture()
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

        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(activeChart.asTexture(), pictureDebuger);
        //lPicObject.transform.parent = pictureDebuger.transform;

    }

    zzActiveChart activeChart;

    public int polygonNumber = 0;
    public int holeNumber = 0;

    public Texture2D[] imagePatterns;

    public MeshRenderer imagePatternShow;
    public int imagePatternShowIndex;
    public zzPointBounds[] imagePatternBounds;
    void sweepPicture()
    {
        pointNumber = 0;
        polygonNumber = 0;
        holeNumber = 0;
        var lPatternResult = zzOutlineSweeper.sweeper(activeChart);
        imagePatterns = new Texture2D[lPatternResult.Count];
        imagePatternBounds = new zzPointBounds[lPatternResult.Count];
        for (int i = 0; i < lPatternResult.Count;++i )
        {
            zzPointBounds lBounds = lPatternResult.sweeperPointResults[i].Bounds;
            imagePatternBounds[i] = lBounds;
            var lBoundMin = lBounds.min;
            var lBoundMax = lBounds.max;
            zzPoint lPatternSize = new zzPoint(
                Mathf.NextPowerOfTwo(lBoundMax.x-lBoundMin.x+1),
                Mathf.NextPowerOfTwo(lBoundMax.y-lBoundMin.y+1)
                );
            imagePatterns[i] = zzImagePatternPicker.pick(lPatternResult.patternMark,i+1, 
                picture, lBounds, lPatternSize );
        }
        imagePatternShow.sharedMaterial.mainTexture = imagePatterns[imagePatternShowIndex];
        //var lSweeperResults = zzOutlineSweeper.sweeper(activeChart, ignoreDistanceInSweeping);
        var lSweeperResults = zzOutlineSweeper
            .simplifySweeperResult(lPatternResult.sweeperPointResults, ignoreDistanceInSweeping);
        modelsSize = new Vector2((float)activeChart.width, (float)activeChart.height);
        var lDebugerObject = deleteOldCreateNewDebuger();
        concaves = new List<zz2DConcave>();
        foreach (var lSweeperResult in lSweeperResults)
        {
            //zzSimplyPolygon lPolygon = addSimplyPolygon(lSweeperResult.edge, "Picture" + concaves.Count,
            //    lDebugerObject.transform);

            //if (lPolygon == null)
            //    continue;
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
            zz2DConcaveDebuger.createDebuger(lConcave,
                "Picture" + concaves.Count, lDebugerObject.transform);
            concaves.Add(lConcave);
        }
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

        zzSimplyPolygonDebuger
            .createDebuger(lPolygon, pName, pDebugerObject, lDebugLineColor);

        pointNumber += lPolygon.pointNum;
        return lPolygon;
    }

    void convexDecompose()
    {
        convexesList = new List<zzSimplyPolygon[]>();
        var lDebugerObject = deleteOldCreateNewDebuger();
        int index = 0;
        foreach (var lConcave in concaves)
        {
            string lName = "convex" + (index++) + "Sub";
            zzSimplyPolygon[] ldecomposed = lConcave.decompose();
            zzSimplyPolygonDebuger
                .createDebuger(ldecomposed, lName, lDebugerObject.transform);
            convexesList.Add(ldecomposed);
        }
    }

    static Mesh _planeMesh;

    static Mesh planeMesh
    {
        get
        {
            if (!_planeMesh)
            {
                _planeMesh = new Mesh();
                // The vertices of mesh
                // 3--2
                // |  |
                // 0--1
                _planeMesh.vertices = new Vector3[]{
                    new Vector3(0,0,0),
                    new Vector3(1,0,0),
                    new Vector3(1,1,0),
                    new Vector3(0,1,0),
                };
                _planeMesh.uv = new Vector2[]{ 
                    new Vector2(0, 0f), 
                    new Vector2(1, 0),
                    new Vector2(1, 1f), 
                    new Vector2(0, 1f),
                };
                _planeMesh.triangles = new int[] { 0, 2, 1, 3, 2, 0, };
                _planeMesh.normals = new Vector3[]{
                    new Vector3(0,0,-1),
                    new Vector3(0,0,-1),
                    new Vector3(0,0,-1),
                    new Vector3(0,0,-1),
                };
            }

            return _planeMesh;
        }
    }

    void draw()
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

            var lRenderObject = new GameObject("Render");
            MeshFilter lMeshFilter = lRenderObject.AddComponent<MeshFilter>();
            lMeshFilter.sharedMesh = planeMesh;
            MeshRenderer lMeshRenderer = lRenderObject.AddComponent<MeshRenderer>();
            var lMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            var lImage = imagePatterns[i];
            lMaterial.mainTexture = lImage;
            lMeshRenderer.sharedMaterial = lMaterial;

            var lBounds = imagePatternBounds[i];
            var lMin = lBounds.min;
            var lMax = lBounds.max;

            //lRenderObject.AddComponent<zzFlatMeshEdit>();
            Vector3 lCenter = new Vector3(
                ((float)lMin.x + (float)lMax.x) / 2f,
                ((float)lMin.y + (float)lMax.y) / 2f,
                0f
            );
            //lConvexsObject是lRenderObject的父物体
            lConvexsObject.transform.position += lCenter;
            lRenderObject.transform.localScale = new Vector3(
                lImage.width,
                lImage.height,
                1f);
            lRenderObject.transform.position = 
                new Vector3(lMin.x - 0.5f, lMin.y - 0.5f, 0.5f);

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
            lRenderObject.transform.parent = lConvexsObject.transform;
        }

    }

    public GameObject models;
    public Vector2 modelsSize;


    [ContextMenu("Step")]
    public bool doStep()
    {
        int lStepValue = (int)step;
        if (lStepValue < (int)Step.clear)
            step = (Step)(lStepValue + 1);
        switch (step)
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


    GameObject deleteOldCreateNewDebuger()
    {
        if (polygonDebugers)
            GameObject.DestroyImmediate(polygonDebugers);
        polygonDebugers = new GameObject("polygonDebugers");
        return polygonDebugers;
    }

    GameObject deleteOldCreateNewDebuger(GameObject pObject, string pName)
    {
        if (pObject)
            GameObject.DestroyImmediate(pObject);
        return new GameObject(pName);
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

        return lOut;
    }

}