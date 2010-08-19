
function Update () {

	if(Input.GetButton ("test1"))
	{
		zzGameObjectCreator.getSingleton().create({
		"creatorName":"AiMachineGun",
		"position":Vector3(0,0,0),
		"rotation":Quaternion(),
		"face":1,
		"layer":9,
		"adversaryLayer":8
		});
	}
}