
//如果索引已经设置则代表是已有的吧,未设置就用bagName创建新包
var bagIndex=-1;
var bagName="";
var owner;

protected var itemSystem:zzItemSystem;

class zzMyItemInBagInfo
{
	var name:String;
	var number:int;
}

var itemInBagInfo:zzMyItemInBagInfo[];

function getBagData()
{
	return  itemSystem.getBagTable().getData(bagIndex);
}

function setItemNum(pName:String,pNum:int)
{
	getBagData().setNum(itemSystem.getItemTypeTable().getIndex(pName),pNum);
}

function addItemOne(pItemIndex:int)
{
	getBagData().addItem(pItemIndex);
}

function useItemOne(pItemIndex:int,pOwner:GameObject)
{
	print("index:"+pItemIndex+",number:"+getBagData().getNum(pItemIndex));
	if(getBagData().getNum(pItemIndex)>0)
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
	
	//Debug.Log(getBagData());
}

function Update () {
}