

class zzVector3Serialize extends IzzUserDataSerializeString
{
	
	function zzVector3Serialize()
	{
		super(typeof(Vector3),"v3");
	}
	
	function userPack(pData:Object):String
	{
		var lV3:Vector3 = pData;
		//Debug.Log( pack(lV3.x) );
		return getUserDataDefineStr()+
			 zzSerializeString.getSingleton().pack(lV3.x)+
			  zzSerializeString.getSingleton().pack(lV3.y)+
			   zzSerializeString.getSingleton().pack(lV3.z);
	}
	
	function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
		//var t = pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		var lV3  = Vector3();
		lV3.x = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		lV3.y = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		lV3.z = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		pOut.data = lV3;
		return pPos;
	}
};

//----------------------------------------------------------------------------------------------


class zzQuaternionSerialize extends IzzUserDataSerializeString
{
	
	function zzQuaternionSerialize()
	{
		super(typeof(Quaternion),"q4");
	}
	
	function userPack(pData:Object):String
	{
		var ldata:Quaternion = pData;
		//Debug.Log( pack(lV3.x) );
		return getUserDataDefineStr()+
			 zzSerializeString.getSingleton().pack(ldata.x)+
			  zzSerializeString.getSingleton().pack(ldata.y)+
			   zzSerializeString.getSingleton().pack(ldata.z)+
			    zzSerializeString.getSingleton().pack(ldata.w);
	}
	
	function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
		//var t = pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		var ldata  = Quaternion();
		ldata.x = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		ldata.y = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		ldata.z = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		ldata.w = zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		pOut.data = ldata;
		return pPos;
	}
};

//----------------------------------------------------------------------------------------------


class zzPairSerialize extends IzzUserDataSerializeString
{
	
	function zzPairSerialize()
	{
		super(typeof(zzPair),"zzPair");
	}
	
	function userPack(pData:Object):String
	{
		var ldata:zzPair = pData;
		//Debug.Log( pack(lV3.x) );
		var zzSerialize = zzSerializeString.getSingleton();
		return getUserDataDefineStr()+zzSerialize.pack( ldata.left )+zzSerialize.pack( ldata.right );
	}
	
	function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
		//var t = pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		//var ldata  = Array();
		
		var zzSerialize = zzSerializeString.getSingleton();
		//var lremained = zzSerialize.unpack(pSerializePackList[pPos] as SerializePackData);
		//++pPos;
		var left = DataWrap();
		pPos = zzSerialize.unpack(pSerializePackList,pPos,left);
		var right = DataWrap();
		pPos = zzSerialize.unpack(pSerializePackList,pPos,right);
		
		pOut.data = zzPair(left,right);
		return pPos;
	}
};


//----------------------------------------------------------------------------------------------


class zzArraySerialize extends IzzUserDataSerializeString
{
	
	function zzArraySerialize()
	{
		super(typeof(Array),"Array");
	}
	
	function userPack(pData:Object):String
	{
		var ldata:Array = pData;
		//Debug.Log( pack(lV3.x) );
		var lOut = getUserDataDefineStr();
		var zzSerialize = zzSerializeString.getSingleton();
		lOut+=  zzSerialize.pack( ldata.Count );
		for(var v in ldata)
		{
			lOut+= zzSerialize.pack(v);
		}
		Debug.Log( lOut );
		return lOut;
	}
	
	function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
		//var t = pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		var ldata  = Array();
		
		var zzSerialize = zzSerializeString.getSingleton();
		var lremained = zzSerialize.unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		
		while(lremained!=0)
		{
			var lData = DataWrap();
			//lData.data = 
			pPos = zzSerialize.unpack(pSerializePackList,pPos,lData);
			ldata.Add(lData.data);
			//++pPos;
			--lremained;
		}
		
		pOut.data = ldata;
		return pPos;
	}
};

//----------------------------------------------------------------------------------------------


class zzTableSerialize extends IzzUserDataSerializeString
{
	
	function zzTableSerialize()
	{
		super(typeof(Hashtable),"table");
	}
	
	function userPack(pData:Object):String
	{
		var ldata:Hashtable = pData;
		//Debug.Log( pack(lV3.x) );
		var zzSerialize = zzSerializeString.getSingleton();
		var lArray=Array();
		for(var i:System.Collections.DictionaryEntry in ldata)
		{
			lArray.Add(new zzPair(i.Key,i.Value));
		}
		return getUserDataDefineStr()+zzSerialize.pack( lArray );
	}
	
	function userUnpack(pSerializePackList:Array,pPos:int,pOut:DataWrap):int//out end postion
	{
	
		var zzSerialize = zzSerializeString.getSingleton();
		var lOut = Hashtable();
		var lData = DataWrap();

		pPos = zzSerialize.unpack(pSerializePackList,pPos,lData);
		
		for(var i:zzPair in lData.data as Array )
		{
			lOut[i.left]=i.right;
		}
		
		pOut.data = lOut;
		return pPos;
	}
};

class zzTableSerialize2 extends zzTableSerialize
{
	function zzTableSerialize2()
	{
		setType(typeof(Boo.Lang.Hash),"table");
	}
}


//----------------------------------------------------------------------------------------------

static function registerMySerialize()
{
	var zzSerialize = zzSerializeString.getSingleton();
	zzSerialize.registerUserSerialize(new zzVector3Serialize());
	zzSerialize.registerUserSerialize(new zzQuaternionSerialize());
	zzSerialize.registerUserSerialize(new zzPairSerialize());
	zzSerialize.registerUserSerialize(new zzArraySerialize());
	zzSerialize.registerUserSerialize(new zzTableSerialize());
	zzSerialize.registerUserSerialize(new zzTableSerialize2());
}