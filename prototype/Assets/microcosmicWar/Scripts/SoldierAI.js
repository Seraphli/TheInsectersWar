
var adversaryName="";

//执行/思考 的频率
var frequencyOfImplement=3.0;

protected var timeToWait:float;//=1.0/frequencyOfImplement

protected var timePos=0.0;

var soldier:Soldier;
var finalAim:Transform;
protected var finalAimPos:Vector3;

function Start()
{
	timeToWait=1.0/frequencyOfImplement;
	timePos=timeToWait+0.1;
	finalAimPos=finalAim.position;
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

function getCommand()
{
}

function calculate()
{
	
}

function Update ()
{
	timePos+=Time.deltaTime;
	var lActionCommand=UnitActionCommand();
	if(timePos>timeToWait)
	{
		lActionCommand=moveToFinalAim();
		timePos=0.0;
	}
	else
		lActionCommand.GoForward=true;
	soldier.setCommand(lActionCommand);
}