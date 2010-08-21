
//var money:int;

//var  itemTypeList=Array();
//var nameToTypeIndex=Hashtable();

class ItemTypeInfo
{
	var name:String;
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

	function ItemBagData(pName:String,itemTypeNum:int)
	{
		itemList=new int[itemTypeNum];
	}
	
	function setNum(index:int,num:int)
	{
		itemList[index]=num;
	}
	
	function getNum(index:int)
	{
		return itemList[index];
	}
	
	function addItemOne(index:int)
	{
		return ++itemList[index];
	}
	
	function addItem(index:int,number:int)
	{
		itemList[index]+=number;
	}

	
	function removeItemOne(index:int)
	{
		return --itemList[index];
	}
	
	protected var itemList:int[];
	
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

//必须在结束增加物品类型后
function addBag(name:String)
{
	return bagTable.addData(name,new ItemBagData(name,itemTypeTable.getNum()) );
}

function getBagTable()
{
	return bagTable;
}

class ItemInfo
{
	var name:String;
	var number:String;
};