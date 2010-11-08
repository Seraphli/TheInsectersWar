
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

//class BodyActionData
//{
//}
//class AnimationActionData
//{
//}
[System.Serializable]
public class BodyAction
{

    public int nowActionIndex = 0;
    public string nowActionType = "";

    //[string]=int
    public Hashtable actionNameToAniSetting = new Hashtable();
    //FIXME_VAR_TYPE actionTypeNameToIndex=Hashtable();
    //[string]=string[]
    public Hashtable nameToActionType = new Hashtable();
    //FIXME_VAR_TYPE nameToActionListMap=Hashtable();
    public Animation myAnimation;

    /// <summary>
    /// 标记已经添加过事件的动画Clip
    /// </summary>
    static Dictionary<AnimationClip, bool> haveAddedEvent = new Dictionary<AnimationClip, bool>();

    public void init(BodyActionInfo cInfo, Animation pAnimation)
    {
        myAnimation = pAnimation;
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
            ArrayList animationNameList = new ArrayList();
            foreach (UnityAnimationInfo animationInfo in actionTypeInfo.animationActionList)
            {
                animationNameList.Add(animationInfo.animationName);
                if (cInfo.mixingTransform)
                {
                    myAnimation[animationInfo.animationName].AddMixingTransform(cInfo.mixingTransform);
                    myAnimation[animationInfo.animationName].layer = cInfo.layer;
                }
                //if(cInfo.mixingTransform2)
                //{
                //	myAnimation[animationInfo.animationName].AddMixingTransform(cInfo.mixingTransform2);
                //	myAnimation[animationInfo.animationName].layer = cInfo.layer;
                //}

                
                AnimationClip   lAnimationClip = myAnimation[animationInfo.animationName].clip;
                if (!haveAddedEvent.ContainsKey(lAnimationClip))
                {
                    //设置动画事件
                    if (animationInfo.functionName.Length != 0)
                    {
                        AnimationEvent lAnimationEvent = new AnimationEvent();
                        lAnimationEvent.functionName = "messageRedirectReceiver";
                        lAnimationEvent.stringParameter = animationInfo.functionName;
                        lAnimationEvent.time = animationInfo.functionImpTime;
                        lAnimationClip.AddEvent(lAnimationEvent);
                        //Debug.Log(animationInfo.animationName+"  "+animationInfo.functionName);
                    }
                    haveAddedEvent[lAnimationClip] = true;
                }
            }
            //将动作/动画表存储在 对应类型名称下
            //nameToActionType[actionTypeInfo.actionTypeName]=animationNameList.ToBuiltin(string);
            nameToActionType[actionTypeInfo.actionTypeName] = animationNameList.ToArray(typeof(string));
        }
        nowActionType = actionTypeList[0].actionTypeName;
    }
    /*
    void  playAction ( int pActionIndex  ){
        if(pActionIndex!=nowActionIndex)
        {
            nowActionIndex=pActionIndex;
            updateAnimation();
        }
    }
    */
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
        AnimationSettingData lAnimationSettingData = (AnimationSettingData)actionNameToAniSetting[pActionName];
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
            nowActionType = pName;
            updateAnimation(true);
        }
    }

    public void updateAnimation(bool pCrossFade)
    {
        //Debug.Log(nameToActionType[nowActionType][nowActionIndex]);
        string[] lnowActionTypeMap = (string[])nameToActionType[nowActionType];
        if (pCrossFade)
        {
            myAnimation.CrossFade(lnowActionTypeMap[nowActionIndex], 0.1f);
        }
        else
            myAnimation.Play(lnowActionTypeMap[nowActionIndex]);
    }

}