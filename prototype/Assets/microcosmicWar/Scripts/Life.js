
var bloodValue = 5.0;
var fullBloodValue = 5.0;

var bloodValueChangeCallback=zzUtilities.nullFunction;
var dieCallback=zzUtilities.nullFunction;

function Start()
{
	var lBloodBar = GetComponentInChildren(BloodBar);
	if(lBloodBar)
		lBloodBar.setLife(this);
}

function setBloodValueChangeCallback(call)
{
	bloodValueChangeCallback=call;
}

function injure(value:float)
{
	bloodValue-=value;
	
	if(bloodValue<=0)
	{
		dieCallback();
		zzCreatorUtility.Destroy (gameObject);
	}
		
	bloodValueChangeCallback();
}

function setBloodValue(lValue:float)
{
	bloodValue=lValue;
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


//function Update ()
//{
//}0