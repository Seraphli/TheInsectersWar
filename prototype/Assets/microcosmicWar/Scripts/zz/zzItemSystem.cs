
using UnityEngine;
using System.Collections;


[System.Serializable]
public class ItemBagData
{
    public string name;
    //FIXME_VAR_TYPE dataChangedCall= zzUtilities.nullFunction;
    //按物品索引排列
    public zzUtilities.voidFunction[] dataChangedCallList;
    public NetworkView networkView;
    public int bagIndex;

    protected int[] itemList;

    public int getItemTypeNum()
    {
        return itemList.Length;
    }

    public void addDataChangedCall(zzUtilities.voidFunction pCall, int pBeginIndex, int pEndIndex)
    {
        //dataChangedCall = pCall;
        for (int i = pBeginIndex; i <= pEndIndex; ++i)
            dataChangedCallList[i] = pCall;
    }

    public ItemBagData(string pName, int itemTypeNum)
    {
        itemList = new int[itemTypeNum];
        dataChangedCallList = new zzUtilities.voidFunction[itemTypeNum];
        //foreach (zzUtilities.voidFunction lFunc in dataChangedCallList)
        //    lFunc = new zzUtilities.voidFunction(zzUtilities.nullFunction);
        for (int i = 0; i < dataChangedCallList.Length; ++i)
            dataChangedCallList[i] = zzUtilities.nullFunction;
    }

    //改变数据,只能在服务器端
    public void setNum(int index, int num)
    {
        //dataChangedCall();
        if (Network.peerType == NetworkPeerType.Disconnected)
            _impSetNum(index, num);
        else
            networkView.RPC("setBagItemNum", RPCMode.All, bagIndex, index, num);
    }

    public void _impSetNum(int index, int num)
    {
        if (num > 999999)
            num = 999999;
        if (num < 0)
            num = 0;
        itemList[index] = num;
        //FIXME_VAR_TYPE lTempCall = zzUtilities.nullFunction;

        //lTempCall = dataChangedCallList[index];
        //lTempCall();
        //Debug.Log(dataChangedCallList == null);
        //Debug.Log(dataChangedCallList[index] == null);
        dataChangedCallList[index]();
    }

    public int getNum(int index)
    {
        return itemList[index];
    }

    public void addItemOne(int index)
    {
        //++itemList[index];
        setNum(index, itemList[index] + 1);
        //dataChangedCall();
    }

    public void addItem(int index, int number)
    {
        setNum(index, itemList[index] + number);
    }


    public void removeItemOne(int index)
    {
        setNum(index, itemList[index] - 1);
        //dataChangedCall();
    }

    public override string ToString(){
		string lOut="";
		foreach(int i in itemList)
            lOut = lOut + i + ",";
		//return itemList.ToString();
		return lOut;
	}
};

[System.Serializable]
public class ItemTypeInfo
{
    public string name;

    public Texture image;

    //string nameForShow;

    public Texture getImage()
    {
        return image;
    }
    /*
    protected FIXME_VAR_TYPE groupIndex=0;
	
    void  setGroupIndex ( int pGroupIndex  ){
        groupIndex = pGroupIndex;
    }
	
    void  getGroupIndex (){
        return groupIndex;
    }*/

    public GameObject useItem;
    //string useItemComponentName;
    //protected GameObject useItemObject;

    public ItemTypeInfo()
    {	/*
		Debug.Log(name+(useItem!=null));
		if(useItem)
		{
			useItemObject = UnityEngine.Object.Instantiate(useItem,Vector3(),Quaternion());
			useItemObject.name = "name/"+name;
		}*/
    }

    public IitemObject getItemObject()
    {
        //Debug.Log(useItemComponentName);
        //Debug.Log(useItemObject!=null);
        //return useItemObject.GetComponent<useItemComponentName>();
        return useItem.GetComponent<IitemObject>();
    }

    public override string ToString()
    {
        return "Item Name:" + name;
    }
};

public class zzItemSystem : MonoBehaviour
{


    //int money;

    //FIXME_VAR_TYPE itemTypeList=Array();
    //FIXME_VAR_TYPE nameToTypeIndex=Hashtable();
    [System.Serializable]
    public class ItemTypeGroup
    {
        public string name;
        public ItemTypeInfo[] itemTypeInfos;
    };

