
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 用于配置unity的动画
/// </summary>
public class zzAnimationConfig : MonoBehaviour
{
    [System.Serializable]
    public class unityAniEventInfo
    {
        public string stringParameter;
        public string functionName = "messageRedirectReceiver";
        public float time;
    }

    [System.Serializable]
    public class unityAniStateConfigInfo
    {
        public string animationName;

        //是否使用配置
        public bool useTheConfig = false;
        public float speed = 1.0f;
        public int layer = 0;

        public unityAniEventInfo[] events = new unityAniEventInfo[]{};

    }

    public Animation myAnimation;
    public unityAniStateConfigInfo[]    animationConfig;

    void Start()
    {
        foreach (unityAniStateConfigInfo lStateConfigInfo in animationConfig)
        {
            //是否使用配置
            if(lStateConfigInfo.useTheConfig)
            {
                AnimationState lAnimationState = myAnimation[lStateConfigInfo.animationName];
                lAnimationState.speed = lStateConfigInfo.speed;
                lAnimationState.layer = lStateConfigInfo.layer;

                //创建事件
                foreach (unityAniEventInfo lEventInfo in lStateConfigInfo.events)
                {
                    AnimationEvent lAnimationEvent = new AnimationEvent();
                    lAnimationEvent.functionName = lEventInfo.functionName;
                    lAnimationEvent.stringParameter = lEventInfo.stringParameter;
                    lAnimationEvent.time = lEventInfo.time;
                    lAnimationState.clip.AddEvent(lAnimationEvent);
                }
            }
        }
    }

    [ContextMenu("refresh Animation List")]
    void refreshAniList()
    {
        foreach (AnimationState lState in myAnimation)
        {
            unityAniStateConfigInfo lConfigInfo
                = System.Array.Find(animationConfig, x => x.animationName == lState.name);
            if(lConfigInfo==null)
            {
                lConfigInfo = new unityAniStateConfigInfo();
                lConfigInfo.animationName = lState.name;
                var lConfigInfoList = new List<unityAniStateConfigInfo>(animationConfig);
                lConfigInfoList.Add( lConfigInfo );
                animationConfig = lConfigInfoList.ToArray();
            }
        }
    }
}