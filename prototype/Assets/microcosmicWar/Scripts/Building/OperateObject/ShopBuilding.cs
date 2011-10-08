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

    void Start()
    {
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

    public override bool operateThis(ObjectOperatives pUser)
    {
        var lHero = pUser.GetComponent<Hero>();
        if (!lHero)
            return false;
        purse = lHero.purse;
        priceList = lHero.priceList;
        itemBag = lHero.itemBag;

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
        public int count;
    }

    public BagItemInfo[] originalBagItemInfo;
    public BagItemInfo[] newBagItemInfo;

    public int[] goodsListToBagItem;
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
                lUiControl.item.visible = true;
            }
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
        originalBagItemInfo = lBagItemInfo;
        newBagItemInfo = lBagItemInfo;
    }

    void initGUI()
    {
        var lItemSystem = WMItemSystem.Singleton;
        shopUI.clear();
        var lGoodItemUIControls = shopUI.flowerItemUIControl;
        for (int i = 0; i < goodsId.Length;++i )
        {
            var lItemID = goodsId[i];
            var lItemInfo = lItemSystem.getItem(lItemID);
            var lUiControl = lGoodItemUIControls[i];
            lUiControl.item.visible = true;
            lUiControl.price = priceList.getPriceInfo(lItemID).sellingPrice;
            lUiControl.icon = lItemInfo.image;
            lUiControl.changedCount = 0;
            var lBagCell =itemBag.getByItemID(lItemID);
            if (lBagCell != null)
            {
                lUiControl.count = lBagCell.count;
                goodsListToBagItem[i] = lBagCell.bagItemIndex;
                bagItemToGoodsList[lBagCell.bagItemIndex] = i;
            }
            else
                lUiControl.count = 0;
        }
        initBagItem();
    }
}