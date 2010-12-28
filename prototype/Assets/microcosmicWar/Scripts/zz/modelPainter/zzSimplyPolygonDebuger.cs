using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class zzSimplyPolygonDebuger : MonoBehaviour
{
    public zzSimplyPolygon polygon
    {
        get { return _polygon; }
        set
        {
            _polygon = value;
            pointNumber = _polygon.pointNum;
            points = new Vector2[pointNumber];

            int i = 0;
            var lNode = polygon.getAllPoints().First;
            while (lNode.Next != null)
            {
                points[i] = lNode.Value.position;
                lNode = lNode.Next;
                ++i;
            }
        }
    }

    public zzSimplyPolygon _polygon;

    public bool showPoint = true;

    public int pointNumber;

    public Color lineColor = Color.red;
    public Color selectedLineColor = Color.white;

    public Vector2[] points;

    Vector3 toV3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0.0f);
    }

    void drawGizmos(Color pLine, Color pConcavePoint, Color pConvexPoints)
    {
        if (polygon == null || !showPoint)
            return;

        Vector3 position = transform.position;
        var lNode = polygon.getAllPoints().First;
        Gizmos.color = pLine;
        while (lNode.Next != null)
        {
            Gizmos.DrawLine(
                position + toV3(lNode.Value.position),
                position + toV3(lNode.Next.Value.position));
            lNode = lNode.Next;

        }
        Gizmos.DrawLine(
            position + toV3(lNode.Value.position),
            position + toV3(polygon.getAllPoints().First.Value.position));


        Gizmos.color = pConcavePoint;
        foreach (var lConcavePoint in polygon.getConcavePoints())
        {
            Gizmos.DrawSphere(position + toV3(lConcavePoint.position), 0.3f);
        }
        Gizmos.color = pConvexPoints;
        foreach (var lConcavePoint in polygon.getConvexPoints())
        {
            Gizmos.DrawSphere(position + toV3(lConcavePoint.position), 0.3f);
        }

    }

    void OnDrawGizmos()
    {
        drawGizmos(lineColor, Color.blue, Color.green);
    }

    void OnDrawGizmosSelected()
    {
        drawGizmos(selectedLineColor, Color.blue, Color.green);
    }

    public static zzSimplyPolygon   toPolygon(zzPainterPoint pPoint)
    {
        zzSimplyPolygon lOut = new zzSimplyPolygon();
        List<Vector2> lPoints = new List<Vector2>();
        zzPainterPoint lNowPoint = pPoint;
        do
        {
            Vector3 l3DPoint = lNowPoint.transform.position;
            lPoints.Add(new Vector2(l3DPoint.x, l3DPoint.y));
            lNowPoint = lNowPoint.nextPoint;
        }
        while (lNowPoint != pPoint);
        lOut.setShape(lPoints.ToArray());
        return lOut;
    }

    public static GameObject createDebuger(zzSimplyPolygon pPolygon, string pName)
    {

        GameObject lPolygonDebugerObject = new GameObject(pName);
        zzSimplyPolygonDebuger lPolygonDebuger = lPolygonDebugerObject.AddComponent<zzSimplyPolygonDebuger>();

        lPolygonDebuger.polygon = pPolygon;
        return lPolygonDebugerObject;

    }

    public static GameObject createDebuger(zzSimplyPolygon pPolygon, string pName,Transform parent)
    {
        GameObject lOut = createDebuger(pPolygon, pName);
        lOut.transform.parent = parent;
        return lOut;
    }

    public static GameObject createDebuger(zzSimplyPolygon pPolygon, string pName,Transform parent, Color lDebugLineColor)
	{
        GameObject lOut = createDebuger(pPolygon, pName);
        lOut.transform.parent = parent;
		lOut.GetComponent<zzSimplyPolygonDebuger>().lineColor = lDebugLineColor;
        return lOut;
	}


    public static GameObject[] createDebuger(zzSimplyPolygon[] pPolygons, string pName)
    {
        GameObject[] lOut = new GameObject[pPolygons.Length];
        for(int i=0;i<pPolygons.Length;++i)
        {
            lOut[i]=createDebuger(pPolygons[i], pName + i);
        }
        return lOut;
    }
    public static GameObject[] createDebuger(zzSimplyPolygon[] pPolygons, string pName,Transform parent)
    {
        GameObject[] lOut = createDebuger(pPolygons, pName);
        foreach (var lObject in lOut)
        {
            lObject.transform.parent = parent;
        }
        return lOut;
    }

    public static GameObject[] createDebuger(zz2DConcave pConcave, string pName)
    {
        List<GameObject>    lOut = new List<GameObject>();
        lOut.Add(createDebuger(pConcave.getOutSidePolygon(), pName + "OutSide"));
        lOut.AddRange(createDebuger(pConcave.getHole(), pName + "Hole"));
        return lOut.ToArray();
    }
    public static GameObject[] createDebuger(zz2DConcave pConcave, string pName, Transform parent)
    {
        GameObject[] lOut = createDebuger(pConcave, pName);
        foreach (var lObject in lOut)
        {
            lObject.transform.parent = parent;
        }
        return lOut;
    }
}