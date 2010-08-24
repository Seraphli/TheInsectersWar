
var bloodValue:int = 5.0;
var fullBloodValue:int = 5.0;

var bloodValueChangeCallback=zzUtilities.nullFunction;
//var dieCallback=zzUtilities.nullFunction;
var dieCallbackList=Array();

protected var injureInfo:Hashtable;
//搜索有无bloodBar 以便显示血量
function Start()
{
	var lBloodBar = GetComponentInChildren(BloodBar);
	if(lBloodBar)
		lBloodBar.setLife(this);
}
/*
function setDieCallback( call )
{
	dieCallback=call;
}*/

function addDieCallback( call )
{
	dieCallbackList.Add(call);
}

function setBloodValueChangeCallback(call)
{
	bloodValueChangeCallback=call;
}

function injure(value:float,pInjureInfo:Hashtable)
{
	injureInfo=pInjureInfo;
	if( bloodValue>0)
	{
		setBloodValue(bloodValue-value);
	}
	injureInfo=null;
}

function injure(value:float)
{
	injure(value,null);
}

//在回调中调用
function getInjureInfo():Hashtable
{
	return injureInfo;
}

function setBloodValue(pValue:float)
{
	if(pValue>fullBloodValue)
		pValue = fullBloodValue;
	if( bloodValue!=pValue )
	{
		bloodValue = pValue;
		bloodValueChangeCallback();
		if(bloodValue<=0)
		{
			//dieCallback();
			for(var dieCallback in dieCallbackList)
				dieCallback();
			//zzCreatorUtility.Destroy (gameObject);
		}
	}
}

function setFullBloodValue(lValue:float)
{
	fullBloodValue=lValue;
}

function getBloodValue()
{
	return bloodValue;
}


function getFullBloodValue()
{
	return fullBloodValue;
}

function isAlive()
{
	return bloodValue>0;
}

function isDead()
{
	return bloodValue<=0;
}


function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	var lBloodValue:float;
	
	//---------------------------------------------------
	if (stream.isWriting)
	{
		lBloodValue=getBloodValue();
	}
	
	stream.Serialize(lBloodValue);
	
	if(stream.isReading)
	{
		setBloodValue(lBloodValue);
	}
}

//function Update ()
//{
//}0