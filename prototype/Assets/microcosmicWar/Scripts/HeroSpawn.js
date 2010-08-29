
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;
var autoCreatePlayer:boolean=false;
var itemBagID:int;

var hero:GameObject;

//是否创建过
var haveFirstCreate=false;

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

	zzCreatorUtility.sendMessage(gameObject,"createNetControl",lHeroObject.networkView.viewID);
	
	return lHeroObject;
}

@RPC
function createNetControl(pHeroID:NetworkViewID)
{
	if(Network.player != owner)
		return;
		
	var lHeroObject:GameObject = NetworkView.Find(pHeroID).gameObject;
	
	//创建同步组件
	var lnetSysn:GameObject = zzCreatorUtility.Instantiate(netSysnPrefab,Vector3(),Quaternion(),0);
	var lHeroNetView:HeroNetView = lnetSysn.GetComponent(HeroNetView);
	lHeroNetView.setOwner(lHeroObject);
	
	//绑定输入
	var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = lSystem.GetComponent(mainInput);
	lMainInput.setToControl(lHeroObject.GetComponent(ActionCommandControl));
	
	//绑定UI
	lHeroObject.AddComponent(BagItemUI);
	lHeroObject.AddComponent(MoneyUI);
	lHeroObject.AddComponent(bagItemUIInput);
}

