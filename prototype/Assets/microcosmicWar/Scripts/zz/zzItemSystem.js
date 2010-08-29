
//var money:int;

//var  itemTypeList=Array();
//var nameToTypeIndex=Hashtable();

class ItemTypeInfo
{
	var name:String;
	
	var image:Texture;
	
	//var nameForShow:String;
	
	function getImage()
	{
		return image;
	}
	/*
	protected var groupIndex=0;
	
	function setGroupIndex(pGroupIndex:int)
	{
		groupIndex = pGroupIndex;
	}
	
	function getGroupIndex()
	{
		return groupIndex;
	}*/
	
	var useItem:GameObject;
	//var useItemComponentName:String;
	protected var useItemObject:GameObject;
	
	function ItemTypeInfo()
	{	/*
		Debug.Log(name+(useItem!=null));
		if(useItem)
		{
			useItemObject = UnityEngine.Object.Instantiate(useItem,Vector3(),Quaternion());
			useItemObject.name = "name/"+name;
		}*/
	}
	
	function getItemObject():ItemObjectImp
	{
		//Debug.Log(useItemComponentName);
		//Debug.Log(useItemObject!=null);
		//return useItemObject.GetComponent(useItemComponentName);
		return useItem.GetComponent(ItemObjectImp);
	}
	
	function ToString () : String 
	{
		return "Item Name:"+name;
	}
};

class ItemTypeGroup
{
	var name:String;
	var itemTypeInfos:ItemTypeInfo[];
};

var itemTypeGroups:ItemTypeGroup[];

protected var itemTypeTable =zzIndexTable();

protected var indexGroupDatas=zzIndexTable();

class ItemGroupData
{
	function ItemGroupData(pName:String,pIndexBegin:int,pIndexEnd:int)
	{
		name = pName;
		indexBegin = pIndexBegin;
		indexEnd = pIndexEnd;
	}
	
	var name:String;
	protected var indexBegin=0;
	protected var indexEnd=0;
	
	function ToString () : String 
	{
		return "Item Group( Name:"+name+", "+indexBegin+", "+indexEnd+")";
	}
	
	function getIndexInBegin()
	{
		return indexBegin;
	}
	
	function getIndexInEnd()
	{
		return indexEnd;
	}
	
	function getItemNum()
	{
		return indexEnd-indexBegin;
	}
};

function getItemTypeTable()
{
	return itemTypeTable;
}

function getGroupTable()
{
	return indexGroupDatas;
}

function getGroupNum()
{
	return indexGroupDatas.getNum();
}



//return index
protected function addType( type:ItemTypeInfo):int
{
	//itemTypeList.Add(type);
	//var lIndex = itemTypeList.Count-1;
	//nameToTypeIndex[type.name]=lIndex;
	return itemTypeTable.addData(type.name,type);
}
/*
function getType( pIndex:int)
{
	return itemTypeList[pIndex];
}

function getTypeIndex( pName:String)
{
	return nameToTypeIndex[pName];
}

function getType( pName:String)
{
	return getType(getTypeIndex(pName));
}

function getTypeNum()
{
	return itemTypeList.Count;
}
*/

static protected var singletonInstance:zzItemSystem=null;

static function getSingleton()
{
	return singletonInstance;
}

function Awake()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
	
	//indexInGroup=new Array[itemTypeGroups.Length];
	//indexGroupDatas = new ItemGroupData[itemTypeGroups.Length];
	var i = 0;
	var lIndex=0;
	while(i<itemTypeGroups.Length)
	{
		//indexGroupDatas[i]=new ItemGroupData(
		indexGroupDatas.addData(itemTypeGroups[i].name,
			new ItemGroupData(
				itemTypeGroups[i].name,
				lIndex,
				lIndex+itemTypeGroups[i].itemTypeInfos.Length-1
			)
		);
		lIndex = lIndex+itemTypeGroups[i].itemTypeInfos.Length;
		++i;
	}
	
	for(var a =0;a<itemTypeGroups.Length;++a)
	{
		for(var itemTypeInfo:ItemTypeInfo in itemTypeGroups[a].itemTypeInfos)
		{
			addType(itemTypeInfo);
		}
		//addType(i);
	}
	
	//Debug.Log(itemTypeTable. getNum());
	Debug.Log(indexGroupDatas);
	Debug.Log(itemTypeTable.ToString());
}

class ItemBagData
{
	var name:String;
	//var dataChangedCall = zzUtilities.nullFunction;
	//按物品索引排列
	var dataChangedCallList : Object[];
	var networkView:NetworkView;
	var bagIndex;
	
	protected var itemList:int[];
	
	function getItemTypeNum()
	{
		return itemList.Length;
	}
	
	function addDataChangedCall(pCall,pBeginIndex:int,pEndIndex:int)
	{
		//dataChangedCall = pCall;
		for(var i = pBeginIndex; i<=pEndIndex;++i)
			dataChangedCallList[i] = pCall;
	}

	function ItemBagData(pName:String,itemTypeNum:int)
	{
		itemList=new int[itemTypeNum];
		dataChangedCallList = new Object[itemTypeNum];
		for(var i in dataChangedCallList)
			i = zzUtilities.nullFunction;
	}
	
	//改变数据,只能在服务器端
	function setNum(index:int,num:int)
	{
		//dataChangedCall();
		if(Network.peerType ==NetworkPeerType.Disconnected)
			_impSetNum(index,num);
		else
			networkView.RPC( "setBagItemNum", RPCMode.AllBuffered ,bagIndex , index,  num);
	}
	
	function _impSetNum(index:int,num:int)
	{
		if(num>999999)
			num=999999;
		if(num<0)
			num=0;
		itemList[index]=num;
		dataChangedCallList[index]();
	}
	
	function getNum(index:int)
	{
		return itemList[index];
	}
	
	function addItemOne(index:int)
	{
		//++itemList[index];
		setNum(index,itemList[index]+1);
		//dataChangedCall();
	}
	
	function addItem(index:int,number:int)
	{
		setNum(index,itemList[index]+number);
	}

	
	function removeItemOne(index:int)
	{
		setNum(index,itemList[index]-1);
		//dataChangedCall();
	}
	
	function ToString () : String 
	{
		var out="";
		for(var i:int in itemList)
			out=out+i+",";
		//return itemList.ToString();
		return out;
	}
};

var bagTable =zzIndexTable();

//必须在结束增加物品类型后 , 返回索引
function addBag(name:String):int
{
	var bagIndex = bagTable.addData(name,new ItemBagData(name,itemTypeTable.getNum()) );
	var lItemBagData:ItemBagData = bagTable.getData(bagIndex);
	lItemBagData.bagIndex = bagIndex;
	lItemBagData.networkView = networkView;
	return bagIndex;
}

function getBagTable()
{
	return bagTable;
}

@RPC
function setBagItemNum(pBagIndex:int,itemIndex:int,num:int)
{
	var lItemBagData:ItemBagData = bagTable.getData(pBagIndex);
	lItemBagData._impSetNum(itemIndex,num);
}

class ItemInfo
{
	var name:String;
	var number:String;
};