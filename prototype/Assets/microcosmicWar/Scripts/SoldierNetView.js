
var soldier:Soldier;
var life:Life;
//var transform;

function Awake()
{
	//soldier=gameObject.GetComponent(soldier);
	if(!soldier)
		soldier=gameObject.GetComponentInChildren(Soldier);
	if(!life)
		life=gameObject.GetComponentInChildren(Life);
	/*
	if( !zzCreatorUtility.isHost() )
	{
		Destroy(soldier.GetComponentInChildren(SoldierAI));
	}*/
	if( !zzCreatorUtility.isMine(gameObject.networkView ) )
	{
		Destroy(soldier.GetComponentInChildren(SoldierAI));
	}
	//if(!soldier)
	//	Debug.LogError(gameObject.name);
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	var pos=Vector3();
	var rot=Quaternion();
	var lVelocity=Vector3();
	var lActionCommand= UnitActionCommand();
	var lBloodValue:float;
	
	//---------------------------------------------------
	if (stream.isWriting)
	{
		pos=transform.position;
		rot=transform.rotation;
		lVelocity = soldier.getVelocity();
		lActionCommand= soldier.getCommand();
		lBloodValue=life.getBloodValue();
		//var cc;
	}
	
	//---------------------------------------------------
	stream.Serialize(pos);
	stream.Serialize(rot);
	stream.Serialize(lVelocity);
	stream.Serialize(lBloodValue);
	stream.Serialize(lActionCommand.FaceLeft);
	stream.Serialize(lActionCommand.FaceRight);
	stream.Serialize(lActionCommand.GoForward);
	stream.Serialize(lActionCommand.Fire);
	stream.Serialize(lActionCommand.Jump);
	
	//---------------------------------------------------
	if(stream.isReading)
	{
		transform.position=pos;
		transform.rotation=rot;
	//if(!soldier)
	//{
	//	Debug.LogError(gameObject.name);
		//print(isStar);
	//	}
		soldier.setVelocity(lVelocity);
		soldier.setCommand(lActionCommand);
		life.setBloodValue(lBloodValue);
		
	}
	/*
		print(pos);
		print(rot);
		print(lActionCommand.FaceLeft);
		print(lActionCommand.FaceRight);
		print(lActionCommand.GoForward);
		print(lActionCommand.Fire);
		print(lActionCommand.Jump);
		*/
}

//function Update () {
//}