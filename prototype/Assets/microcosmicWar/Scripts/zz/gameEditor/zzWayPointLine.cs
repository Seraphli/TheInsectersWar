using UnityEngine;

public class zzWayPointLine : P2PLine
{
    public Transform begin;

    //public Vector3 beginPointPos;

    public Transform end;

    public float lineZ = -3f;

    //public Vector3 endPointPos;
    void Start()
    {
        beginPosition =getVector( begin.position);
        endPosition = getVector( end.position);
    }

    public void setPoints(zzWayPoint pPointFrom,zzWayPoint pPointTo)
    {
        begin = pPointFrom.transform;
        end = pPointTo.transform;
        pPointFrom.addNextPoint(pPointTo);
        print(begin.position);
        print(end.position);
    }

    void Update()
    {
        if( !begin | !end)
        {
            Destroy(this);
            return;
        }

        if (beginPosition != getVector( begin.position))
            beginPosition = getVector( begin.position);
        if (endPosition !=getVector(  end.position))
            endPosition = getVector( end.position);
    }

    Vector3 getVector(Vector3 pPos)
    {
        pPos.z = lineZ;
        return pPos;
    }

    void OnDestory()
    {
        if(begin&&end)
            begin.GetComponent<zzWayPoint>()
                .removeNextPoint(end.GetComponent<zzWayPoint>());
    }
}