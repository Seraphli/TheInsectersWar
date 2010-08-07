/*
class AnimationImpInTime extends AnimationListener
{
	
	var  ImpFunction=zzUtilities.nullFunction;
	
	var ImpTime=0.0;
	
	//此次循环中是否执行过
	protected var IsImpInThisLoop=false;
	
	virtual  function updateCallback(time:float)
	{
		//Debug.Log("AnimationImpInTime");
		if(!IsImpInThisLoop && time>ImpTime)
		{
			ImpFunction();
			IsImpInThisLoop=true;
		}
	}
	
	virtual  function overEndBeforeUpdateCallback(){IsImpInThisLoop=false;}
	
	virtual  function endTheAnimationCallback(){IsImpInThisLoop=false;}
}
*/
//-----------------------------------------------------------------------------------------------

class AnimationImpTimeListInfo
{
	function AnimationImpTimeListInfo(pTime:float,func)
	{
		ImpFunction=func;
		ImpTime=pTime;
	}
	
	function AnimationImpTimeListInfo()
	{
		ImpFunction=zzUtilities.nullFunction;
	
		ImpTime=0.0;
	}
	
	var  ImpFunction=zzUtilities.nullFunction;
	
	var ImpTime=0.0;
}

class AnimationImpInTimeList extends AnimationListener
{
	//时间要从小到大排列
	var animationImpTimeListInfo:AnimationImpTimeListInfo[];
	
	protected var infoArray=Array();
	
	protected var playNum=0;
	
	virtual function beginTheAnimationCallback()
	{
		playNum=0;
	}
	
	virtual  function updateCallback(time:float)
	{
		//Debug.Log("AnimationImpInTimeList");
		while(playNum<animationImpTimeListInfo.length && time>animationImpTimeListInfo[playNum].ImpTime)
		{
			animationImpTimeListInfo[playNum].ImpFunction();
			++playNum;
		}
	}
	
	virtual function overBeginCall()
	{
		playNum=0;
	}
	
	function addImp(pTime:float,func)
	{
		var imp=AnimationImpTimeListInfo();
		imp.ImpTime=pTime;
		imp.ImpFunction=func;
		infoArray.Add(imp);
	}
	
	function endAddImp()
	{
		var lTemp:AnimationImpTimeListInfo[] = infoArray.ToBuiltin( AnimationImpTimeListInfo );
		setImpInfoList(lTemp);
		infoArray=Array();
	}
	
	
	//virtual  function overEndCallback(){IsImpInThisLoop=false;}
	
	//考虑到执行在非循环动画中,超出动画范围处,所有将下面删除
	//virtual  function endTheAnimationCallback()
	//{
	//	playNum=animationImpTimeListInfo.length;
	//}
	
	//virtual  function overEndAfterUpdateCallback()
	//{
	//	playNum=0;
	//}
	
	function getImpInfoList()
	{
		return animationImpTimeListInfo;
	}
	
	function setImpInfoList(pInfo:AnimationImpTimeListInfo[])
	{
		animationImpTimeListInfo=pInfo;
	}
}
