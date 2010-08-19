

//执行频率/每秒执行的次数

var fequencyTimer=zzFrequencyTimer();

var enemyList=Hashtable();

var fireTarget:Transform;

var aiMachineGun:DefenseTower;

var adversaryName:String;
var adversaryLayer=-1;

//敌人还在这个角度时,枪不动
var fireDeviation=4.0;

function OnTriggerEnter (other : Collider)
{
	//print("OnTriggerEnter"+other.gameObject.layer);
	if(other.gameObject.layer==adversaryLayer)
	{
		if(!fireTarget)
			fireTarget=other.transform;
		enemyList[other.transform]=true;
	}
}


function OnTriggerExit (other : Collider) 
{
	//print("OnTriggerExit"+other.gameObject.layer);
	if(other.gameObject.layer==adversaryLayer)
	{
		enemyList.Remove(other.transform);
		
		//移出的是否是目标兵
		if(fireTarget==other.transform)
			searchFireTargetInList();
	}
}

protected function searchFireTargetInList()
{
	//避免一帧中多次OnTriggerExit和其他的情况,所以用此方法
	
	//在此情况下重新搜索
	//if(!fireTarget || !enemyList.ContainsKey(fireTarget))
	//{
		fireTarget=null;
		for(var i:System.Collections.DictionaryEntry in enemyList)
		{
			if(i.Key)
			{
				fireTarget=i.Key;
				break;
			}
			enemyList.Remove(i);
		}
	//}
	return fireTarget;
}

//function SetAdversaryLayerValue(pLayerValue:int)
//{
//	adversaryLayerValue = pLayerValue;
//}

function Start()
{
	if(!aiMachineGun)
		aiMachineGun=transform.parent.GetComponentInChildren(DefenseTower);
	if(adversaryLayer==-1)
		adversaryLayer= LayerMask.NameToLayer(adversaryName);
	fequencyTimer.setImpFunction(ImpUpdate);
}

function ImpUpdate () 
{
	//searchFireTarget();
	if(fireTarget)
	//if(searchFireTargetInList())
	{
		aiMachineGun.setFire(true);
		aiMachineGun.takeAim(fireTarget.position,fireDeviation);
	}
	else
	{
		aiMachineGun.setFire(false);
	}
}

function Update()
{
	fequencyTimer.Update();
}

//function searchFireTarget()
//{
//}