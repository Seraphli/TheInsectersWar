using UnityEngine;
using System.Collections;

public class zz2DRigidbodyObject : zzEditableObject
{
    public override void applyPlayState()
    {
        rigidbody.isKinematic = _fixed;
        rigidbody.useGravity = _useGravity;
        rigidbody.constraints = rigidbodyConstraints;
    }

    public override void applyPauseState()
    {
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.constraints = noneRigidbodyConstraints;
    }

    public override void OnDragOn()
    {
        rigidbody.isKinematic = false;
    }

    //[SerializeField]
    //bool _freezePositionX = false;

    //[SerializeField]
    //bool _freezePositionY = false;


    [LabelUI(verticalDepth = -1, horizontalDepth = 0)]
    public const string freezePositionLabel = "冻结位移";

    bool _freezePositionX = false;
    bool _freezePositionY = false;

    [zzSerialize]
    [FieldUI("X", verticalDepth = -1, horizontalDepth = 1)]
    public bool freezeXPosition
    {
        get { return _freezePositionX; }
        set
        {
            _freezePositionX = value;
            updateFreeze();
        }
    }

    [zzSerialize]
    [FieldUI("Y", verticalDepth = -1, horizontalDepth = 2)]
    public bool freezeYPosition
    {
        get { return _freezePositionY; }
        set
        {
            _freezePositionY = value;
            updateFreeze();
        }
    }

    bool _freezeRotation = false;

    [zzSerialize]
    [FieldUI("冻结旋转", verticalDepth = 0)]
    public bool freezeRotation
    {
        get { return _freezeRotation; }
        set
        {
            _freezeRotation = value;
            updateFreeze();
        }
    }

    [SerializeField]
    bool _fixed = false;

    [zzSerialize]
    [FieldUI("固定物体")]
    public bool fixedObject
    {
        get { return _fixed; }
        set
        {
            _fixed = value;
            if (inPlaying)
            {
                rigidbody.isKinematic = true;
                rigidbody.WakeUp();
            }
        }
    }

    const RigidbodyConstraints noneRigidbodyConstraints =
                RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezePositionZ;
    bool __freezeRotation = false;

    RigidbodyConstraints rigidbodyConstraints
    {
        get
        {
            RigidbodyConstraints lOut = noneRigidbodyConstraints;

            if (_freezeRotation)
                lOut |= RigidbodyConstraints.FreezeRotationZ;
            if (_freezePositionX)
                lOut |= RigidbodyConstraints.FreezePositionX;
            if (_freezePositionY)
                lOut |= RigidbodyConstraints.FreezePositionY;
            return lOut;
        }
    }

    void updateFreeze()
    {
        if (inPlaying)
        {
            rigidbody.constraints = rigidbodyConstraints;
        }
    }

    [SerializeField]
    bool _useGravity;

    [zzSerialize]
    [FieldUI("使用重力")]
    public bool useGravity
    {
        get { return _useGravity; }
        set
        {
            _useGravity = value;
            if (inPlaying)
                rigidbody.useGravity = _useGravity;
        }
    }


    [zzSerialize]
    [FieldUI("质量(kg)")]
    public float mass
    {
        get { return rigidbody.mass; }
        set
        {
            rigidbody.mass = value;
        }
    }


}