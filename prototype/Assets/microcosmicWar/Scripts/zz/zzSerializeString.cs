
using UnityEngine;
using System.Collections;


public enum SerializePackType
{
    stringType,
    intType,
    floatType,
    boolType,
    userdata
}

public class SerializePackData
{
    public SerializePackType type;
    public string data;

    override public string ToString()
    {
        return "value:" + data + " type:" + type;
    }
};

public abstract class IzzUserDataSerializeString
{
    public System.Type type;
    public string typeName;

    public string UserDataDefineStr;

    public IzzUserDataSerializeString(System.Type pType, string pTypeName)
    {
        //type = pType;
        //typeName = pTypeName;
        setType(pType, pTypeName);
        UserDataDefineStr = zzSerializeString.getSingleton().packUserData(typeName);
    }

    public abstract string userPack(object pData);

    //out end postion
    public abstract int userUnpack(ArrayList pSerializePackList, int pPos, DataWrap pOut);

    public string getUserDataDefineStr()
    {
        return UserDataDefineStr;
    }

    protected void setType(System.Type pType, string pTypeName)
    {
        type = pType;
        typeName = pTypeName;
    }
};


public class zzSerializeString
{



    static protected zzSerializeString singletonInstance = new zzSerializeString();

    public static zzSerializeString getSingleton()
    {
        return singletonInstance;
    }

    public static zzSerializeString Singleton
    {
        get { return singletonInstance; }
    }

    //public zzSerializeString (){
    //    if(singletonInstance)
    //        Debug.LogError("have singletonInstance");
    //    singletonInstance = this;
    //}

    protected Hashtable typeToSerializeMap = new Hashtable();
    protected Hashtable typeNameToSerializeMap = new Hashtable();

    public void registerUserSerialize(IzzUserDataSerializeString pI)
    {
        if (typeToSerializeMap.Contains(pI.type))
            Debug.LogError("typeToSerializeMap.Contains(pI.type)==true");
        typeToSerializeMap[pI.type] = pI;

        if (typeNameToSerializeMap.Contains(pI.typeName))
            Debug.LogError("typeNameToSerializeMap.Contains(pI.typeName)==true");
        typeNameToSerializeMap[pI.typeName] = pI;
    }

    public IzzUserDataSerializeString getUserSerializeFromType(System.Type type)
    {
        return typeToSerializeMap[type] as IzzUserDataSerializeString;
    }

    public IzzUserDataSerializeString getrUserSerializeFromTypeName(string typeName)
    {
        return typeNameToSerializeMap[typeName] as IzzUserDataSerializeString;
    }

    public string pack(string pStr, string pStrType)
    {
        return pStrType + "," + pStr.Length + ":" + pStr;
    }

    public string pack(string pStr)
    {
        return pack(pStr, "s");
    }

    public string pack(int pData)
    {
        return pack(System.Convert.ToString(pData), "i");
    }

    public string pack(float pData)
    {
        return pack(System.Convert.ToString(pData), "f");
    }

    public string pack(bool pData)
    {
        //return pack(System.Convert.ToString(pData), "b");
        return pack(pData ? "1" : "0", "b");
    }

    public string packUserData(string pStr)
    {
        return pack(pStr, "\n");
    }

    public SerializePackType stringToSerializePackType(string pStr)
    {
        if (pStr == "s")
            return SerializePackType.stringType;
        else if (pStr == "i")
            return SerializePackType.intType;
        else if (pStr == "f")
            return SerializePackType.floatType;
        else if (pStr == "b")
            return SerializePackType.boolType;
        else if (pStr == "\n")
            return SerializePackType.userdata;
        else
            Debug.LogError("stringToSerializePackType error type:" + pStr);
        return 0;
    }

    //public string SerializePackTypeToString(SerializePackType pType)
    //{
    //    switch (pType)
    //    {
    //        case SerializePackType.stringType: return "s";
    //        case SerializePackType.intType: return "i";
    //        case SerializePackType.floatType: return "f";
    //        case SerializePackType.boolType: return "b";
    //        case SerializePackType.userdata: return "u";
    //    };
    //    Debug.LogError("SerializePackTypeToString error type:" + pType);
    //    return "";
    //}

    public object unpack(SerializePackData pSerializePackData)
    {
        switch (pSerializePackData.type)
        {
            case SerializePackType.stringType: return pSerializePackData.data;
            case SerializePackType.intType: return System.Convert.ToInt32(pSerializePackData.data);
            case SerializePackType.floatType: return System.Convert.ToSingle(pSerializePackData.data);
            //case SerializePackType.boolType: return System.Convert.ToBoolean(pSerializePackData.data);
            case SerializePackType.boolType: return (pSerializePackData.data=="0" ? false : true);
            case SerializePackType.userdata: return pSerializePackData.data;
        };
        Debug.LogError("SerializePackTypeToString error type:" + pSerializePackData.type);
        return null;
    }

