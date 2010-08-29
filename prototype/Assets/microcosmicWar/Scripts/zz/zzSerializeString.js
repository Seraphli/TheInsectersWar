
class IzzUserDataSerializeString
{
	var type:System.Type;
	var typeName:String;
	
	var UserDataDefineStr:String;
	
	function IzzUserDataSerializeString(pType:System.Type,pTypeName:String)
	{
		//type = pType;
		//typeName = pTypeName;
		setType(pType,pTypeName);
		UserDataDefineStr=zzSerializeString.getSingleton().packUserData(typeName);
	}
	
	virtual function userPack(pData:Object):String
	{
	}
	
	virtual function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
	}
	
	function getUserDataDefineStr():String
	{
		return UserDataDefineStr;
	}
	
	protected function setType(pType:System.Type,pTypeName:String)
	{
		type = pType;
		typeName = pTypeName;
	}
};



static protected var singletonInstance:zzSerializeString;

static function getSingleton()
{
	return singletonInstance;
}

function zzSerializeString()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
}

protected var typeToSerializeMap=Hashtable();
protected var typeNameToSerializeMap=Hashtable();

function registerUserSerialize(pI:IzzUserDataSerializeString)
{
	typeToSerializeMap[pI.type]=pI;
	typeNameToSerializeMap[pI.typeName]=pI;
}

function getrUserSerializeFromType(type:System.Type):IzzUserDataSerializeString
{
	return typeToSerializeMap[type];
}

function getrUserSerializeFromTypeName(typeName:String):IzzUserDataSerializeString
{
	return typeNameToSerializeMap[typeName];
}

function pack(pStr:String,pStrType:String)
{
	return pStrType+","+pStr.Length+":"+pStr;
}

function pack(pStr:String)
{
	return  pack(pStr,"s");
}

function pack(pData:int)
{
	return pack(System.Convert.ToString(pData),"i");
}

function pack(pData:float)
{
	return pack(System.Convert.ToString(pData),"f");
}

function pack(pData:boolean)
{
	return pack(System.Convert.ToString(pData),"b");
}

function packUserData(pStr:String)
{
	return  pack(pStr,"u");
}

enum SerializePackType
{
	string,
	int,
	float,
	bool,
	userdata
}

function stringToSerializePackType(pStr:String)
{
	if(pStr=="s")
		return SerializePackType.string;
	else if(pStr=="i")
		return SerializePackType.int;
	else if(pStr=="f")
		return SerializePackType.float;
	else if(pStr=="b")
		return SerializePackType.bool;
	else if(pStr=="u")
		return SerializePackType.userdata;
	else
		Debug.LogError("stringToSerializePackType error type:"+pStr);
}

function SerializePackTypeToString(pType:SerializePackType)
{
	switch(pType)
	{
		case SerializePackType.string:return "s";
		case SerializePackType.int: return "i";
		case SerializePackType.float: return "f";
		case SerializePackType.bool: return "b";
		case SerializePackType.userdata: return "u";
	};
	Debug.LogError("SerializePackTypeToString error type:"+pType);
	return "";
}

class SerializePackData
{
	var type:SerializePackType;
	var data:String;
	
	function ToString () : String 
	{
		return "value:"+data+" type:"+type;
	}
};

function unpack(pSerializePackData:SerializePackData)
{
	switch(pSerializePackData.type)
	{
		case SerializePackType.string:return pSerializePackData.data;
		case SerializePackType.int: return System.Convert.ToInt32(pSerializePackData.data);
		case SerializePackType.float: return System.Convert.ToSingle(pSerializePackData.data);
		case SerializePackType.bool: return System.Convert.ToBoolean(pSerializePackData.data);
		case SerializePackType.userdata: return pSerializePackData.data;
	};
	Debug.LogError("SerializePackTypeToString error type:"+pSerializePackData.type);
	return null;
}

