using UnityEngine;
using System.Collections;

public class zzGUIAniToTargetScale:MonoBehaviour
{
    public zzGUITransform GUITransform;

    public float targetScale;
    public float speed;

    float mNowSpeed;
    float mTargetScale;

    void Start()
    {
        scaleToTarget(targetScale);
    }

    public void scaleToTarget(float pTargetScale)
    {
        targetScale = pTargetScale;
        mTargetScale = pTargetScale;
        float lNowScale = GUITransform.scale.x;
        if(pTargetScale>lNowScale)
        {
            mNowSpeed = Mathf.Abs(speed);
        }
        else if (pTargetScale < lNowScale)
        {
            mNowSpeed = - Mathf.Abs(speed);
        }
        else//pTargetScale == lNowScale
        {
            this.enabled = false;
        }
        this.enabled = true;
    }

    void Update()
    {
        Vector2 lNowScale = GUITransform.scale;
        float lNewScaleValue = lNowScale.x;
        lNewScaleValue += mNowSpeed*Time.deltaTime;
        Vector2 lNewScale = new Vector2(lNewScaleValue, lNewScaleValue);
        if(mNowSpeed>0)//放大
        {
            if (lNewScaleValue > mTargetScale)
            {
                lNewScale = new Vector2(mTargetScale, mTargetScale);
                this.enabled = false;
            }
        }
        else//缩小
        {
            if (lNewScaleValue < mTargetScale)
            {
                lNewScale = new Vector2(mTargetScale, mTargetScale);
                this.enabled = false;
            }
        }
        GUITransform.scale = lNewScale;
    }
}