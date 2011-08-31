using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutPoint : MonoBehaviour
{

    public int pointId;

    public InPoint[] connectPoints = new InPoint[0];

    List<InPoint> mConnectPoints = new List<InPoint>();

    [SerializeField]
    float _powerValue;

    public void connect(InPoint pInPoint)
    {
        pInPoint.disconnect();
        pInPoint.connectPoint = this;
        mConnectPoints.Add(pInPoint);
        connectPoints = mConnectPoints.ToArray();
    }

    public void disconnect(InPoint pInPoint)
    {
        for (int i = 0; i < mConnectPoints.Count; ++i)
        {
            if (pInPoint == connectPoints[i])
            {
                pInPoint.connectPoint = null;
                mConnectPoints.RemoveAt(i);
                connectPoints = mConnectPoints.ToArray();
                return;
            }
        }
        Debug.LogError("disconnect(InPoint pInPoint)");
    }

    void Awake()
    {
        _powerValue = 0f;
        var lPoints = connectPoints;
        foreach (var lPoint in lPoints)
        {
            connect(lPoint);
        }
    }

    public float powerValue
    {
        get { return _powerValue; }
    }

    public void sendFull()
    {
        send(1.0f);
    }

    public void sendNull()
    {
        send(0.0f);
    }

    public void send(float pValue)
    {
        if (pValue == _powerValue)
            return;
        //print(name + ":" + pValue);
        _powerValue = pValue;
        foreach (var lInPoint in connectPoints)
        {
            lInPoint.send(pValue);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 lSelfPos = transform.position;
        Gizmos.color = Color.blue;
        foreach (var lInPoint in connectPoints)
        {
            if (lInPoint)
                Gizmos.DrawLine(lInPoint.transform.position, lSelfPos);
        }
    }
}