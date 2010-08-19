
//var money:int;

var  itemTypeList=Array();
var nameToTypeIndex=Hashtable();

class ItemTypeInfo
{
	var name:String;
}

var itemTypeInfos:ItemTypeInfo[];

//return index
function addType( type:ItemTypeInfo):int
{
	itemTypeList.Add(type);
	var lIndex = itemTypeList.Count-1;
	nameToTypeIndex[type.name]=lIndex;
	return lIndex;
}

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


static protected var singletonInstance:ItemSystem=null;

static function getSingleton()
{
	return singletonInstance;
}

function Awake()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
	
	for(var i:ItemTypeInfo in itemTypeInfos)
	{
		addType(i);
	}
}

class ItemInfo
{
	var name:String;
	var number:String;
};