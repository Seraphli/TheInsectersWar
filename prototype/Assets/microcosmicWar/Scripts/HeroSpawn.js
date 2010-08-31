
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;
var autoCreatePlayer:boolean=false;
var itemBagID:int;

var hero:GameObject;

//�Ƿ񴴽���
protected var haveFirstCreate=false;

function Start()
{
/*
	if( zzCreatorUtility.isHost() )
	{
		//setOwer(Network.player);
		zzCreatorUtility.sendMessage(gameObject,"setOwerImp",Network.player);
		createHero();
	}
*/
	if(autoCreatePlayer)
		createHeroFirstTime();
}

function setOwer(pOwner:NetworkPlayer)
{
	//owner = pOwner;
	zzCreatorUtility.sendMessage(gameObject,"setOwerImp",pOwner);
}

@RPC
function setOwerImp(pOwner:NetworkPlayer)
{
	owner = pOwner;
}

function createHeroFirstTime()
{
	if(haveFirstCreate)
		Debug.LogError("haveFirstCreate == true");
	hero = _createHero();
	var itemBagControl:zzItemBagControl = hero.GetComponent(zzItemBagControl);
	itemBagControl.addCallAfterStart(_toGetItemBagID);
	haveFirstCreate = true;
}

function _toGetItemBagID()
{
	var itemBagControl:zzItemBagControl = hero.GetComponent(zzItemBagControl);
	itemBagID = itemBagControl.getBagID();
}

//function _theHeroDead()


//����ֻ���ڷ������˵���
function _reviveHero()
{
	if(!haveFirstCreate)
		Debug.LogError("haveFirstCreate == false");
	//if(Network.peerType !=NetworkPeerType.Disconnected)
	//	var lHeroObject:GameObject = Network.Instantiate(heroPrefab,transform.position,Quaternion(),0);
	hero = _createHero();
	
	var itemBagControl:zzItemBagControl = hero.GetComponent(zzItemBagControl);
	itemBagControl.setUseExistBag(itemBagID);
	
	//haveFirstCreate = true;
}

//����ֻ���ڷ������˵���
protected function _createHero()
{
	var lHeroObject:GameObject = zzCreatorUtility.Instantiate(heroPrefab,transform.position,Quaternion(),0);

	//print(lHeroObject.GetInstanceID());
	//print(owner);
	//zzCreatorUtility.sendMessage(gameObject,"createNetControl",lHeroObject.networkView.viewID);
	if(Network.peerType ==NetworkPeerType.Disconnected)
		createControl(lHeroObject);
	else
	{
		if(Network.player == owner )//��������
			createNetControl(lHeroObject);
		else
			networkView.RPC("RPCcreateNetControl",owner,lHeroObject.networkView.viewID);
	}
	
	var lRemoveCall:IobjectListener = lHeroObject.GetComponent(IobjectListener);
	
	lRemoveCall.setRemovedCallFunc(_reviveHero);
	
	return lHeroObject;
}

@RPC
protected function RPCcreateNetControl(pHeroID:NetworkViewID)
{
	var lHeroObject:GameObject = NetworkView.Find(pHeroID).gameObject;
	createNetControl(lHeroObject);
}

protected function createNetControl(pHeroObject:GameObject)
{
	//����ͬ�����
	var lnetSysn:GameObject = zzCreatorUtility.Instantiate(netSysnPrefab,Vector3(),Quaternion(),0);
	var lHeroNetView:HeroNetView = lnetSysn.GetComponent(HeroNetView);
	lHeroNetView.setOwner(pHeroObject);
	
	createControl(pHeroObject);
}

protected function createControl( pHeroObject:GameObject)
{	
	//������
	var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = lSystem.GetComponent(mainInput);
	lMainInput.setToControl(pHeroObject.GetComponent(ActionCommandControl));
	//print(pHeroObject.name);
	//print(pHeroObject.GetInstanceID());
	
	//��UI
	pHeroObject.AddComponent(BagItemUI);
	pHeroObject.AddComponent(MoneyUI);
	pHeroObject.AddComponent(bagItemUIInput);
	
	//�������
	var lCameraFollow:_2DCameraFollow=gameObject.Find("Main Camera").GetComponent(_2DCameraFollow);
	lCameraFollow.setTaget(pHeroObject.transform);

}

