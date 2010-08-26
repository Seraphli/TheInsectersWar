
class zzCharacter
{
	var characterController:CharacterController;
	
	var runSpeed=5.0;
	var gravity = 15.0;
	var jumpSpeed = 10.0;
	
	protected var moveV=Vector3();
	
	function update(pUnitActionCommand:UnitActionCommand,pFaceValue:int,isAlive:boolean)
	{
		moveV.x=0;
		moveV.z=0;
		
		if(pUnitActionCommand.GoForward)
			moveV.x=pFaceValue;
		
		if(isAlive && characterController.isGrounded)
		{
			if( !pUnitActionCommand.FaceDown)
			{
				if(pUnitActionCommand.Jump)
					moveV.y = jumpSpeed;
				else
					moveV.y = -0.1;	//以免飞起来
			}
		}
		else
			moveV.y -= gravity* Time.deltaTime;
			
		// Move the controller
		var lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);

		characterController.Move(lVelocity* Time.deltaTime);
	}
	
	function isGrounded()
	{
		return  characterController.isGrounded;
	}
	
	function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
	{
		var pos=Vector3();
		var rot=Quaternion();
		var lTransform = characterController.transform;
		//---------------------------------------------------
		if (stream.isWriting)
		{
			pos=lTransform.position;
			rot=lTransform.rotation;
		}
		//---------------------------------------------------
		stream.Serialize(pos);
		stream.Serialize(rot);
		stream.Serialize(moveV);
		//---------------------------------------------------
		if(stream.isReading)
		{
			lTransform.position=pos;
			lTransform.rotation=rot;
			
		}
	}
	
};

enum UnitFaceDirection
{
	left,
	right,
}

class UnitFace
{
	static function getValue( type:UnitFaceDirection)
	{
		if(type == UnitFaceDirection.left)
			return -1;
		return 1;
	}
	
	static function getFace(pValue:int):UnitFaceDirection
	{
		if(pValue>0)
			return UnitFaceDirection.right;
		return UnitFaceDirection.left;
	}
}

var face:UnitFaceDirection = UnitFaceDirection.left;

//返回在x上的值
function getFaceValue():int
{
	return UnitFace.getValue(face);
}


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

//根据现在命令的方向更新朝向,若朝向有变则返回true
function updateFace():boolean
{
	if(unitActionCommand.FaceLeft != unitActionCommand.FaceRight)
	{
		if(unitActionCommand.FaceLeft && face==UnitFaceDirection.right)
		{
			face=UnitFaceDirection.left;
			return true;
		}
		if(unitActionCommand.FaceRight && face==UnitFaceDirection.left)
		{
			face=UnitFaceDirection.right;
			return true;
		}
	}
	return false;
}


function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	//var lFace:int = face;
	//stream.Serialize(lFace);
	//face =  lFace;
	
	stream.Serialize(unitActionCommand.FaceLeft);
	stream.Serialize(unitActionCommand.FaceRight);
	stream.Serialize(unitActionCommand.FaceUp);
	stream.Serialize(unitActionCommand.FaceDown);
	
	stream.Serialize(unitActionCommand.GoForward);
	
	stream.Serialize(unitActionCommand.Fire);
	stream.Serialize(unitActionCommand.Jump);
	
	
}
