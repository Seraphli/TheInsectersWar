
var heroPrefab:GameObject;
var netSysnPrefab:GameObject;

var owner:NetworkPlayer;
var autoCreatePlayer:boolean=false;
var itemBagID:int;

var hero:GameObject;
var rebirthClockUI:zzInterfaceGUI;
var SystemObject:GameObject;

//�Ƿ񴴽���
protected var haveFirstCreate=false;

//���������ʱ��
var rebirthTime = 10;
protected var rebirthClockTimer:zzTimer;
//����ʣ���ʱ��
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


//����ֻ���ڷ������˵���
function _rebirthHero()
{
	if(!haveFirstCreate)
		Debug.LogError("haveFirstCreate == false");
		
	if(HeroBelongToThePlayer)
	{
		//�ͷ�֮ǰ������
		_releaseControl(hero);
	}
	
	_createHeroRebirthClock();
	
	//�������ӳ�ִ��
	rebirthTimer = gameObject.AddComponent(zzTimer);
	
	rebirthTimer.setImpFunction(_rebirthHeroCreate);
	rebirthTimer.setInterval(rebirthTime);
	//if(Network.peerType !=NetworkPeerType.Disconnected)
	//	var lHeroObject:GameObject = Network.Instantiate(heroPrefab,transform.position,Quaternion(),0);
	
	//haveFirstCreate = true;
}

//��Ӣ���Ƿ����ڴ����
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

//��ʱ������ʾ����
protected function _createHeroRebirthClock()
{
/*
	if(Network.peerType ==NetworkPeerType.Disconnected)
		RPCCreateHeroRebirthClock();
	else
	{
		if(Network.player == owner )//��������
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

//��rebirthTimer����
protected function _rebirthHeroCreate()
{
	//��ʱ���� rebirthTimer ��Update,������
	rebirthTimer.enabled=false;
	Destroy(rebirthTimer);
	
	hero = _createHero();

	var itemBagControl:zzItemBagControl = hero.GetComponent(zzItemBagControl);
	itemBagControl.setUseExistBag(itemBagID);
}

//��rebirthClockTimer����
protected function _updateRebirthTimeLeave()
{
	--rebirthTimeLeave;
	if(rebirthTimeLeave<=0)
	{
		//��ʱ���� �ر�rebirthClockTimer ��Update,������
		rebirthClockUI.setVisible(false);
		rebirthClockTimer.enabled=false;
		Destroy(rebirthClockTimer);
	}
	rebirthClockUI.setText(rebirthTimeLeave.ToString());
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
	//����ͬ�����
	var lnetSysn:GameObject = zzCreatorUtility.Instantiate(netSysnPrefab,Vector3(),Quaternion(),0);
	var lHeroNetView:HeroNetView = lnetSysn.GetComponent(HeroNetView);
	lHeroNetView.setOwner(pHeroObject);
	
	createControl(pHeroObject);
}

protected function createControl( pHeroObject:GameObject)
{	
	//������
	//var lSystem:GameObject = GameObject.Find("System");
	var lMainInput:mainInput = SystemObject.GetComponent(mainInput);
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

protected function _releaseControl( pHeroObject:GameObject)
{
	var lMainInput:mainInput = SystemObject.GetComponent(mainInput);
	lMainInput.setToControl(null);
	Destroy(pHeroObject.GetComponent(bagItemUIInput));
	
}
