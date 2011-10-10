using UnityEngine;
using System.Collections.Generic;

public class ShopBuilding : OperateObject
{
    public WMPurse purse;
    public WMPriceList priceList;
    public WMItemBag itemBag;

    public string[] goodsType;
    public int maxGoodsCount = 8;
    public int[] goodsId = new int[0];
    public ShopUI shopUI;

    public int goodsCount
    {
        get { return goodsId.Length; }
    }

    System.Action<string> errorEvent;

    public void addErrorReceiver(System.Action<string> pReceiver)
    {
        errorEvent += pReceiver;
    }

    static void nullErrorReceiver(string p){}

    void Start()
    {
        //if (errorEvent == null)
        //    errorEvent = nullErrorReceiver;
        if (errorEvent == null)
            errorEvent = (x) => Debug.Log(x);
        shopUI = GameObject.FindWithTag("ShopUI").GetComponent<ShopUI>();

        //创建商品列表
        typeIncluded = new HashSet<string>();
        var lItemSystem = WMItemSystem.Singleton;
        List<int> lGoodsId = new List<int>();
        foreach (var lGoodsType in goodsType)
        {
            var lTypeTree = lItemSystem.getTypeTree(lGoodsType);
            if (lTypeTree != null
                && !typeIncluded.Contains(lGoodsType))
            {
                typeIncluded.Add(lGoodsType);
                lGoodsId.AddRange(createGoodsId(lTypeTree));
            }
        }
        if (lGoodsId.Count > maxGoodsCount)
            Debug.LogError("too much goods");
        goodsId = lGoodsId.GetRange(0, Mathf.Min(lGoodsId.Count, maxGoodsCount)).ToArray();
    }

    public Hero testObject;

    [ContextMenu("test")]
    public void test()
    {
        operateThis(testObject);
    }

    public void operateThis(Hero lHero)
    {
        purse = lHero.purse;
        originalMoney = purse.number;
        priceList = lHero.priceList;
        itemBag = lHero.itemBag;
        initGUI();

    }

    public override bool operateThis(ObjectOperatives pUser)
    {
        var lHero = pUser.GetComponent<Hero>();
        if (!lHero)
            return false;
        operateThis(lHero);
        return true;
    }

    public HashSet<string> typeIncluded;

    public List<int> createGoodsId(WMItemSystem.InfoNode pNode)
    {
        List<int> lOut = new List<int>();
        foreach (var lElement in pNode.elements)
        {
            lOut.Add(lElement.id);
        }

        foreach (var lNode in pNode.nodes)
        {
            if(!typeIncluded.Contains(lNode.name))
            {
                typeIncluded.Add(lNode.name);
                lOut.AddRange(createGoodsId(lNode));
            }
        }
        return lOut;
    }


    [System.Serializable]
    public class BagItemInfo
    {
        public int id;
        public bool isEmpty
        {
            get { return id == 0; }
        }
        public int count;

        public void clear()
        {
            id = 0;
        }
    }

    public BagItemInfo[] originalBagItemInfo;
    public BagItemInfo[] newBagItemInfo;

    public int[] goodsListToBagItem;
    //public int[] goodsListToOldBagItem;
    //todo:delete
    public int[] bagItemToGoodsList;

    void initBagItem()
    {
        var lItemUIControls = shopUI.equipmentItemUIControl;
        var lBagItems = itemBag.items;
        var lBagItemInfo = new BagItemInfo[lBagItems.Length];
        var lItemSystem = WMItemSystem.Singleton;
        for (int i = 0; i < lBagItems.Length; ++i)
        {
            var lUiControl = lItemUIControls[i];
            var lBagItem = lBagItems[i];
            var lItemInfo = lItemSystem.getItem(lBagItem.itemId);
            if (!lBagItem.isEmpty)
            {
                lUiControl.visible = true;
                lBagItemInfo[i] = new BagItemInfo()
                {
                    id = lBagItem.itemId,
                    count = lBagItem.count,
                };
                lUiControl.count = lBagItem.count;
                lUiControl.icon = lItemInfo.image;
                lUiControl.price = lItemInfo.sellingPrice;
                lUiControl.changedCount = 0;
            }
            else
            {
                lBagItemInfo[i] = new BagItemInfo();
            }
        }
        originalBagItemInfo = lBagItemInfo;
        //newBagItemInfo = new BagItemInfo[lBagItemInfo.Length];
        //for (int i = 0; i < lBagItemInfo.Length;++i )
        //{
        //    newBagItemInfo[i] = new BagItemInfo()
        //    {
        //        id = lBagItemInfo[i].id,
        //        count = lBagItemInfo[i].count,
        //    };
        //}
        newBagItemInfo = copy(lBagItemInfo);
    }

