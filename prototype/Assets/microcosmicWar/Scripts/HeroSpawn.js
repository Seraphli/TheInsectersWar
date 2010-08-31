
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;
var autoCreatePlayer:boolean=false;
var itemBagID:int;

var hero:GameObject;

//是否创建过
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


//创建只能在服务器端调用
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

//创建只能在服务器端调用
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
		if(Network.player == owner )//服务器端
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
	//创建同步组件
	var lnetSysn:GameObject = zzCreatorUtility.Instantiate(netSysnPrefab,Vector3(),Quaternion(),0);
	var lHeroNetView:HeroNetView = lnetSysn.GetComponent(HeroNetView);
	lHeroNetView.setOwner(pHeroObject);
	
	createControl(pHeroObject);
}

protected function createControl( pHeroObject:GameObject)
{	
	//绑定输入
	var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = lSystem.GetComponent(mainInput);
	lMainInput.setToControl(pHeroObject.GetComponent(ActionCommandControl));
	//print(pHeroObject.name);
	//print(pHeroObject.GetInstanceID());
	
	//绑定UI
	pHeroObject.AddComponent(BagItemUI);
	pHeroObject.AddComponent(MoneyUI);
	pHeroObject.AddComponent(bagItemUIInput);
	
	//绑定摄像机
	var lCameraFollow:_2DCameraFollow=gameObject.Find("Main Camera").GetComponent(_2DCameraFollow);
	lCameraFollow.setTaget(pHeroObject.transform);

}

