var recoverValue:int=50.0;
var duration = 10.0;
var life:Life;

//var timePos = 0.0;
var recoverValueResidue=0.0;

function Update () 
{
	var lDeltaTime =  Time.deltaTime;
	if(lDeltaTime>duration)
		lDeltaTime = duration;
	recoverValueResidue += recoverValue * lDeltaTime/ duration;
	var lNowRecoverValue:int  = recoverValueResidue;
	recoverValueResidue -= lNowRecoverValue;
	duration-=lDeltaTime;
	//if(lNowRecoverValue>recoverValue)
	//	lNowRecoverValue = recoverValue;
	recoverValue -= lNowRecoverValue;
	life.setBloodValue(life.getBloodValue()+lNowRecoverValue);
	
	if(duration<=0)
		zzCreatorUtility.Destroy(this);
}