using UnityEngine;

public class zzWayPointLine : P2PLine
{
    public zzWayPoint begin;

    public zzWayPoint end;

    [zzSerialize]
    public int beginPointID
    {
        get
        {
            return begin.gameObject.GetInstanceID();
        }
        set
        {
            zzGetObjectByID.addSetMethod(
                (x) => begin = x.GetComponent<zzWayPoint>(),
                value
                );
        }
    }

    [zzSerialize]
    public int endPointID
    {
        get
        {
            return end.gameObject.GetInstanceID();
        }
        set
        {
            zzGetObjectByID.addSetMethod(
                (x) => end = x.GetComponent<zzWayPoint>(),
                value
                );
        }
    }

    void Start()
    {
        beginPosition = begin.lineCenter;
        endPosition = end.lineCenter;
        updateArrow();
        //begin.addNextPoint(end);
    }

    public void setPoints(zzWayPoint pPointFrom, zzWayPoint pPointTo)
    {
        begin = pPointFrom;
        end = pPointTo;
    }

    void Update()
    {
        if( !begin | !end)
        {
            Destroy(this);
            return;
        }

        if (beginPosition != begin.lineCenter)
        {
            beginPosition = begin.lineCenter;
            updateArrow();
        }
        if (endPosition != end.lineCenter)
        {
            endPosition = end.lineCenter;
            updateArrow();
        }
    }

    void updateArrow()
    {
        var lPosition = (beginPosition + endPosition)/2f;
        transform.position = lPosition;
        var lBeginToEnd = endPosition -  beginPosition;
        var lRotation = new Quaternion();
        lRotation.SetFromToRotation(Vector3.up, lBeginToEnd);
        transform.rotation = lRotation;
    }


    //void OnDestory()
    //{
    //    if(begin&&end)
    //        begin.removeNextPoint(end);
    //}
}