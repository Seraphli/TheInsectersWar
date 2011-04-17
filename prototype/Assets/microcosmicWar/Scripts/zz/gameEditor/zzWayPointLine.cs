using UnityEngine;

public class zzWayPointLine : P2PLine
{
    public Transform begin;

    //public Vector3 beginPointPos;

    public Transform end;

    //public Vector3 endPointPos;
    void Start()
    {
        beginPosition = begin.position;
        endPosition = end.position;
    }

    void Update()
    {
        if( !begin | !end)
        {
            Destroy(this);
            return;
        }

        if (beginPosition != begin.position)
            beginPosition = begin.position;
        if (endPosition != end.position)
            endPosition = end.position;
    }

    void OnDestory()
    {
        if(begin&&end)
            begin.GetComponent<zzWayPoint>()
                .removeNextPoint(end.GetComponent<zzWayPoint>());
    }
}