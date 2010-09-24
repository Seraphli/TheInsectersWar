
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

function injure(value:int,pInjureInfo:Hashtable)
{
	injureInfo=pInjureInfo;
	if( bloodValue>0)
	{
		setBloodValue(bloodValue-value);
	}
	injureInfo=null;
}

function injure(value:int)
{
	injure(value,null);
}

//在回调中调用
function getInjureInfo():Hashtable
{
	return injureInfo;
}

function setBloodValue(pValue:int)
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
			//for(var dieCallback in dieCallbackList)
			//	dieCallback();
			//zzCreatorUtility.Destroy (gameObject);
			zzCreatorUtility.sendMessage(gameObject,"Life_die");
		}
	}
}

@RPC
function Life_die()
{
	bloodValue = 0;
	for(var dieCallback in dieCallbackList)
		dieCallback(this);
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
	var lBloodValue:int=getBloodValue();
	
	//---------------------------------------------------
	//if (stream.isWriting)
	//{
	//	lBloodValue=getBloodValue();
	//}
	
	stream.Serialize(lBloodValue);
	
	if(stream.isReading)
	{
		setBloodValue(lBloodValue);
	}
}

static function getLifeFromTransform(pOwn:Transform):Life
{	
	var lLife:Life=pOwn.gameObject.GetComponent(Life);
	
	if(!lLife)
	{
		while(pOwn.parent)
		{
			pOwn=pOwn.parent;
			lLife = pOwn.gameObject.GetComponent(Life);
			if(lLife)
				break;
		}
	}
	return lLife;
}

//function Update ()
//{
//}0