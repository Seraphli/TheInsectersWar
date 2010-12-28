using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class zz2DConcaveDebuger : MonoBehaviour
{
    zz2DConcave concave;
    public Color outLineColor;
    public Color holeLineColor = Color.yellow;

    void setConcave(zz2DConcave pConcave)
    {
        concave = pConcave;
        gameObject.AddComponent<zzSimplyPolygonDebuger>().polygon = concave.getOutSidePolygon();
        foreach (var lHole in concave.getHole())
        {
            var lPolygonDebuger = gameObject.AddComponent<zzSimplyPolygonDebuger>();
            lPolygonDebuger.polygon = lHole;
            lPolygonDebuger.lineColor = holeLineColor;
        }
    }

    [ContextMenu("step decompose")]
    void stepDecompose()
    {
        if (!concave.isConvex)
        {
            var lPolygonDebugerList = gameObject.GetComponents<zzSimplyPolygonDebuger>();
            foreach (var lPolygonDebuger in lPolygonDebugerList)
            {
                Object.DestroyImmediate(lPolygonDebuger);
            }
            var lDecomposedConcaves = concave.stepDecompose();
            int i = 0;
            foreach (var lConcave in lDecomposedConcaves)
            {
                createDebuger(lConcave, i.ToString(), transform);
                ++i;
            }
        }
    }

    public static GameObject createDebuger(zz2DConcave pPolygon, string pName)
    {

        GameObject lPolygonDebugerObject = new GameObject(pName);
        zz2DConcaveDebuger lPolygonDebuger = lPolygonDebugerObject.AddComponent<zz2DConcaveDebuger>();

        lPolygonDebuger.setConcave(pPolygon);
        return lPolygonDebugerObject;

    }

    public static GameObject createDebuger(zz2DConcave pPolygon, string pName, Transform parent)
    {
        GameObject lOut = createDebuger(pPolygon, pName);
        lOut.transform.parent = parent;
        return lOut;
    }
}