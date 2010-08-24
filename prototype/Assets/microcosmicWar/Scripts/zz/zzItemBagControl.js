
//��������Ѿ���������������еİ�,δ���þ���bagName�����°�
var bagIndex=-1;
var bagName="";
var owner:GameObject;

protected var itemSystem:zzItemSystem;

function getItemSystem()
{
	return itemSystem;
}

class zzMyItemInBagInfo
{
	var name:String;
	var number:int;
}

var itemInBagInfo:zzMyItemInBagInfo[];

function setItemChangedCall(pCall)
{
	getBagData().addDataChangedCall(pCall,1,getBagData().getItemTypeNum()-1);
}

function setMoneyChangedCall(pCall)
{
	getBagData().addDataChangedCall(pCall,0,0);
}

function getBagData():ItemBagData
{
	return  itemSystem.getBagTable().getData(bagIndex);
}

function setItemNum(pName:String,pNum:int)
{
	getBagData().setNum(itemSystem.getItemTypeTable().getIndex(pName),pNum);
}

//0 ΪǮ
function addItemOne(pItemIndex:int)
{
	getBagData().addItemOne(pItemIndex);
}

//0 ΪǮ
function addItem(index:int,number:int)
{
	getBagData().addItem(index,number);
}

//�õ�������Ϊ0����Ĺ�������,�Ա�UI��ʾ
function getItemList()
{
	var lOut = Array();
	for(var i=1;i<itemSystem.getItemTypeTable().getNum();++i)
	{
		var numOfTheItem = getNum(i);
		while(numOfTheItem>0)
		{
			lOut.Add(i);
			--numOfTheItem;
		}
	}
	return lOut;
}

function addMoney(number:int)
{
	//Debug.Log("addMoney");
	addItem(0,number);
	//Debug.Log(getMoneyNum());
}

function getNum(index:int)
{
	return getBagData().getNum(index);
}

function getMoneyNum()
{
	return getNum(0);
}

function useItemOne(pItemIndex:int,pOwner:GameObject)
{
	//print("index:"+pItemIndex+",number:"+getBagData().getNum(pItemIndex));
	if(getNum(pItemIndex)>0)
	{
		var item:ItemObjectImp = itemSystem.getItemTypeTable().getData(pItemIndex).getItemObject();
		if(item.canUse(pOwner))
		{
			item.use();
			getBagData().removeItemOne(pItemIndex);
		}
	}
}

function useItemOne(pItemIndex:int)
{
	useItemOne(pItemIndex,owner);
}

function Start()
{
	if(!owner)
		owner= gameObject;
	itemSystem = zzItemSystem.getSingleton();
	if(bagIndex!=-1)
	{
		bagName = getBagData().name;
	}
	else
		bagIndex =itemSystem.addBag(bagName);
		
	for(var i:zzMyItemInBagInfo in itemInBagInfo)
	{
		setItemNum(i.name,i.number);
	}
	
	//�ͷ��������� �Ա�õ�����
	var lEmitter:Emitter = owner.GetComponentInChildren(Emitter);
	if(lEmitter)
		lEmitter.setInjureInfo({"bagControl":this});
	
	//Debug.Log(getBagData());
}

