
var defenseTower:DefenseTower;

function Awake()
{
	if(!defenseTower)
		defenseTower=gameObject.GetComponentInChildren(DefenseTower);
		
	if( !zzCreatorUtility.isMine(gameObject.networkView ) )
	{
		Destroy(defenseTower.GetComponentInChildren(AiMachineGunAI));
	}
}

function Update () {
}