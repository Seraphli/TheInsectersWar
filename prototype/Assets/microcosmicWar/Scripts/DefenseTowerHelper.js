
var race:Race = Race.eNone;

function Awake()
{
	if( race ==Race.eNone )
		return;
		
	var lTowerObject:GameObject = gameObject;
	var lTowerAI:AiMachineGunAI = lTowerObject.GetComponentInChildren(AiMachineGunAI);
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
	//print(lTowerObject.layer);
	lTowerAI.setAdversaryLayer(PlayerInfo.getAdversaryRaceLayer(race));
}