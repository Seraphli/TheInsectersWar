

//ִ��Ƶ��/ÿ��ִ�еĴ���

var fequencyTimer=zzFrequencyTimer();

var enemyList=Hashtable();

var fireTarget:Transform;

var aiMachineGun:DefenseTower;

var adversaryName:String;
var adversaryLayer=-1;

//���˻�������Ƕ�ʱ,ǹ����
var fireDeviation=4.0;

//var adversaryNumInFireRange = 0;

function setAdversaryLayer(pLayer:int)
{
	adversaryLayer = pLayer;
}

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
	//if(other.gameObject.layer==adversaryLayer)
	//{
	/*
		print("OnTriggerExit");
		if(fireTarget==null)
		{
			print(transform.parent.parent.gameObject.name);
			print(fireTarget==null);
			print(enemyList.Count);
		}
	*/
		
		enemyList.Remove(other.transform);
		
		//�Ƴ����Ƿ���Ŀ���
		if(fireTarget==other.transform)
			searchFireTargetInList();
	//}
}

protected function searchFireTargetInList()
{
	//����һ֡�ж��OnTriggerExit�����������,�����ô˷���
	
	//�ڴ��������������
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
		
	/*
		if(fireTarget==null)
		{
			print(transform.parent.parent.gameObject.name);
			print(fireTarget==null);
			print(enemyList.Count);
		}
		*/
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
		searchFireTargetInList();//��ʱ���屻�Ƴ�ʱ,û��OnTriggerExit
	}
	//adversaryNumInFireRange = enemyList.Count;
}

function Update()
{
	fequencyTimer.Update();
}

//function searchFireTarget()
//{
//}