
using UnityEngine;
using System.Collections;



class zzVector3Serialize : IzzUserDataSerializeString
{
	
	public zzVector3Serialize ():base(typeof(Vector3),"v3")
    {
	}
	
	public override string userPack ( object pData  ){
        Vector3 lV3 = (Vector3)pData;
		//Debug.Log( pack(lV3.x) );
		return getUserDataDefineStr()+
			 zzSerializeString.getSingleton().pack(lV3.x)+
			  zzSerializeString.getSingleton().pack(lV3.y)+
			   zzSerializeString.getSingleton().pack(lV3.z);
	}

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
	{
		//FIXME_VAR_TYPE t= pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		Vector3 lV3  = new Vector3();
		lV3.x = (float) zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
        lV3.y = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
        lV3.z = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		pOut.data = lV3;
		return pPos;
	}
};

//----------------------------------------------------------------------------------------------

class zzVector2Serialize : IzzUserDataSerializeString
{

    public zzVector2Serialize()
        : base(typeof(Vector2), "v2")
    {
    }

    public override string userPack(object pData)
    {
        Vector2 lV2 = (Vector2)pData;
        //Debug.Log( pack(lV2.x) );
        return getUserDataDefineStr() +
             zzSerializeString.getSingleton().pack(lV2.x) +
              zzSerializeString.getSingleton().pack(lV2.y);
    }

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
    {
        //FIXME_VAR_TYPE t= pSerializePackList[pPos] as SerializePackData;
        //Debug.Log(t);
        //Debug.Log(zzSerializeString.getSingleton().unpack(t));
        Vector2 lV2 = new Vector2();
        lV2.x = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
        ++pPos;
        lV2.y = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
        ++pPos;
        pOut.data = lV2;
        return pPos;
    }
};


//----------------------------------------------------------------------------------------------

class zzQuaternionSerialize : IzzUserDataSerializeString
{
	
	public  zzQuaternionSerialize ():base(typeof(Quaternion),"q4")
    {
		
	}

    public override string userPack(object pData)
    {
        Quaternion ldata = (Quaternion)pData;
		//Debug.Log( pack(lV3.x) );
		return getUserDataDefineStr()+
			 zzSerializeString.getSingleton().pack(ldata.x)+
			  zzSerializeString.getSingleton().pack(ldata.y)+
			   zzSerializeString.getSingleton().pack(ldata.z)+
			    zzSerializeString.getSingleton().pack(ldata.w);
	}

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
	{
		//FIXME_VAR_TYPE t= pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		Quaternion ldata = new Quaternion();
		ldata.x = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
        ldata.y = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
        ldata.z = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
        ldata.w = (float)zzSerializeString.getSingleton().unpack(pSerializePackList[pPos] as SerializePackData);
		++pPos;
		pOut.data = ldata;
		return pPos;
	}
};

//----------------------------------------------------------------------------------------------


class zzPairSerialize : IzzUserDataSerializeString
{
	
    //public  zzPairSerialize ():base(typeof(zzPair),"zzPair")
	public  zzPairSerialize ():base(typeof(zzPair),"zp")
    {
		
	}

    public override string userPack(object pData)
    {
        zzPair ldata = (zzPair)pData;
		//Debug.Log( pack(lV3.x) );
        zzSerializeString zzSerialize = zzSerializeString.getSingleton();
		return getUserDataDefineStr()+zzSerialize.pack( ldata.left )+zzSerialize.pack( ldata.right );
	}

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
	{
		//FIXME_VAR_TYPE t= pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		//FIXME_VAR_TYPE ldata= Array();
		
		zzSerializeString zzSerialize= zzSerializeString.getSingleton();
		//FIXME_VAR_TYPE lremained= zzSerialize.unpack(pSerializePackList[pPos] as SerializePackData);
		//++pPos;
		DataWrap left= new DataWrap();
		pPos = zzSerialize.unpack(pSerializePackList,pPos,left);
		DataWrap right= new DataWrap();
		pPos = zzSerialize.unpack(pSerializePackList,pPos,right);
		
		pOut.data = new zzPair(left.data,right.data);
		return pPos;
	}
};


//----------------------------------------------------------------------------------------------


class zzArrayListSerialize : IzzUserDataSerializeString
{

    public zzArrayListSerialize()
        : base(typeof(ArrayList), "list")
    {
		
	}

