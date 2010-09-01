
var adversaryName:String;

//执行频率/每秒执行的次数
//var frequencyOfImplement=3.0;
var fequencyTimer=zzFrequencyTimer();

//protected var timeToWait:float;//=1.0/frequencyOfImplement

//protected var timePos=0.0;

var actionCommandControl:ActionCommandControl;
var finalAim:Transform;
//var enemyLayer:int;
var enable=true;

protected var finalAimPos:Vector3;
protected var actionCommand=UnitActionCommand();

protected var adversaryLayerValue=-1;

var fireTarget:Transform;

//射击的检测距离将在介于与以下值
//protected
var fireDistanceMin:float;
//protected
var fireDistanceMax:float;

//为了使每个兵的行为不同,射击的范围也取随机值
var fireDistanceMinRandMin=4.0;
var fireDistanceMinRandMax=8.0;

var fireDistanceMaxRandMin=8.0;
var fireDistanceMaxRandMax=12.0;

function Start()
{
	//客户端的AI 在 SoldierNetView 中去除
	//if(!zzCreatorUtility.isHost())
	//	Debug.LogError("AI not removed in not host");
	//{
	//	Destroy(this);
	//	timeToWait=100.0;
	//	return;
	//}
	//timeToWait=1.0/frequencyOfImplement;
	//timePos=timeToWait+0.1;
	if(finalAim)
		finalAimPos=finalAim.position;
	if(adversaryLayerValue==-1)
		adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
	//print(LayerMask.NameToLayer(adversaryName));
	//print(adversaryLayerValue);
	if(!actionCommandControl)
		actionCommandControl=gameObject.GetComponentInChildren(ActionCommandControl);
	fequencyTimer.setImpFunction(AiUpdate);
	
	fireDistanceMin=Random.Range(fireDistanceMinRandMin,fireDistanceMinRandMax);
	fireDistanceMax=Random.Range(fireDistanceMaxRandMin,fireDistanceMaxRandMax);
	
	//产生第一个命令
	if(enable)
		AiUpdate();
}

function SetFinalAim(pFinalAim:Transform)
{
	finalAim= pFinalAim;
	finalAimPos=Vector3();
	if(pFinalAim)
		finalAimPos=finalAim.position;
}

function SetSoldier(pSoldier:Soldier)
{
	//print("SetSoldier");
	//print(pSoldier);
	soldier=pSoldier;
}

function SetAdversaryLayerValue(pLayerValue:int)
{
	adversaryLayerValue = pLayerValue;
}

function moveToFinalAim()
{
	var lActionCommand=UnitActionCommand();
	if(finalAim)
	{
		var lT = finalAimPos.x - transform.position.x ;
		//var lActionCommand=UnitActionCommand();
		lActionCommand.GoForward=true;
		/*
		if(lT<0)
		{
			lActionCommand.FaceLeft=true;
		}
		else
		{
			lActionCommand.FaceRight=true;
		}*/
		//soldier.setCommand(lActionCommand);
		setFaceCommand(lActionCommand,lT);
		return lActionCommand;
	}
	return lActionCommand;
}

function setFaceCommand(pActionCommand:UnitActionCommand,face:int)
{
		if(face<0)
		{
			pActionCommand.FaceLeft=true;
		}
		else if(face>0)
		{
			pActionCommand.FaceRight=true;
		}
}

function needFire()
{
	//var lFwd = transform.TransformDirection(Vector3.forward);
	var lHit : RaycastHit;
	//print(transform.position);
	//print(soldier);
	//print(adversaryLayerValue);
	//var lFaceDirection = 0;
	var lFaceValue = actionCommandControl.getFaceValue();
	if (Physics.Raycast (transform.position, Vector3(lFaceValue,0,0) , lHit, Random.Range (fireDistanceMin,fireDistanceMax),adversaryLayerValue)) 
	//if (Physics.Raycast (transform.position, lFwd , lHit, 4.0,adversaryLayerValue)) 
	{
		fireTarget=lHit.transform;
		return lFaceValue;
	}
	if (Physics.Raycast (transform.position, Vector3(-lFaceValue,0,0) , lHit, fireDistanceMin,adversaryLayerValue)) 
	{
		return -lFaceValue;
	}
	return 0;
}

function getCommand()
{
	return actionCommand;
}

function calculate()
{
	var lFireTaget:int = needFire();
	if(lFireTaget!=0)
	{
		actionCommand.clear();
		actionCommand.Fire=true;
		setFaceCommand(actionCommand,lFireTaget);
	}
	else
		actionCommand=moveToFinalAim();
}

function Update ()
{
	if(enable)
	{
	//print("AI Update");
	//if(zzCreatorUtility.isHost())
	//{
		/*timePos+=Time.deltaTime;
		//var lActionCommand=UnitActionCommand();
		if(timePos>timeToWait)
		{
			//lActionCommand=moveToFinalAim();
			calculate();
			timePos=0.0;
		}
		//soldier.setCommand(lActionCommand);
		soldier.setCommand(getCommand());*/
	//}
		fequencyTimer.Update();
	}
}

function AiUpdate()
{
	calculate();
	actionCommandControl.setCommand(getCommand());
}