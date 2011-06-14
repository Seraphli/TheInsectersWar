using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RaceValueShow:MonoBehaviour
{
    public Renderer valueRenderer;

    public float beginOffset;
    public float endOffset;

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
            valueRenderer.material.mainTextureOffset 
                = new Vector2(Mathf.Lerp(beginOffset,endOffset,_rate), 0f);
        }
    }

    void Awake()
    {
        rate = _rate;
    }
        
    ////在编辑模式下显示
    //void OnDrawGizmosSelected()
    //{
    //    if( !Application.isPlaying )
    //    {
    //        rate = _rate;
    //    }
    //}

}