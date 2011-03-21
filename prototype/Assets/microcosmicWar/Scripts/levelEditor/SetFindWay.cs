using UnityEngine;
using System.Collections;

public class SetFindWay:MonoBehaviour
{
    //public Transform nodes;

    public void ToSet()
    {
        var lWaySearcher = AstarPath.active;
        //lWaySearcher.listRootNode = nodes;
        print(lWaySearcher.listRootNode.childCount);
        lWaySearcher.Scan(true, 0);
    }
}