
//class IobjectListener
//{
	protected var initedCallFunc=zzUtilities.nullFunction;
	protected var removedCallFunc=zzUtilities.nullFunction;
	
	function setInitedCallFunc( func)
	{
		initedCallFunc = func;
	}
	
	function setRemovedCallFunc( func)
	{
		removedCallFunc = func;
	}
	
	virtual function initedCall()
	{
		initedCallFunc();
	}

	virtual function removedCall()
	{
		removedCallFunc();
	}
//}

//function Start()
//{
//	initCall();
//}

//function Update () {
//}