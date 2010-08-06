
class AnimationImpInTime extends AnimationListener
{
	
	var  ImpFunction=zzUtilities.nullFunction;
	
	var ImpTime=0.0;
	
	//�˴�ѭ�����Ƿ�ִ�й�
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

//-----------------------------------------------------------------------------------------------

class AnimationImpTimeListInfo
{
	var  ImpFunction=zzUtilities.nullFunction;
	
	var ImpTime=0.0;
}

class AnimationImpInTimeList extends AnimationListener
{
	//ʱ��Ҫ��С��������
	var animationImpTimeListInfo:AnimationImpTimeListInfo[];
	
	var playNum=0;
	
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
	
	//virtual  function overEndCallback(){IsImpInThisLoop=false;}
	
	virtual  function endTheAnimationCallback()
	{
		playNum=animationImpTimeListInfo.length;
	}
	
	virtual  function overEndAfterUpdateCallback()
	{
		playNum=0;
	}
	
	function getImpInfoList()
	{
		return animationImpTimeListInfo;
	}
}
