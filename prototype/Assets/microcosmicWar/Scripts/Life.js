
var bloodValue = 5.0;
var fullBloodValue = 5.0;

var bloodValueChangeCallback=zzUtilities.nullFunction;
var dieCallback=zzUtilities.nullFunction;
//搜索有无bloodBar 以便显示血量
function Start()
{
	var lBloodBar = GetComponentInChildren(BloodBar);
	if(lBloodBar)
		lBloodBar.setLife(this);
}

function setDieCallback( call )
{
	dieCallback=call;
}

function setBloodValueChangeCallback(call)
{
	bloodValueChangeCallback=call;
}

function injure(value:float)
{
	if( bloodValue>0)
	{
		setBloodValue(bloodValue-value);
		
	}
}
function setBloodValue(pValue:float)
{
	if( bloodValue!=pValue )
	{
		bloodValue = pValue;
		bloodValueChangeCallback();
		if(bloodValue<=0)
		{
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



//function Update ()
//{
//}0