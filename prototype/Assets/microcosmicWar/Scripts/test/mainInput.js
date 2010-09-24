
var actionCommandControl:ActionCommandControl;

function setToControl(pActionCommandControl:ActionCommandControl)
{
	actionCommandControl = pActionCommandControl;
}

function GetActionCommandFromInput():UnitActionCommand
{
	var lActionCommand=UnitActionCommand();
	//lActionCommand.MoveLeft=Input.GetButton ("left");
	if(Input.GetButton ("left"))
	{
		lActionCommand.FaceLeft=true;
		lActionCommand.GoForward=true;
	}
	//lActionCommand.MoveRight=Input.GetButton ("right");
	if(Input.GetButton ("right"))
	{
		lActionCommand.FaceRight=true;
		lActionCommand.GoForward=true;
	}
	if(Input.GetButton ("down"))
		lActionCommand.FaceDown=true;
	if(Input.GetButton ("up"))
		lActionCommand.FaceUp=true;
		
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButtonDown("jump");
	return lActionCommand;
}

function Update ()
{
	//if( zzCreatorUtility.isHost())
	if(actionCommandControl)
		actionCommandControl.setCommand(GetActionCommandFromInput());
}