    public int _selected;

    public int selected
    {
        get { return _selected;}
        set
        {
            _selected = value;
            shopUI.panAngle = getAngle(value);
        }
    }

    public int originalMoney = 0;
    public int cost = 0;

    public int money
    {
        get { return originalMoney - cost; }
    }

    //有空位则返回空位索引,否则返回-1
    int haveEmpty()
    {
        for (int i = 0; i < newBagItemInfo.Length;++i )
        {
            if (newBagItemInfo[i].isEmpty)
                return i;
        }
        return -1;
    }


    //int isMaxCount(int pGoodsIndex)
    //{
    //    var lGoodsID = goodsId[pIndex];
    //    var lInfo = WMItemSystem.Singleton.getItem(lGoodsID);
    //    if(goodsListToBagItem[pGoodsIndex]!=-1)
    //    {
    //        if (newBagItemInfo[goodsListToBagItem[pGoodsIndex]].count >= lInfo.maxCount)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    void swap<T>(T[] pArray, int pIndex1, int pIndex2)
    {
        //print("pArray.Length:" + pArray.Length + " pIndex1:" + pIndex1);
        var lTemp = pArray[pIndex1];
        pArray[pIndex1] = pArray[pIndex2];
        pArray[pIndex2] = lTemp;
    }

    public void swapBagItem(int pBagIndex1,int pBagIndex2)
    {
        //print("pBagIndex1:" + pBagIndex1 + " pBagIndex2:" + pBagIndex2);
        swap(newBagItemInfo, pBagIndex1, pBagIndex2);
        //var lGoodsIndex1 = bagItemToGoodsList[pBagIndex1];
        //var lGoodsIndex2 = bagItemToGoodsList[pBagIndex2];
        //print(goodsListToBagItem.Length);
        //swap(goodsListToBagItem, lGoodsIndex1, lGoodsIndex2);
        //swap(bagItemToGoodsList, pBagIndex1, pBagIndex2);
    }

    [ContextMenu("buySeleted")]
    public void buySeleted()
    {
        buy(_selected);
    }

    [ContextMenu("sellSeleted")]
    public void sellSeleted()
    {
        if (goodsListToBagItem[_selected] >= 0)
            sell(goodsListToBagItem[_selected]);
    }

    public int sellBagItemIndex;

    [ContextMenu("sellBagItemSeleted")]
    public void sellBagItemSeleted()
    {
        sell(sellBagItemIndex);
    }

    public void sell(int pBagItemIndex)
    {
        var lBagItem = newBagItemInfo[pBagItemIndex];
        if (lBagItem.isEmpty)
        {
            return;
        }
        lBagItem.count -= 1;
        if(lBagItem.count==0)
        {
            lBagItem.clear();
            goodsListToBagItem[bagItemToGoodsList[pBagItemIndex]] = -1;
            bagItemToGoodsList[pBagItemIndex] = -1;
        }
        cost = calculateCost(originalBagItemInfo, newBagItemInfo);
        updateGUI();
    }

