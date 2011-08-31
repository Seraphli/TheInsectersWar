using UnityEngine;
using System.Collections;


public class InPoint:MonoBehaviour
{
    public int pointId;

    public delegate void ProcessFunc(InPoint sender, float value);

    public delegate void ProcessFuncVoidArg();

    public delegate void ProcessFuncFloatArg(float value);

    public OutPoint connectPoint;

    event ProcessFunc inDataFunc;

    public void addProcessFunc(ProcessFunc pFunc)
    {
        inDataFunc += pFunc;
    }

    public void addProcessFuncVoidArg(ProcessFuncVoidArg pFunc)
    {
        inDataFunc += (InPoint sender, float value) => pFunc();
    }

    public void addProcessFuncFloatArg(ProcessFuncFloatArg pFunc)
    {
        inDataFunc += (InPoint sender, float value) => pFunc(value);
    }

    public void disconnect()
    {
        if (connectPoint)
            connectPoint.disconnect(this);
    }

    [SerializeField]
    float _powerValue;

    public ControlPointLine pointLine;

    public void showLine()
    {
        if(connectPoint )
        {
            if (!pointLine)
            {
                pointLine = ((GameObject)Instantiate(GameSystem.Singleton.controlPointLinePrefab)).
                      GetComponent<ControlPointLine>();
            }
            pointLine.inPoint = this.transform;
            pointLine.outPoint = connectPoint.transform;
        }
    }

    public void hideLine()
    {
        if (pointLine)
        {
            Destroy(pointLine.gameObject);
            pointLine = null;
        }
    }

    void Awake()
    {
        _powerValue = 0f;
    }

    public float powerValue
    {
        get { return _powerValue; }
    }

    public void send(float pValue)
    {
        _powerValue = pValue;
        inDataFunc(this, pValue);
    }
}