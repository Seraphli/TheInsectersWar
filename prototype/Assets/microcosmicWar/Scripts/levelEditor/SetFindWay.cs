using UnityEngine;
using System.Collections.Generic;

public class SetFindWay:MonoBehaviour
{

    public void ToSet()
    {
        var lWaySearcher = AstarPath.active;
        //lWaySearcher.listRootNode = nodes;
        print(lWaySearcher.listRootNode.childCount);
        var lNodeLinks = new List<AstarClasses.NodeLink>();
        foreach (Transform lWayPointTransform in lWaySearcher.listRootNode)
        {
            var lWayPoint = lWayPointTransform.GetComponent<zzWayPoint>();
            if (lWayPoint && lWayPoint.nextPoints.Length > 0)
            {
                var lFromPos = lWayPoint.transform.position;
                foreach (var lNextPoint in lWayPoint.nextPoints)
                {
                    var lLink = new AstarClasses.NodeLink();
                    lLink.oneWay = true;
                    lLink.fromVector = lFromPos;
                    lLink.toVector = lNextPoint.transform.position;
                    lNodeLinks.Add(lLink);
                }
            }
        }
        lWaySearcher.links = lNodeLinks.ToArray();
        lWaySearcher.Scan(true, 0);
    }
}