    public void buy(int pGoodsIndex)
    {
        var lGoodsID = goodsId[pGoodsIndex];
        var lInfo = WMItemSystem.Singleton.getItem(lGoodsID);

        var lPrice = priceList.getPriceInfo(lGoodsID);
        int lNeedMoney;
        if (money < lPrice.sellingPrice)
        {
            errorEvent("钱不够");
            return;
        }
        int lEmptyPlace = haveEmpty();
        //检查空位
        if (lEmptyPlace==-1)
        {
            errorEvent("物品包已满,请先卖出物品后再购买");
            return;
        }

        var lBagItemIndex = goodsListToBagItem[pGoodsIndex];
        if(lBagItemIndex==-1)
        {
            //包中无此物品
            var lOriPos = System.Array.FindIndex(originalBagItemInfo, (x) => x.id == lGoodsID);
            if(lOriPos<0)
            {
                //在原来的包中也没有此物体
                lBagItemIndex = lEmptyPlace;
            }
            else
            {
                //在原来的包中,有此物体
                var lNewBagItemInfo = copy(newBagItemInfo);
                swap(lNewBagItemInfo, lOriPos, lEmptyPlace);
    //public void swapBagItem(int pBagIndex1,int lEmptyPlace)
    //{
    //    swap(newBagItemInfo, pBagIndex1, pBagIndex2);
    //}
                lBagItemIndex = lOriPos;
                lNewBagItemInfo[lBagItemIndex].id = lInfo.id;
                lNewBagItemInfo[lBagItemIndex].count += 1;
                var lNewCost = calculateCost(originalBagItemInfo, lNewBagItemInfo);
                if (lNewCost > originalMoney)
                {
                    errorEvent("钱不够");
                    return;
                }
                swapBagItem(lOriPos, lEmptyPlace);

            }
            bagItemToGoodsList[lBagItemIndex] = pGoodsIndex;
            goodsListToBagItem[pGoodsIndex] = lBagItemIndex;
            newBagItemInfo[lBagItemIndex].id = lInfo.id;
        }
        else
        {
            var lNewBagItemInfo = copy(newBagItemInfo);
            lNewBagItemInfo[lBagItemIndex].count += 1;
            var lNewCost = calculateCost(originalBagItemInfo, lNewBagItemInfo);
            if (lNewCost > originalMoney)
            {
                errorEvent("钱不够");
                return;
            }
            //包中已有此物品
            if (newBagItemInfo[lBagItemIndex].count >= lInfo.maxCount)
            {
                errorEvent("此物品已达最大数量");
                return;
            }
        }
        newBagItemInfo[lBagItemIndex].count += 1;
        cost = calculateCost(originalBagItemInfo,newBagItemInfo);
        updateGUI();
    }

    static BagItemInfo[] copy(BagItemInfo[] pInfo)
    {
        var lOut = new BagItemInfo[pInfo.Length];
        for (int i = 0; i < pInfo.Length; ++i)
        {
            lOut[i] = new BagItemInfo()
            {
                id = pInfo[i].id,
                count = pInfo[i].count,
            };
        }
        return lOut;

    }

    int calculateCost(BagItemInfo[] pOldInfo,BagItemInfo[] pNewInfo)
    {
        var lItemSystem =WMItemSystem.Singleton;
        int lCost = 0;
        for (int i = 0; i < pOldInfo.Length;++i )
        {
            var lOldInfo = pOldInfo[i];
            var lNewInfo = pNewInfo[i];
            //var lOldItemInfo = lItemSystem.getItem(lOldInfo.id);
            var lOldInfoPrice = priceList.getPriceInfo(lOldInfo.id);
            var lNewInfoPrice = priceList.getPriceInfo(lNewInfo.id);
            if (lOldInfo.id == lNewInfo.id && !lOldInfo.isEmpty)
            {
                var lChanged = lNewInfo.count - lOldInfo.count;
                if (lChanged > 0)
                    lCost += lOldInfoPrice.buyingPrice * lChanged;
                else if (lChanged < 0)
                    lCost -= lOldInfoPrice.sellingPrice * lChanged;
            }
            else
            {
                if (!lOldInfo.isEmpty)
                    lCost -= lOldInfoPrice.sellingPrice;
                if (!lNewInfo.isEmpty)
                    lCost += lNewInfoPrice.buyingPrice;
            }
        }
        return lCost;
    }

    float getAngle(int pSelect)
    {
        return -pSelect * (360f / shopUI.flowerItemCount);
    }

    [ContextMenu("forwardSelect")]
    public void forwardSelect()
    {
        _selected = (_selected+1)%goodsCount;
        shopUI.rotateToTarget(getAngle(_selected));
    }

