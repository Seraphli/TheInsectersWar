using UnityEngine;
using System.Collections.Generic;

public class zzWayPointAutoLineShow : MonoBehaviour
{

    public Transform pointParent;

    public Transform lineParent;

    public float neighborRange = 20f;

    public float maxHeigth = 5f;

    public LayerMask pointMask;

    public LayerMask preventMask;

    //public float lineZ = -3f;

    public float lineWidth = 0.1f;

    public Material lineMaterial;

    public int lineLayer = 0;

    //List<LineRenderer> lines = new List<LineRenderer>();

    class NeighborInfo
    {
        Dictionary<zzWayPoint, HashSet<zzWayPoint>>
            data = new Dictionary<zzWayPoint, HashSet<zzWayPoint>>();

        public HashSet<zzWayPoint> getNeighborData(zzWayPoint pPoint)
        {
            return getNeighborData(data, pPoint);
        }

        public static HashSet<zzWayPoint> getNeighborData(
            Dictionary<zzWayPoint, HashSet<zzWayPoint>> pData,
            zzWayPoint pPoint)
        {
            HashSet<zzWayPoint> lOut;
            if (pData.ContainsKey(pPoint))
                lOut = pData[pPoint];
            else
            {
                lOut = new HashSet<zzWayPoint>();
                pData[pPoint] = lOut;
            }
            return lOut;
        }

        public bool isNeighbor(zzWayPoint pPointA, zzWayPoint pPointB)
        {
            return getNeighborData(pPointA).Contains(pPointB);
        }

        public void addNeighbor(zzWayPoint pPointA, zzWayPoint pPointB)
        {
            getNeighborData(pPointA).Add(pPointB);
            getNeighborData(pPointB).Add(pPointA);
        }
    }

    [ContextMenu("update Line")]
    public void updateLine()
    {
        //lineParent.childCount;
        List<zzPair<Vector3>> lLines = new List<zzPair<Vector3>>();
        var lNeighborInfo = new NeighborInfo();
        foreach (Transform lPoint in pointParent)
        {
            var lWayPoint = lPoint.GetComponent<zzWayPoint>();
            if (!lWayPoint)
                continue;
            var lWayPointPos = lWayPoint.lineCenter;
            var lNeighbors = lWayPoint.getNeighbor(neighborRange, maxHeigth
                    , pointMask, preventMask);
            foreach (var lNeighbor in lNeighbors)
            {
                if (!lNeighborInfo.isNeighbor(lWayPoint, lNeighbor))
                {
                    lLines.Add(new zzPair<Vector3>(lWayPointPos, lNeighbor.lineCenter));
                    lNeighborInfo.addNeighbor(lWayPoint, lNeighbor);
                    //NeighborInfo
                }
            }
        }
        updateLine(lLines);
    }

    public void updateLine(List<zzPair<Vector3>> pLines)
    {
        if (lineParent.childCount >= pLines.Count)
        {
            int i = 0;
            for (; i < pLines.Count; ++i)
            {
                var lLineRender = lineParent.GetChild(i)
                        .GetComponent<LineRenderer>();
                setLine(pLines[i], lLineRender);
            }
            for (; i < lineParent.childCount; ++i)
            {
                Destroy(lineParent.GetChild(i).gameObject);
            }
        }
        else//if(lineParent.childCount<pLines.Count)
        {
            int i = 0;
            for (; i < lineParent.childCount; ++i)
            {
                var lLineRender = lineParent.GetChild(i)
                        .GetComponent<LineRenderer>();
                setLine(pLines[i], lLineRender);
            }
            for (; i < pLines.Count; ++i)
            {
                var lObject = new GameObject("Line");
                lObject.layer = lineLayer;
                setLine(pLines[i], lObject.AddComponent<LineRenderer>());
                lObject.transform.parent = lineParent;
            }
        }
    }

    void setLine(zzPair<Vector3> pLinePos, LineRenderer pLineRenderer)
    {
        pLineRenderer.SetPosition(0, pLinePos.left);
        pLineRenderer.SetPosition(1, pLinePos.right);
        pLineRenderer.SetWidth(lineWidth, lineWidth);
        pLineRenderer.material = lineMaterial;
    }
}