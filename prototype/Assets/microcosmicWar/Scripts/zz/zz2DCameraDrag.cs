﻿using UnityEngine;

public class zz2DCameraDrag : MonoBehaviour
{
    public Camera dragedCamera;

    //public bool inDraging
    //{

    //}

    //应该让鼠标指针一直保持在这个位置
    [SerializeField]
    Vector3 dragBeginMouseWorldPos;

    public void beginDrag()
    {
        enabled = true;
        dragBeginMouseWorldPos
            = dragedCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update()
    {
        var lNowMouseWorldPos
            = dragedCamera.ScreenToWorldPoint(Input.mousePosition);
        var lCameraPos = dragedCamera.transform.position;
        var lMouseToCameraVector = lCameraPos - lNowMouseWorldPos;
        var lNewPos = dragBeginMouseWorldPos + lMouseToCameraVector;
        lNewPos.z = lCameraPos.z;
        dragedCamera.transform.position = lNewPos;
    }

    public void endDrag()
    {
        enabled = false;
    }
}