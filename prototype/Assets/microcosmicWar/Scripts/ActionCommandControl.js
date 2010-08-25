

class UnitActionCommand
{
	//None,
	
	var FaceLeft=false;//朝向左
	var FaceRight=false;//朝向右
	var FaceUp=false;
	var FaceDown=false;
	var GoForward=false;//前进
	var Fire=false;//开火
	var Jump=false;//跳跃
	
	function clear()
	{
		//MoveLeft=false;
		//MoveRight=false;
		FaceLeft=false;
		FaceRight=false;
		FaceUp=false;
		FaceDown=false;
		
		GoForward=false;
		
		Fire=false;
		Jump=false;
	}
}

var unitActionCommand:UnitActionCommand;

function setCommand( pUnitActionCommand:UnitActionCommand)
{
	unitActionCommand = pUnitActionCommand;
}

function getCommand()
{
	return unitActionCommand;
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	stream.Serialize(unitActionCommand.FaceLeft);
	stream.Serialize(unitActionCommand.FaceRight);
	stream.Serialize(unitActionCommand.FaceUp);
	stream.Serialize(unitActionCommand.FaceDown);
	
	stream.Serialize(unitActionCommand.GoForward);
	
	stream.Serialize(unitActionCommand.Fire);
	stream.Serialize(unitActionCommand.Jump);
	
}