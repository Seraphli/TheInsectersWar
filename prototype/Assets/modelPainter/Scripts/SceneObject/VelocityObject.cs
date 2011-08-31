using UnityEngine;
using System.Collections;

class VelocityObject : zzEditableObject
{
    public InPoint controlPoint;

    [SerializeField]
    float _maxSpeed = 10f;

    [SerializeField]
    float _minSpeed = 0f;

    public Rigidbody myRigidbody;
    public Transform myTransform;


    void Start()
    {
        myRigidbody = rigidbody;
        myTransform = transform;
        controlPoint.addProcessFuncFloatArg(setSpeedRate);
        setSpeedRate(0f);
    }

    public float speed;

    //0-1
    public void setSpeedRate(float pRate)
    {
        speed = Mathf.Lerp(_minSpeed,_maxSpeed,pRate);
    }

    static public bool needSpeed(float lSelfSpeed,float lNeedSpeed)
    {
        if (lSelfSpeed >0)
        {
            if(lNeedSpeed>0 && lSelfSpeed>lNeedSpeed )
                return false;
        }
        else if(lSelfSpeed<0)
        {
            if (lNeedSpeed < 0 && lSelfSpeed < lNeedSpeed)
                return false;
        }
        return true;
    }

    void FixedUpdate()
    {
        if(speed==0f)
            return;
        var lLocalVelocity = myTransform.InverseTransformDirection(myRigidbody.velocity);
        if(needSpeed(lLocalVelocity.x,speed))
        {
            lLocalVelocity.x = speed;
            myRigidbody.velocity = myTransform.TransformDirection(lLocalVelocity);
        }

            //= new Vector3(_minForce + (_maxForce - _minForce) * pRate, 0f, 0f);

    }

    public override InPoint[] inPoints
    {
        get
        {
            return new InPoint[] { controlPoint };
        }
    }
}