using UnityEngine;
using System.Collections;

class LogicAnd : zzEditableObject
{
    public InPoint[] _inPoints;
    public OutPoint outPoint;

    public bool[] lastOnOff;

    public int signalCount = 0;

    public override InPoint[] inPoints
    {
        get { return _inPoints; }
    }

    public void Start()
    {
        setInPointsId();
        lastOnOff = new bool[_inPoints.Length];
        foreach (var lPoint in _inPoints)
        {
            lPoint.addProcessFunc(receiveSignal);
            receiveSignal(lPoint);
        }
    }

    void receiveSignal(InPoint sender)
    {
        receiveSignal(sender, sender.powerValue);
    }

    void receiveSignal(InPoint sender, float value)
    {
        int lID = sender.pointId;
        if(value>=1f)
        {
            if(!lastOnOff[lID])
            {
                lastOnOff[lID] = true;
                if (++signalCount == _inPoints.Length)
                    outPoint.sendFull();
            }
        }
        else if (lastOnOff[lID])
        {
            lastOnOff[lID] = false;
            --signalCount;
            outPoint.sendNull();
        }
    }
}