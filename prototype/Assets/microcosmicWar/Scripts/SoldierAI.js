
var adversaryName:String;

//ִ��Ƶ��/ÿ��ִ�еĴ���
//var frequencyOfImplement=3.0;
var fequencyTimer=zzFrequencyTimer();

//protected var timeToWait:float;//=1.0/frequencyOfImplement

//protected var timePos=0.0;

var soldier:Soldier;
var finalAim:Transform;
//var enemyLayer:int;
var enable=true;

protected var finalAimPos:Vector3;
protected var actionCommand=UnitActionCommand();

protected var adversaryLayerValue=-1;

function Start()
{
	//�ͻ��˵�AI �� SoldierNetView ��ȥ��
	//if(!zzCreatorUtility.isHost())
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
	if(!soldier)
		soldier=gameObject.GetComponentInChildren(Soldier);
	fequencyTimer.setImpFunction(AiUpdate);
	
	//������һ������
	if(enable)
		AiUpdate();
}

function SetFinalAim(pFinalAim:Transform)
{
	finalAim= pFinalAim;
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
	var lT = finalAimPos.x - transform.position.x ;
	var lActionCommand=UnitActionCommand();
	lActionCommand.GoForward=true;
	if(lT<0)
	{
		lActionCommand.FaceLeft=true;
	}
	else
	{
		lActionCommand.FaceRight=true;
	}
	//soldier.setCommand(lActionCommand);
	return lActionCommand;
}

function needFire()
{
	//var lFwd = transform.TransformDirection(Vector3.forward);
	var lHit : RaycastHit;
	//print(transform.position);
	//print(soldier);
	//print(adversaryLayerValue);
	if (Physics.Raycast (transform.position, Vector3(soldier.getFaceDirection(),0,0) , lHit, Random.Range (4.0,8.0),adversaryLayerValue)) 
	//if (Physics.Raycast (transform.position, lFwd , lHit, 4.0,adversaryLayerValue)) 
	{
		return true;
	}
	return false;
}

function getCommand()
{
	return actionCommand;
}

function calculate()
{
	if(needFire())
	{
		actionCommand.clear();
		actionCommand.Fire=true;
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
	soldier.setCommand(getCommand());
}