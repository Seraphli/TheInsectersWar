﻿using UnityEngine;
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
    [FieldUI("角度",verticalDepth = -3)]
    [SliderUI(0f,359.99f,verticalDepth = -2)]
    public float objectRotation
    {
        get
        {
            return transform.localRotation.eulerAngles.z;
        }
        set
        {
            var lLocalRotation = new Quaternion();
            lLocalRotation.eulerAngles
                = new Vector3(0f, 0f, Mathf.Clamp(value, 0f, 359.99f));
            transform.localRotation = lLocalRotation;
        }
    }


    [LabelUI(verticalDepth = -1, horizontalDepth = 0)]
    public const string freezePositionLabel = "冻结位移";

    [SerializeField]
    bool _freezePositionX = false;
    [SerializeField]
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

    [SerializeField]
    bool _freezeRotation = false;

    [zzSerialize]
    [FieldUI("冻结旋转", verticalDepth = 0, tooltip = "禁止物体在运行时旋转")]
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
            rigidbody.mass = Mathf.Max(0.5f,value);
        }
    }


}