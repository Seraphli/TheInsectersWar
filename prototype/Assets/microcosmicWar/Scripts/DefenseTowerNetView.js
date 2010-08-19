
var defenseTower:DefenseTower;
var life:Life;

function Awake()
{
	if(!defenseTower)
		defenseTower=gameObject.GetComponentInChildren(DefenseTower);
	if(!life)
		life=gameObject.GetComponentInChildren(Life);
	
	//print(!zzCreatorUtility.isHost());
	if( !zzCreatorUtility.isHost() )
	{
		Destroy(defenseTower.GetComponentInChildren(AiMachineGunAI));
	}
}


function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	life.OnSerializeNetworkView(stream,info);
	
	var lAngle:float;
	var lFire:boolean;
	var lAimAngular:float;
	//var lFace:int;
	
	//---------------------------------------------------
	if (stream.isWriting)
	{
		lAngle=defenseTower.getAngle();
		lFire=defenseTower.inFiring();
		lAimAngular=defenseTower._getSmoothAngle();
	//	lFace=defenseTower.getFaceDirection();
		
	}
	
	//---------------------------------------------------
	stream.Serialize(lAngle);
	stream.Serialize(lFire);
	stream.Serialize(lAimAngular);
	//stream.Serialize(lFace);
	
	//---------------------------------------------------
	if(stream.isReading)
	{
		defenseTower.setAngle(lAngle);
		defenseTower.setFire(lFire);
		defenseTower._setSmoothAngle(lAimAngular);
	//	defenseTower.setFaceDirection(lFace);
		
	}
}