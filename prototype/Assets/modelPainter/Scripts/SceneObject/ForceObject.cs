using UnityEngine;
using System.Collections;

class ForceObject : zzEditableObject
{
    public InPoint controlPoint;

    [SerializeField]
    float _minForce = 0f;

    [zzSerialize]
    [FieldUI("最大力量")]
    public float minForce
    {
        get { return _minForce; }
        set 
        {
            _minForce = value;
            updateForce();
        }
    }

    [SerializeField]
    float _maxForce = 10f;

    [zzSerialize]
    [FieldUI("最小力量")]
    public float maxForce
    {
        get { return _maxForce; }
        set 
        { 
            _maxForce = value;
            updateForce();
        }
    }

    public ConstantForce myConstantForce;

    void updateForce()
    {
        setForceRate(controlPoint.powerValue);
    }

    void Start()
    {
        controlPoint.addProcessFuncFloatArg(setForceRate);
        myConstantForce = constantForce;
        setForceRate(0f);
    }

    //0-1
    public void setForceRate(float pRate)
    {
        myConstantForce.relativeForce
            = new Vector3(Mathf.Lerp(_minForce,_maxForce,pRate), 0f, 0f);
    }

    public override InPoint[] inPoints
    {
        get
        {
            return new InPoint[] { controlPoint };
        }
    }

}