
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;

function Start()
{
	if( zzCreatorUtility.isHost() )
	{
		//setOwer(Network.player);
		zzCreatorUtility.sendMessage(gameObject,"setOwerImp",Network.player);
		createHero();
	}
}
/*
function setOwer(pOwner:NetworkPlayer)
{
	//owner = pOwner;
	networkView.RPC("setOwerImp",RPCMode.All, pOwner);
}
*/
@RPC
function setOwerImp(pOwner:NetworkPlayer)
{
	owner = pOwner;
}

//����ֻ���ڷ������˵���
function createHero()
{
	//if(Network.peerType !=NetworkPeerType.Disconnected)
	//	var lHeroObject:GameObject = Network.Instantiate(heroPrefab,transform.position,Quaternion(),0);
	var lHeroObject:GameObject = zzCreatorUtility.Instantiate(heroPrefab,transform.position,Quaternion(),0);
	
	//networkView.RPC("RPCSetOwner",RPCMode.All, lHeroObject.networkView.viewID);
	zzCreatorUtility.sendMessage(gameObject,"createNetControl",lHeroObject.networkView.viewID);
}

@RPC
function createNetControl(pHeroID:NetworkViewID)
{
	if(Network.player != owner)
		return;
		
	var lHeroObject:GameObject = NetworkView.Find(pHeroID).gameObject;
	
	//����ͬ�����
	var lnetSysn:GameObject = zzCreatorUtility.Instantiate(netSysnPrefab,Vector3(),Quaternion(),0);
	var lHeroNetView:HeroNetView = lnetSysn.GetComponent(HeroNetView);
	lHeroNetView.setOwner(lHeroObject);
	
	//������
	var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = lSystem.GetComponent(mainInput);
	lMainInput.setToControl(lHeroObject.GetComponent(ActionCommandControl));
	
	//��UI
	lHeroObject.AddComponent(BagItemUI);
	lHeroObject.AddComponent(MoneyUI);
}

