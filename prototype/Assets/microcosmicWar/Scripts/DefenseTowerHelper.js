
var race:Race = Race.eNone;

function Awake()
{
	if( race ==Race.eNone )
		return;
		
	var lTowerObject:GameObject = gameObject;
	//var lTowerAI:AiMachineGunAI = lTowerObject.GetComponentInChildren(AiMachineGunAI);
	var lTowerAI:AiMachineGunAI = lTowerObject.transform.Find("turn/enemyDetector").GetComponent(AiMachineGunAI);
	//var AIOb: Transform = transform.Find("turn/enemyDetector");
	//print("AIOb:"+(AIOb==null));
	//print(AIOb.GetComponent(AiMachineGunAI)==null);
	//print(AIOb.GetComponent(AiMachineGunAI).enabled);
	//print(AIOb.active);
	/*
	switch( race )
	{
		case Race.ePismire:
		{
			break;
		}
		case Race.eBee:
		{
			break;
		}
	}
	*/
	lTowerObject.layer = PlayerInfo.getRaceLayer(race);
	//print(gameObject.name);
	//print(lTowerAI==null);
	//print(lTowerObject.layer);
	lTowerAI.setAdversaryLayer(PlayerInfo.getAdversaryRaceLayer(race));
}