    [ContextMenu("backwardSelect")]
    public void backwardSelect()
    {
        _selected = (_selected - 1) % goodsCount;
        if (_selected<0)
            _selected += goodsCount;
        shopUI.rotateToTarget(getAngle(_selected));
    }

    void updateGUI()
    {
        shopUI.clear();
        var lGoodItemUIControls = shopUI.flowerItemUIControl;
        var lItemSystem = WMItemSystem.Singleton;

        //花瓣
        for (int i = 0; i < goodsId.Length; ++i)
        {
            var lItemID = goodsId[i];
            var lItemInfo = lItemSystem.getItem(lItemID);
            var lUiControl = lGoodItemUIControls[i];
            lUiControl.visible = true;
            lUiControl.changedCount = 0;
            var lBagItemIndex = goodsListToBagItem[i];
            if (lBagItemIndex != -1)
            {
                var lOriginalBagItem = originalBagItemInfo[lBagItemIndex];
                var lNewBagItem = newBagItemInfo[lBagItemIndex];
                lUiControl.count = lNewBagItem.count;
                //if (!lOriginalBagItem.isEmpty)
                //{
                //    if (lOriginalBagItem.id == lNewBagItem.id)
                //    {
                //        lUiControl.changedCount = lNewBagItem.count - lOriginalBagItem.count;
                //    }
                //    else
                //    {
                //        lUiControl.changedCount = -lOriginalBagItem.count;
                //    }
                //}
                //else if (!lNewBagItem.isEmpty)
                //{
                //    lUiControl.changedCount = lNewBagItem.count;
                //}
            }
            else
            {
                lUiControl.count = 0;
                lUiControl.changedCount = 0;
            }
        }

        //包

        var lItemUIControls = shopUI.equipmentItemUIControl;
        var lBagItems = itemBag.items;
        for (int i = 0; i < newBagItemInfo.Length; ++i)
        {
            var lUiControl = lItemUIControls[i];
            var lNewBagItem = newBagItemInfo[i];
            if (!lNewBagItem.isEmpty)
            {
                lUiControl.visible = true;
                var lItemInfo = lItemSystem.getItem(lNewBagItem.id);
                var lOriginalBagItem = originalBagItemInfo[i];
                lUiControl.count = lNewBagItem.count;
                lUiControl.icon = lItemInfo.image;
                lUiControl.price = lItemInfo.sellingPrice;
                if (lNewBagItem.id == lOriginalBagItem.id)
                    lUiControl.changedCount = lNewBagItem.count - lOriginalBagItem.count;
                else
                    lUiControl.changedCount = lNewBagItem.count;
            }
        }
    }

    void initGUI()
    {
        var lItemSystem = WMItemSystem.Singleton;
        shopUI.clear();
        var lGoodItemUIControls = shopUI.flowerItemUIControl;
        goodsListToBagItem = new int[goodsId.Length];
        bagItemToGoodsList = new int[goodsId.Length];
        for (int i = 0; i < goodsId.Length;++i )
        {
            var lItemID = goodsId[i];
            var lItemInfo = lItemSystem.getItem(lItemID);
            var lUiControl = lGoodItemUIControls[i];
            lUiControl.visible = true;
            lUiControl.price = priceList.getPriceInfo(lItemID).buyingPrice;
            lUiControl.icon = lItemInfo.image;
            lUiControl.changedCount = 0;
            var lBagCell =itemBag.getByItemID(lItemID);
            if (lBagCell != null)
            {
                lUiControl.count = lBagCell.count;
                goodsListToBagItem[i] = lBagCell.bagItemIndex;
                //bagItemToGoodsList[lBagCell.bagItemIndex] = i;
            }
            else
            {
                goodsListToBagItem[i] = -1;
                //bagItemToGoodsList[lBagCell.bagItemIndex] = -1;
                lUiControl.count = 0;
            }
        }
        //goodsListToOldBagItem = new int[goodsListToBagItem.Length];
        //goodsListToOldBagItem = System.Array.Copy(goodsListToBagItem,
        //    goodsListToOldBagItem, goodsListToBagItem.Length);
        initBagItem();
        selected = goodsCount / 2;
    }
}