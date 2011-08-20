
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//actionType: 平举 向上

//action: 开火 待机

//actionType=action[]
[System.Serializable]
public class UnityAnimationInfo
{
    public string animationName;
    public string functionName;
    public float functionImpTime;
}

//一个动作的类型
[System.Serializable]
public class ActionTypeInfo
{
    //比如枪平举
    public string actionTypeName;

    public UnityAnimationInfo[] animationActionList;
}

[System.Serializable]
public class AnimationSettingForAction
{
    public string name;
    public bool useFader;
}

//身体的某一部分的动作
[System.Serializable]
public class BodyActionInfo
{
    //string bodyName;
    public int layer = 0;

    public Transform mixingTransform;

    //Transform mixingTransform2;

    //animationSettingList 与ActionTypeInfo.animationActionInfo 数量应相同.
    public AnimationSettingForAction[] animationSettingList;

    public ActionTypeInfo[] actionTypeList;
}

[System.Serializable]
public class AnimationSettingData
{
    public bool useFader;
    public int animationIndex;
}


[System.Serializable]
public class BodyAction:MonoBehaviour
{
    public BodyActionInfo actionInfo;

    public int nowActionIndex = 0;
    public string nowActionType = "";

    public Dictionary<string, AnimationSettingData> actionNameToAniSetting =
        new Dictionary<string, AnimationSettingData>();

    public Dictionary<string, string[]> nameToActionType
        = new Dictionary<string, string[]>();

    public Animation myAnimation;

    /// <summary>
    /// 标记已经添加过事件的动画Clip
    /// </summary>
    static Dictionary<AnimationClip, bool> haveAddedEvent = new Dictionary<AnimationClip, bool>();

    void Awake()
    {
        init(actionInfo);
    }

    public void init(BodyActionInfo cInfo)
    {
        ActionTypeInfo[] actionTypeList = cInfo.actionTypeList;

        //存储动作名对应的索引
        for (int lNameIndex = 0; lNameIndex < cInfo.animationSettingList.Length; ++lNameIndex)
        {
            AnimationSettingForAction animationSettingInfo = cInfo.animationSettingList[lNameIndex];
            AnimationSettingData animationSettingData = new AnimationSettingData();

            animationSettingData.useFader = animationSettingInfo.useFader;
            animationSettingData.animationIndex = lNameIndex;

            actionNameToAniSetting[animationSettingInfo.name] = animationSettingData;
        }

        //遍历动作类型
        for (int i = 0; i < actionTypeList.Length; ++i)
        {
            ActionTypeInfo actionTypeInfo = actionTypeList[i];
            //actionNameToAniSetting[actionTypeInfo.actionTypeName]=i;

            //遍历一个动作类型中的动作/动画
            List<string> animationNameList = new List<string>(actionTypeInfo.animationActionList.Length);
            foreach (UnityAnimationInfo animationInfo in actionTypeInfo.animationActionList)
            {
                animationNameList.Add(animationInfo.animationName);
                if (cInfo.mixingTransform)
                {
                    myAnimation[animationInfo.animationName].AddMixingTransform(cInfo.mixingTransform);
                    myAnimation[animationInfo.animationName].layer = cInfo.layer;
                }
   
                //AnimationClip   lAnimationClip = myAnimation[animationInfo.animationName].clip;
                //if (!haveAddedEvent.ContainsKey(lAnimationClip))
                //{
                //    //设置动画事件
                //    if (animationInfo.functionName.Length != 0)
                //    {
                //        AnimationEvent lAnimationEvent = new AnimationEvent();
                //        lAnimationEvent.functionName = "messageRedirectReceiver";
                //        lAnimationEvent.stringParameter = animationInfo.functionName;
                //        lAnimationEvent.time = animationInfo.functionImpTime;
                //        lAnimationClip.AddEvent(lAnimationEvent);
                //        //Debug.Log(animationInfo.animationName+"  "+animationInfo.functionName);
                //    }
                //    haveAddedEvent[lAnimationClip] = true;
                //}
            }
            //将动作/动画表存储在 对应类型名称下
            //nameToActionType[actionTypeInfo.actionTypeName]=animationNameList.ToBuiltin(string);
            nameToActionType[actionTypeInfo.actionTypeName] = animationNameList.ToArray();
        }
        //nowActionType = actionTypeList[0].actionTypeName;
        var lNowAniamtion = nameToActionType[nowActionType][nowActionIndex];
        myAnimation.Play(lNowAniamtion);

        animationState = myAnimation[lNowAniamtion];
    }

    public void playAction(string pActionName)
    {
        //Debug.Log(pActionName);
        /*
        Debug.Log(actionNameToAniSetting);
	
        foreach(System.Collections.DictionaryEntry i in actionNameToAniSetting)
        {
            Debug.Log(i.Key+" "+i.Value);
        }
            */
        //Debug.Log(pActionName);
        AnimationSettingData lAnimationSettingData = actionNameToAniSetting[pActionName];
        //Debug.Log(lAnimationSettingData);
        //playAction(lAnimationSettingData.animationIndex);
        if (lAnimationSettingData.animationIndex != nowActionIndex)
        {
            nowActionIndex = lAnimationSettingData.animationIndex;
            updateAnimation(lAnimationSettingData.useFader);
        }
    }

    public void playActionType(string pName)
    {
        //Debug.Log(pName);
        if (pName != nowActionType)
        {
            var lastAniState = animationState;
            nowActionType = pName;
            updateAnimation(true);
            animationState.time = lastAniState.time;
        }
    }

    public AnimationState animationState;

    public void updateAnimation(bool pCrossFade)
    {
        //Debug.Log(nameToActionType[nowActionType][nowActionIndex]);
        string[] lnowActionTypeMap = nameToActionType[nowActionType];
        var lNowAniName = lnowActionTypeMap[nowActionIndex];
        animationState = myAnimation[lNowAniName];
        if (pCrossFade)
        {
            myAnimation.CrossFade(lNowAniName, 0.1f);
        }
        else
            myAnimation.Play(lnowActionTypeMap[nowActionIndex]);
    }

}