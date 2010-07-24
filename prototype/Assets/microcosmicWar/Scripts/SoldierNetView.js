
var soldier:Soldier;
//var transform;

function Start()
{
	//soldier=gameObject.GetComponent(soldier);
	if(!soldier)
		soldier=gameObject.GetComponentInChildren(Soldier);
	if(!soldier)
		Debug.LogError(gameObject.name);
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	var pos=Vector3();
	var rot=Quaternion();
	var lActionCommand= UnitActionCommand();
	
	//---------------------------------------------------
	if (stream.isWriting)
	{
		pos=transform.position;
		rot=transform.rotation;
		lActionCommand= soldier.getCommand();
		//var cc;
	}
	
	//---------------------------------------------------
	stream.Serialize(pos);
	stream.Serialize(rot);
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
	if(!soldier)
		Debug.LogError(gameObject.name);
		soldier.setCommand(lActionCommand);
		
		
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