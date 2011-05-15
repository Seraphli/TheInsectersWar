﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RaceValueShow:MonoBehaviour
{
    public zzPlaneMesh planeMesh = new zzPlaneMesh();
    public Material image;

    [SerializeField]
    float _rate;
    public float rate
    {
        get
        {
            return _rate;
        }

        set
        {
            _rate = value;
            planeMesh.UVs[3].y = value;
            planeMesh.UVs[2].y = value;


            planeMesh.vertices[3].y = value;
            planeMesh.vertices[2].y = value;

            planeMesh.UpdateMesh();
        }
    }

    void Awake()
    {
        planeMesh.Init(gameObject);
        GetComponent<MeshRenderer>().material = image;
        rate = _rate;
    }
        
    //在编辑模式下显示
    void OnDrawGizmosSelected()
    {
        if( !Application.isPlaying )
        {
            if (!planeMesh.mesh
                || !GetComponent<MeshFilter>()
                || !GetComponent<MeshFilter>().sharedMesh)
                Awake();
            rate = _rate;
        }
    }

}