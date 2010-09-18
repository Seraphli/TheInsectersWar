
//actionType: 平举 向上

//action: 开火 待机

//actionType=action[]

class UnityAnimationInfo
{
	var animationName:String;
	var functionName:String;
	var functionImpTime:float;
}

//一个动作的类型
class ActionTypeInfo
{
	//比如枪平举
	var actionTypeName:String;
	
	var animationActionList:UnityAnimationInfo[];
}

class AnimationSettingForAction
{
	var name:String;
	var useFader:boolean;
}

//身体的某一部分的动作
class BodyActionInfo
{
	//var bodyName:String;
	var layer=0;
	
	var mixingTransform:Transform;
	
	//var mixingTransform2:Transform;
	
	//animationSettingList 与ActionTypeInfo.animationActionInfo 数量应相同.
	var animationSettingList:AnimationSettingForAction[];
	
	var actionTypeList:ActionTypeInfo[];
}

class AnimationSettingData
{
	var useFader:boolean;
	var animationIndex:int;
}

//class BodyActionData
//{
//}
//class AnimationActionData
//{
//}
class BodyAction
{

var nowActionIndex=0;
var nowActionType="";

//[string]=int
var actionNameToAniSetting=Hashtable();
//var actionTypeNameToIndex=Hashtable();
//[string]=string[]
var nameToActionType=Hashtable();
//var nameToActionListMap=Hashtable();
var myAnimation:Animation;

function init(cInfo:BodyActionInfo,pAnimation:Animation)
{
	myAnimation=pAnimation;
	var actionTypeList=cInfo.actionTypeList;
		
	//存储动作名对应的索引
	for(var lNameIndex=0;lNameIndex<cInfo.animationSettingList.length;++lNameIndex)
	{
		var animationSettingInfo = cInfo.animationSettingList[lNameIndex];
		var animationSettingData=AnimationSettingData();
		
		animationSettingData.useFader=animationSettingInfo.useFader;
		animationSettingData.animationIndex=lNameIndex;
		
		actionNameToAniSetting[animationSettingInfo.name]=animationSettingData;
	}
	
	//遍历动作类型
	for(var i=0; i<actionTypeList.length;++i)
	{
		var actionTypeInfo=actionTypeList[i];
		//actionNameToAniSetting[actionTypeInfo.actionTypeName]=i;
		
		//遍历一个动作类型中的动作/动画
		var animationNameList=Array();
		for(var animationInfo in actionTypeInfo.animationActionList)
		{
			animationNameList.Add(animationInfo.animationName);
			if(cInfo.mixingTransform)
			{
				myAnimation[animationInfo.animationName].AddMixingTransform(cInfo.mixingTransform);
				myAnimation[animationInfo.animationName].layer = cInfo.layer;
			}
			//if(cInfo.mixingTransform2)
			//{
			//	myAnimation[animationInfo.animationName].AddMixingTransform(cInfo.mixingTransform2);
			//	myAnimation[animationInfo.animationName].layer = cInfo.layer;
			//}
			if(animationInfo.functionName.Length!=0)
			{
				var lAnimationEvent=AnimationEvent();
				lAnimationEvent.functionName="messageRedirectReceiver";
				lAnimationEvent.stringParameter=animationInfo.functionName;
				lAnimationEvent.time=animationInfo.functionImpTime;
				myAnimation[animationInfo.animationName].clip.AddEvent(lAnimationEvent);
				//Debug.Log(animationInfo.animationName+"  "+animationInfo.functionName);
			}
		}
		//将动作/动画表存储在 对应类型名称下
		nameToActionType[actionTypeInfo.actionTypeName]=animationNameList.ToBuiltin(String);
	}
	nowActionType=actionTypeList[0].actionTypeName;
}
/*
function playAction(pActionIndex:int)
{
	if(pActionIndex!=nowActionIndex)
	{
		nowActionIndex=pActionIndex;
		updateAnimation();
	}
}
*/
function playAction(pActionName:String)
{
	//Debug.Log(pActionName);
	/*
	Debug.Log(actionNameToAniSetting);
	
	for(var i:System.Collections.DictionaryEntry in actionNameToAniSetting)
	{
		Debug.Log(i.Key+" "+i.Value);
	}
		*/
	//Debug.Log(pActionName);
	var lAnimationSettingData:AnimationSettingData = actionNameToAniSetting[pActionName];
	//Debug.Log(lAnimationSettingData);
	//playAction(lAnimationSettingData.animationIndex);
	if(lAnimationSettingData.animationIndex!=nowActionIndex)
	{
		nowActionIndex=lAnimationSettingData.animationIndex;
		updateAnimation(lAnimationSettingData.useFader);
	}
}

function playActionType(pName:String)
{
	//Debug.Log(pName);
	if(pName!=nowActionType)
	{
		nowActionType=pName;
		updateAnimation(true);
	}
}

function updateAnimation(pCrossFade:boolean)
{
	//Debug.Log(nameToActionType[nowActionType][nowActionIndex]);
	if(pCrossFade)
		myAnimation.CrossFade(nameToActionType[nowActionType][nowActionIndex],0.1);
	else
		myAnimation.Play(nameToActionType[nowActionType][nowActionIndex]);
}

}