function unpackOne(pStr:String,pBeginPos:int, pSerializePackData:SerializePackData)
{
	//var lStrOut:String=pStrOut;
	var lCommaIndex:int = pStr.IndexOf(",",pBeginPos);
	//Debug.Log(pStr.Substring(pBeginPos) );
	pSerializePackData.type=stringToSerializePackType(pStr.Substring(pBeginPos,lCommaIndex-pBeginPos));
	var lCutIndex:int = pStr.IndexOf(":",++lCommaIndex);
	var lStrLength:int=  System.Convert.ToInt32( pStr.Substring(lCommaIndex,lCutIndex-lCommaIndex) );
	//Debug.Log(lCutIndex);
	//Debug.Log(lStrLength);
	pSerializePackData.data=pStr.Substring(++lCutIndex,lStrLength);
	//Debug.Log(lCutIndex+lStrLength);
	//Debug.Log(lStrOut);
	return lCutIndex+lStrLength;
	//返回新索引位置
}

function unpackToList(pStr:String)
{
	var lOut=Array();
	var i=0;
	while(i<pStr.Length)
	{
		var lData=SerializePackData();
		i = unpackOne(pStr,i,lData);
		lOut.Add(lData);
	}
	return lOut;
}

//pSerializePackList  SerializePackData[]  , 
function unpack(pSerializePackList:Array,pPos:int,pOut:DataWrap)
{
	var lSerializePackData:SerializePackData = pSerializePackList[pPos];
	if(lSerializePackData.type == SerializePackType.userdata)
	{
		return getrUserSerializeFromTypeName(lSerializePackData.data)
			.userUnpack(pSerializePackList,pPos+1,pOut);
	}
	else
	{
		pOut.data=unpack(lSerializePackData);
		return pPos+1;
	}
}

function pack(pData:Object):String
{
	//Debug.Log(typeof(pData));
	switch(typeof(pData))
	{
		case String: return pack(pData as String);
		case int: return pack( System.Convert.ToInt32( pData ) );
		case float: return pack( System.Convert.ToSingle(pData));
		case boolean: return pack( System.Convert.ToBoolean(pData));
	}
	
	//Debug.Log(getrUserSerializeFromType(typeof(pData)));
	return  getrUserSerializeFromType(typeof(pData)).userPack(pData);
}

//返回第一个数据 一般用以 Hashtable ,Array
function unpackToData(pStr:String):Object
{
	var lUnpackList = unpackToList(pStr);
	var i=0;
	var lOut =DataWrap();
	//while(i<lUnpackList.Count)
	//{
	//	i=unpack(lUnpackList,i,lOut);
	//}
	unpack(lUnpackList,i,lOut);
	return lOut.data;
}

function Awake()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
	zzMySerializeString.registerMySerialize();
	
	/*
	var ltable:Hashtable = Hashtable();
	print(ltable);
	 ltable =	{
					"a":123,
					"b":345,
					"fasda":"adsf",
					"zzzz":false,
					"face":0
				};
				
	var lPacked= pack(ltable);
	var lUnPacked = unpackToData(lPacked);
	for(var i:System.Collections.DictionaryEntry in ltable)
	{
		print(""+i.Key+" "+i.Value);
	}
	print("----------------------------------------");
	print("----------------------------------------");
	for(var i:System.Collections.DictionaryEntry in lUnPacked)
	{
		print(""+i.Key+" "+i.Value);
	}
	print(lUnPacked["face"]);
	*/
	
	
	/*
	var ltable:Hashtable = Hashtable();
	print(ltable);
	 ltable =	{
					"a":123,
					"b":345,
					"fasda":"adsf",
					"zzzz":false
				};
	print(ltable);
	print(typeof(ltable));
	print(typeof(Hashtable));
	
	//registerUserSerialize(new zzVector3Serialize());
	var lPacked= pack("a")+pack(123)+pack(true)+pack(1.5/34)+pack(Vector3(1,2,3))
		+pack(Quaternion(7,6,5,4))+pack( zzPair(4,Vector3(4,5,6)) )
		+pack(ltable as Hashtable);
	var lUnpackList = unpackToList(lPacked);
	var i=0;
	var lOut =DataWrap();
	while(i<lUnpackList.Count)
	{
		i=unpack(lUnpackList,i,lOut);
		//print(i);
		//print(lUnpackList.Count);
		if(typeof(lOut.data)==Hashtable)
		{
			for(var i:System.Collections.DictionaryEntry in lOut.data)
			{
				//print(i.Key);
				//print(i.Value);
				print(""+i.Key+" "+i.Value);
			}
		}
		print(lOut.data);
	}*/
}

function Update () {
}