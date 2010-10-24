
using UnityEngine;
using System.Collections;

public class zzItemBagControl : MonoBehaviour
{


    public int bagIndex = 0;
    //若为真 代表是已有的包,未设置就用bagName创建新包
    public bool useExistBag = false;
    public string bagName = "";
    public GameObject owner;

    protected zzItemSystem itemSystem;
    protected bool haveBeInited = false;

    //为了解决初次刷新时 UI和此类 初始化先后的问题
    public ArrayList callListAfterStart = new ArrayList();

    public void setUseExistBag(int pBagIndex)
    {
        //useExistBag = pUse;
        //bagIndex = pBagIndex;

        zzCreatorUtility.sendMessage(gameObject, "impSetUseExistBag", pBagIndex);
    }

    [RPC]
    public void impSetUseExistBag(int pBagIndex)
    {
        useExistBag = true;
        bagIndex = pBagIndex;
    }

    public void addCallAfterStart(zzUtilities.voidFunction pCall)
    {
        //for 转换
        zzUtilities.voidFunction lTempCall = pCall;
        lTempCall = pCall;
        if (haveBeInited)
            lTempCall();
        //pCall();
        else
            callListAfterStart.Add(pCall);
    }

    protected void callAfterStart()
    {
        //for 转换
        //zzUtilities.voidFunction lTempCall ;

        foreach (zzUtilities.voidFunction call in callListAfterStart)
        {
           // lTempCall = call;
            //lTempCall();
            call();
        }
        callListAfterStart.Clear();
    }


    public zzItemSystem getItemSystem()
    {
        return itemSystem;
    }

    [System.Serializable]
    public class zzMyItemInBagInfo
    {
        public string name;
        public int number;
    }

    public zzMyItemInBagInfo[] itemInBagInfo;

    public void setItemChangedCall(zzUtilities.voidFunction pCall)
    {
        getBagData().addDataChangedCall(pCall, 1, getBagData().getItemTypeNum() - 1);
    }

    public void setMoneyChangedCall(zzUtilities.voidFunction pCall)
    {
        getBagData().addDataChangedCall(pCall, 0, 0);
    }

    public ItemBagData getBagData()
    {
        return itemSystem.getBagTable().getData(bagIndex) as ItemBagData;
    }

    public void setItemNum(string pName, int pNum)
    {
        getBagData().setNum(itemSystem.getItemTypeTable().getIndex(pName), pNum);
    }

    //0 为钱
    public void addItemOne(int pItemIndex)
    {
        getBagData().addItemOne(pItemIndex);
    }

    //0 为钱
    public void addItem(int index, int number)
    {
        getBagData().addItem(index, number);
    }

    //得到除索引为0以外的工具数组,以便UI显示
    public ArrayList getItemList()
    {
        ArrayList lOut = new ArrayList();
        for (int i = 1; i < itemSystem.getItemTypeTable().getNum(); ++i)
        {
            int numOfTheItem = getNum(i);
            while (numOfTheItem > 0)
            {
                lOut.Add(i);
                --numOfTheItem;
            }
        }
        return lOut;
    }

    public void addMoney(int number)
    {
        //Debug.Log("addMoney");
        addItem(0, number);
        //Debug.Log(getMoneyNum());
    }

    public int getNum(int index)
    {
        return getBagData().getNum(index);
    }

    public int getMoneyNum()
    {
        return getNum(0);
    }

    public void useItemOne(int pItemIndex, GameObject pOwner)
    {
        //print("index:"+pItemIndex+",number:"+getBagData().getNum(pItemIndex));
        if (zzCreatorUtility.isHost())
            impUseItemOne(pItemIndex, pOwner);
        else
            networkView.RPC("RPCUseItemOne", RPCMode.Server, pItemIndex, networkView.viewID);
    }

    [RPC]
    public void RPCUseItemOne(int pItemIndex, NetworkViewID pOwnerID)
    {
        impUseItemOne(pItemIndex, NetworkView.Find(pOwnerID).gameObject);
    }

    protected void impUseItemOne(int pItemIndex, GameObject pOwner)
    {
        if (getNum(pItemIndex) > 0)
        {
            ItemTypeInfo lItemTypeInfo = itemSystem.getItemTypeTable()
                .getData(pItemIndex) as ItemTypeInfo;
            IitemObject item = lItemTypeInfo.getItemObject();
            if (item.canUse(pOwner))
            {
                item.use();
                getBagData().removeItemOne(pItemIndex);
            }
        }
    }

    public void useItemOne(int pItemIndex)
    {
        useItemOne(pItemIndex, owner);
    }

    public int getBagID()
    {
        return bagIndex;
    }

    void Start()
    {
        if (!owner)
            owner = gameObject;
        itemSystem = zzItemSystem.getSingleton();
        if (useExistBag)
        {
            //print("bagIndex!=-1");
            //print(bagIndex);
            bagName = getBagData().name;
        }
        else
        {
            bagIndex = itemSystem.addBag(bagName);
            //print(bagIndex);
            foreach (zzMyItemInBagInfo i in itemInBagInfo)
            {
                setItemNum(i.name, i.number);
            }
        }


        //和发射器连接 以便得到奖励
        Emitter lEmitter = owner.GetComponentInChildren<Emitter>();
        if (lEmitter)
        {
            Hashtable lInjureInfo = new Hashtable();
            lInjureInfo["bagControl"] = this;

            lEmitter.setInjureInfo(lInjureInfo);
            //lEmitter.setInjureInfo({"bagControl":this});
        }

        //Debug.Log(getBagData());

        haveBeInited = true;
        callAfterStart();
    }


}