    public override string userPack(object pData)
    {
        ArrayList ldata = (ArrayList)pData;
		//Debug.Log( pack(lV3.x) );
		string lOut= getUserDataDefineStr();
		zzSerializeString zzSerialize= zzSerializeString.getSingleton();
		lOut+=  zzSerialize.pack( ldata.Count );
		foreach(object v in ldata)
		{
			lOut+= zzSerialize.pack(v);
		}
		//Debug.Log( lOut );
		return lOut;
	}

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
	{
		//FIXME_VAR_TYPE t= pSerializePackList[pPos] as SerializePackData;
		//Debug.Log(t);
		//Debug.Log(zzSerializeString.getSingleton().unpack(t));
		
		zzSerializeString zzSerialize = zzSerializeString.getSingleton();

        //读取出数组的大小
        int lremained = (int)zzSerialize.unpack(pSerializePackList[pPos] as SerializePackData);
        ArrayList ldata = new ArrayList(lremained);
		++pPos;
		
		while(lremained!=0)
		{
            DataWrap lData = new DataWrap();
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


class zzArraySerialize<T> : IzzUserDataSerializeString
{

    public zzArraySerialize(string pName)
        : base(typeof(T[]), pName)
    {

    }

    public override string userPack(object pData)
    {
        T[] ldata = (T[])pData;
        string lOut = getUserDataDefineStr();
        zzSerializeString zzSerialize = zzSerializeString.getSingleton();
        lOut += zzSerialize.pack(ldata.Length);
        foreach (object v in ldata)
        {
            lOut += zzSerialize.pack(v);
        }
        return lOut;
    }

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
    {

        zzSerializeString zzSerialize = zzSerializeString.getSingleton();

        //读取出数组的大小
        int lremained = (int)zzSerialize.unpack(pSerializePackList[pPos] as SerializePackData);
        T[] ldata = new T[lremained];
        ++pPos;

        int i = 0;
        //while (lremained != 0)
        while (lremained != i)
        {
            DataWrap lData = new DataWrap();

            pPos = zzSerialize.unpack(pSerializePackList, pPos, lData);
            ldata[i] = (T)lData.data;
            ++i;
            //--lremained;
        }

        pOut.data = ldata;
        return pPos;
    }
};

//----------------------------------------------------------------------------------------------


class zzTableSerialize : IzzUserDataSerializeString
{

    public zzTableSerialize()
        : base(typeof(Hashtable), "table")
    {
	}

    public override string userPack(object pData)
    {
        Hashtable ldata = (Hashtable)pData;
		//Debug.Log( pack(lV3.x) );
		zzSerializeString zzSerialize = zzSerializeString.getSingleton();
		ArrayList lArray=new ArrayList();
		foreach(System.Collections.DictionaryEntry i in ldata)
		{
			lArray.Add(new zzPair(i.Key,i.Value));
		}
		return getUserDataDefineStr()+zzSerialize.pack( lArray );
	}

    public override int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)//out end postion
	{
	
		zzSerializeString zzSerialize= zzSerializeString.getSingleton();
		Hashtable lOut= new Hashtable();
		DataWrap lData= new DataWrap();

		pPos = zzSerialize.unpack(pSerializePackList,pPos,lData);
		
		foreach(zzPair i in lData.data as ArrayList )
		{
			lOut[i.left]=i.right;
		}
		
		pOut.data = lOut;
		return pPos;
	}
};

//class zzTableSerialize2 : zzTableSerialize
//{
//    public  zzTableSerialize2 (){
//        setType(typeof(Boo.Lang.Hash),"table");
//    }
//}


//----------------------------------------------------------------------------------------------

public class zzMySerializeString
{

    public static void registerMySerialize()
    {
        zzSerializeString zzSerialize = zzSerializeString.getSingleton();
        zzSerialize.registerUserSerialize(new zzArraySerialize<int>("iAr"));
        zzSerialize.registerUserSerialize(new zzArraySerialize<float>("fAr"));
        zzSerialize.registerUserSerialize(new zzArraySerialize<bool>("bAr"));
        zzSerialize.registerUserSerialize(new zzArraySerialize<string>("sAr"));
        zzSerialize.registerUserSerialize(new zzArraySerialize<Hashtable>("tableAr"));
        zzSerialize.registerUserSerialize(new zzVector3Serialize());
        zzSerialize.registerUserSerialize(new zzArraySerialize<Vector3>("v3Ar"));
        zzSerialize.registerUserSerialize(new zzVector2Serialize()); ;
        zzSerialize.registerUserSerialize(new zzArraySerialize<Vector2>("v2Ar"));
        zzSerialize.registerUserSerialize(new zzQuaternionSerialize());
        zzSerialize.registerUserSerialize(new zzArraySerialize<Quaternion>("q4Ar"));
        zzSerialize.registerUserSerialize(new zzPairSerialize());
        zzSerialize.registerUserSerialize(new zzArrayListSerialize());
        zzSerialize.registerUserSerialize(new zzTableSerialize());
        //zzSerialize.registerUserSerialize(new zzTableSerialize2());
    }

}