    public ItemTypeGroup[] itemTypeGroups;

    protected zzIndexTable itemTypeTable = new zzIndexTable();

    protected zzIndexTable indexGroupDatas = new zzIndexTable();

    public class ItemGroupData
    {
        public ItemGroupData(string pName, int pIndexBegin, int pIndexEnd)
        {
            name = pName;
            indexBegin = pIndexBegin;
            indexEnd = pIndexEnd;
        }

        public string name;
        protected int indexBegin = 0;
        protected int indexEnd = 0;

        public override string ToString()
        {
            return "Item Group( Name:" + name + ", " + indexBegin + ", " + indexEnd + ")";
        }

        public int getIndexInBegin()
        {
            return indexBegin;
        }

        public int getIndexInEnd()
        {
            return indexEnd;
        }

        public int getItemNum()
        {
            return indexEnd - indexBegin;
        }
    };

    /// <summary>
    /// 或者物品类型表
    /// </summary>
    /// <returns></returns>
    public zzIndexTable getItemTypeTable()
    {
        return itemTypeTable;
    }

    public zzIndexTable getGroupTable()
    {
        return indexGroupDatas;
    }

    public int getGroupNum()
    {
        return indexGroupDatas.getNum();
    }



    //return index
    protected int addType(ItemTypeInfo type)
    {
        //itemTypeList.Add(type);
        //FIXME_VAR_TYPE lIndex= itemTypeList.Count-1;
        //nameToTypeIndex[type.name]=lIndex;
        return itemTypeTable.addData(type.name, type);
    }
    /*
    void  getType (  int pIndex  ){
        return itemTypeList[pIndex];
    }

    void  getTypeIndex (  string pName  ){
        return nameToTypeIndex[pName];
    }

    void  getType (  string pName  ){
        return getType(getTypeIndex(pName));
    }

    void  getTypeNum (){
        return itemTypeList.Count;
    }
    */

    static protected zzItemSystem singletonInstance = null;

    public static zzItemSystem getSingleton()
    {
        return singletonInstance;
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;

        //indexInGroup=new Array[itemTypeGroups.Length];
        //indexGroupDatas = new ItemGroupData[itemTypeGroups.Length];
        int i = 0;
        int lIndex = 0;
        while (i < itemTypeGroups.Length)
        {
            //indexGroupDatas[i]=new ItemGroupData(
            indexGroupDatas.addData(itemTypeGroups[i].name,
                new ItemGroupData(
                    itemTypeGroups[i].name,
                    lIndex,
                    lIndex + itemTypeGroups[i].itemTypeInfos.Length - 1
                )
            );
            lIndex = lIndex + itemTypeGroups[i].itemTypeInfos.Length;
            ++i;
        }

        for (int a = 0; a < itemTypeGroups.Length; ++a)
        {
            foreach (ItemTypeInfo itemTypeInfo in itemTypeGroups[a].itemTypeInfos)
            {
                addType(itemTypeInfo);
            }
            //addType(i);
        }

        //Debug.Log(itemTypeTable. getNum());
        //Debug.Log(indexGroupDatas);
        //Debug.Log(itemTypeTable.ToString());
    }

    protected zzIndexTable bagTable = new zzIndexTable();

    //必须在结束增加物品类型后 , 返回索引
    public int addBag(string name)
    {
        int bagIndex = bagTable.addData(name, new ItemBagData(name, itemTypeTable.getNum()));
        ItemBagData lItemBagData = (ItemBagData)bagTable.getData(bagIndex);
        lItemBagData.bagIndex = bagIndex;
        lItemBagData.networkView = networkView;
        return bagIndex;
    }

    /// <summary>
    /// 获取存储物品包的表
    /// </summary>
    /// <returns></returns>
    public zzIndexTable getBagTable()
    {
        return bagTable;
    }

    [RPC]
    public void setBagItemNum(int pBagIndex, int itemIndex, int num)
    {
        ItemBagData lItemBagData = (ItemBagData)bagTable.getData(pBagIndex);
        lItemBagData._impSetNum(itemIndex, num);
    }

    //[System.Serializable]
    //public class ItemInfo
    //{
    //    public string name;
    //    public string number;
    //};
}