using UnityEngine;
using System.Collections;

public class zzRigidbodyObject : zzEditableObject
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
        rigidbody.freezeRotation = false;
    }

    public override void OnDragOn()
    {
        rigidbody.isKinematic = false;
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

    bool _freezeXRotation;
    bool _freezeYRotation;
    bool _freezeZRotation;

    RigidbodyConstraints rigidbodyConstraints
    {
        get
        {
            RigidbodyConstraints lOut = RigidbodyConstraints.None;
            if (_freezeXRotation)
                lOut |= RigidbodyConstraints.FreezeRotationX;
            if (_freezeYRotation)
                lOut |= RigidbodyConstraints.FreezeRotationY;
            if (_freezeZRotation)
                lOut |= RigidbodyConstraints.FreezeRotationZ;
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

    [LabelUI(verticalDepth = 0, horizontalDepth = 0)]
    public const string freezeRotationLabel = "冻结旋转";

    [zzSerialize]
    [FieldUI("X", verticalDepth = 0, horizontalDepth = 1)]
    public bool freezeXRotation
    {
        get { return _freezeXRotation; }
        set
        {
            _freezeXRotation = value;
            updateFreeze();
        }
    }

    [zzSerialize]
    [FieldUI("Y", verticalDepth = 0, horizontalDepth = 2)]
    public bool freezeYRotation
    {
        get { return _freezeYRotation; }
        set
        {
            _freezeYRotation = value;
            updateFreeze();
        }
    }

    [zzSerialize]
    [FieldUI("Z", verticalDepth = 0, horizontalDepth = 3)]
    public bool freezeZRotation
    {
        get { return _freezeZRotation; }
        set
        {
            _freezeZRotation = value;
            updateFreeze();
        }
    }

}