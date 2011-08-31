using UnityEngine;
using System.Collections;

public class ControlPointLine : P2PLine
{
    public Transform inPoint;
    public Transform outPoint;

    void Update()
    {
        beginPosition = outPoint.position;
        endPosition = inPoint.position;
    }
}