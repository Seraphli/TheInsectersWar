
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;
var autoCreatePlayer:boolean=false;
var itemBagID:int;

var hero:GameObject;
var rebirthClockUI:zzInterfaceGUI;
var SystemObject:GameObject;

//是否创建过
protected var haveFirstCreate=false;

//重生所需的时间
var rebirthTime = 10;
protected var rebirthClockTimer:zzTimer;
//重生剩余的时间
protected var rebirthTimeLeave=0;
protected var rebirthTimer:zzTimer;

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
	if(!rebirthClockUI)
		rebirthClockUI = gameObject.Find("Main Camera/UI/rebirthClock").GetComponent(zzInterfaceGUI);
		
	if(!SystemObject)
		SystemObject= GameObject.Find("System");
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
function _rebirthHero()
{
	if(!haveFirstCreate)
		Debug.LogError("haveFirstCreate == false");
		
	if(HeroBelongToThePlayer)
	{
		//释放之前的输入
		_releaseControl(hero);
	}
	
	_createHeroRebirthClock();
	
	//重生的延迟执行
	rebirthTimer = gameObject.AddComponent(zzTimer);
	
	rebirthTimer.setImpFunction(_rebirthHeroCreate);
	rebirthTimer.setInterval(rebirthTime);
	//if(Network.peerType !=NetworkPeerType.Disconnected)
	//	var lHeroObject:GameObject = Network.Instantiate(heroPrefab,transform.position,Quaternion(),0);
	
	//haveFirstCreate = true;
}

//此英雄是否属于此玩家
function HeroBelongToThePlayer():boolean
{
	if(Network.peerType ==NetworkPeerType.Disconnected)
	{
		return true;
	}
	
	if(Network.player == owner)
		return true;
		
	return false;
}

//计时器的显示更新
protected function _createHeroRebirthClock()
{
/*
	if(Network.peerType ==NetworkPeerType.Disconnected)
		RPCCreateHeroRebirthClock();
	else
	{
		if(Network.player == owner )//服务器端
			RPCCreateHeroRebirthClock();
		else
			networkView.RPC("RPCCreateHeroRebirthClock",owner);
	}
*/
	if(HeroBelongToThePlayer())
		RPCCreateHeroRebirthClock();
	else
		networkView.RPC("RPCCreateHeroRebirthClock",owner);
}

@RPC
function RPCCreateHeroRebirthClock()
{
	rebirthClockTimer = gameObject.AddComponent(zzTimer);
	rebirthClockTimer.setImpFunction(_updateRebirthTimeLeave);
	rebirthClockTimer.setInterval(1.0);
	
	rebirthClockUI.setVisible(true);
	rebirthTimeLeave = rebirthTime;
	rebirthClockUI.setText(rebirthTimeLeave.ToString());
}

//被rebirthTimer调用
protected function _rebirthHeroCreate()
{
	//计时结束 rebirthTimer 的Update,并销毁
	rebirthTimer.enabled=false;
	Destroy(rebirthTimer);
	
	hero = _createHero();

	var itemBagControl:zzItemBagControl = hero.GetComponent(zzItemBagControl);
	itemBagControl.setUseExistBag(itemBagID);
}

//被rebirthClockTimer调用
protected function _updateRebirthTimeLeave()
{
	--rebirthTimeLeave;
	if(rebirthTimeLeave<=0)
	{
		//计时结束 关闭rebirthClockTimer 的Update,并销毁
		rebirthClockUI.setVisible(false);
		rebirthClockTimer.enabled=false;
		Destroy(rebirthClockTimer);
	}
	rebirthClockUI.setText(rebirthTimeLeave.ToString());
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
	
	lRemoveCall.setRemovedCallFunc(_rebirthHero);
	
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
	//var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = SystemObject.GetComponent(mainInput);
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

protected function _releaseControl( pHeroObject:GameObject)
{
	var lMainInput:mainInput = SystemObject.GetComponent(mainInput);
	lMainInput.setToControl(null);
	Destroy(pHeroObject.GetComponent(bagItemUIInput));
	
}
