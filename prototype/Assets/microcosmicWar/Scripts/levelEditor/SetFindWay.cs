using UnityEngine;
using System.Collections.Generic;

public class SetFindWay:MonoBehaviour
{
    public Transform wayNodeLinkRoot;

    void Start()
    {
        ToSet();
    }

    public void ToSet()
    {
        var lWaySearcher = AstarPath.active;
        //lWaySearcher.listRootNode = nodes;
        print(lWaySearcher.listRootNode.childCount);
        var lNodeLinks = new List<AstarClasses.NodeLink>();
        foreach (Transform lLinkTransform in wayNodeLinkRoot)
        {
            var lWayPointLine = lLinkTransform.GetComponent<zzWayPointLine>();
            var lLink = new AstarClasses.NodeLink();
            lLink.oneWay = true;
            lLink.fromVector = lWayPointLine.begin.transform.position;
            lLink.toVector = lWayPointLine.end.transform.position;
            lNodeLinks.Add(lLink);
        }
        lWaySearcher.links = lNodeLinks.ToArray();
        lWaySearcher.Scan(true, 0);
    }
}