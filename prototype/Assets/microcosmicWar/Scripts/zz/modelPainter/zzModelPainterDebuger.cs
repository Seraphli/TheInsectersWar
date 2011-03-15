using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzModelPainterDebuger : zzIModelPainterProcessor
{
    public GameObject polygonDebugers;
    public GameObject pictureDebuger;

    public override void showPicture()
    {
        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(picture, pictureDebuger);
    }

    public override void pickPicture()
    {
        pictureDebuger = deleteOldCreateNewDebuger(pictureDebuger, "pictureDebuger");
        zzFlatMeshUtility.showPicture(activeChart.asTexture(), pictureDebuger);
    }

    public override void sweepPicture()
    {
        var lDebugerObject = deleteOldCreateNewDebuger();
        int i = 0;
        foreach (var lConcave in concaves)
        {
            zz2DConcaveDebuger.createDebuger(lConcave,
                "Picture" + i, lDebugerObject.transform);
            ++i;
        }
    }

    public override void convexDecompose()
    {
        var lDebugerObject = deleteOldCreateNewDebuger();
        int i = 0;
        foreach (var lDecomposed in convexesList)
        {
            string lName = "convex" + i + "Sub";
            zzSimplyPolygonDebuger
                .createDebuger(lDecomposed, lName, lDebugerObject.transform);
            ++i;
        }
    }

    public override void draw()
    {
    }

    public override void clear()
    {
        if (polygonDebugers)
            GameObject.DestroyImmediate(polygonDebugers);
        if (pictureDebuger)
            GameObject.DestroyImmediate(pictureDebuger);

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

}