    //返回新索引位置
    public int unpackOne(string pStr, int pBeginPos, SerializePackData pSerializePackData)
    {
        //string lStrOut=pStrOut;
        int lCommaIndex = pStr.IndexOf(",", pBeginPos);
        //Debug.Log(pStr.Substring(pBeginPos) );
        pSerializePackData.type = stringToSerializePackType(pStr.Substring(pBeginPos, lCommaIndex - pBeginPos));
        int lCutIndex = pStr.IndexOf(":", ++lCommaIndex);
        int lStrLength = System.Convert.ToInt32(pStr.Substring(lCommaIndex, lCutIndex - lCommaIndex));
        //Debug.Log(lCutIndex);
        //Debug.Log(lStrLength);
        pSerializePackData.data = pStr.Substring(++lCutIndex, lStrLength);
        //Debug.Log(lCutIndex+lStrLength);
        //Debug.Log(lStrOut);
        return lCutIndex + lStrLength;
        //返回新索引位置
    }

    public ArrayList unpackToList(string pStr)
    {
        ArrayList lOut = new ArrayList();
        int i = 0;
        while (i < pStr.Length)
        {
            SerializePackData lData =new SerializePackData();
            i = unpackOne(pStr, i, lData);
            lOut.Add(lData);
        }
        return lOut;
    }

    //pSerializePackList  SerializePackData[]  , 
    public int unpack(ArrayList pSerializePackList, int pPos, DataWrap pOut)
    {
        SerializePackData lSerializePackData = pSerializePackList[pPos] as SerializePackData;
        if (lSerializePackData.type == SerializePackType.userdata)
        {
            return getrUserSerializeFromTypeName(lSerializePackData.data)
                .userUnpack(pSerializePackList, pPos + 1, pOut);
        }
        else
        {
            pOut.data = unpack(lSerializePackData);
            return pPos + 1;
        }
    }

    public string pack(object pData)
    {
        //Debug.Log(typeof(pData));
        //switch (pData.GetType())
        //{
        //    case typeof(String): return pack(pData as string);
        //    case typeof(int): return pack(System.Convert.ToInt32(pData));
        //    case typeof(float): return pack(System.Convert.ToSingle(pData));
        //    case typeof(bool): return pack(System.Convert.ToBoolean(pData));
        //}

        System.Type lType = pData.GetType();
        if (lType == typeof(string)) return pack(pData as string);
        else if(lType == typeof(int))return pack(System.Convert.ToInt32(pData));
        else if (lType == typeof(float)) return pack(System.Convert.ToSingle(pData));
        else if (lType == typeof(bool)) return pack(System.Convert.ToBoolean(pData));

        //Debug.Log(getrUserSerializeFromType(typeof(pData)));
        return getUserSerializeFromType(pData.GetType()).userPack(pData);
    }

    public bool isSupportedType(System.Type lType)
    {
        if (lType != typeof(string)
            && lType != typeof(int)
            && lType != typeof(float)
            && lType != typeof(bool)
            && !typeToSerializeMap.Contains(lType))
            return false;
        return true;
    }

    //返回第一个数据 一般用以 Hashtable ,Array
    public object unpackToData(string pStr)
    {
        ArrayList lUnpackList = unpackToList(pStr);
        int i = 0;
        DataWrap lOut = new DataWrap();
        //while(i<lUnpackList.Count)
        //{
        //	i=unpack(lUnpackList,i,lOut);
        //}
        unpack(lUnpackList, i, lOut);
        return lOut.data;
    }

    //void Awake()
    zzSerializeString()
    {
        if (singletonInstance!=null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        zzMySerializeString.registerMySerialize();

        /*
        Hashtable ltable = Hashtable();
        print(ltable);
         ltable =	{
                        "a":123,
                        "b":345,
                        "fasda":"adsf",
                        "zzzz":false,
                        "face":0
                    };
				
        FIXME_VAR_TYPE lPacked= pack(ltable);
        FIXME_VAR_TYPE lUnPacked= unpackToData(lPacked);
        foreach(System.Collections.DictionaryEntry i in ltable)
        {
            print(""+i.Key+" "+i.Value);
        }
        print("----------------------------------------");
        print("----------------------------------------");
        foreach(System.Collections.DictionaryEntry i in lUnPacked)
        {
            print(""+i.Key+" "+i.Value);
        }
        print(lUnPacked["face"]);
        */


        /*
        Hashtable ltable = Hashtable();
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
        FIXME_VAR_TYPE lPacked= pack("a")+pack(123)+pack(true)+pack(1.5f/34)+pack(Vector3(1,2,3))
            +pack(Quaternion(7,6,5,4))+pack( zzPair(4,Vector3(4,5,6)) )
            +pack(ltable as Hashtable);
        FIXME_VAR_TYPE lUnpackList= unpackToList(lPacked);
        FIXME_VAR_TYPE i=0;
        FIXME_VAR_TYPE lOut=DataWrap();
        while(i<lUnpackList.Count)
        {
            i=unpack(lUnpackList,i,lOut);
            //print(i);
            //print(lUnpackList.Count);
            if(typeof(lOut.data)==Hashtable)
            {
                foreach(System.Collections.DictionaryEntry i in lOut.data)
                {
                    //print(i.Key);
                    //print(i.Value);
                    print(""+i.Key+" "+i.Value);
                }
            }
            print(lOut.data);
        }*/
    }

    //void  Update (){
    //}
}