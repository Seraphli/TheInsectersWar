using UnityEngine;
using System.Collections.Generic;

public class WMPriceList:MonoBehaviour
{
    [System.Serializable]
    public class TypeDiscountInfo
    {
        public string typeName;
        public float discount;
    }

    [System.Serializable]
    public class PriceInfo
    {
        public int buyingPrice = 10;
        public int sellingPrice = 10;
    }

    public TypeDiscountInfo[] typeDiscountInfo;
    public PriceInfo[] priceInfo;

    public PriceInfo getPriceInfo(int pItemID)
    {
        return priceInfo[pItemID];
    }

    Dictionary<string, float> typeNameToDiscount;

    void Start()
    {
        typeNameToDiscount = new Dictionary<string, float>();
        foreach (var lInfo in typeDiscountInfo)
        {
            typeNameToDiscount[lInfo.typeName] = lInfo.discount;
        }
        var lItemSystem = WMItemSystem.Singleton;
        priceInfo = new PriceInfo[lItemSystem.items.Count];
        addItemElements(lItemSystem.itemTree, 1.0f);
    }

    void addItemElement(int pItemID, int pBuyingPrice, int pSellingPrice)
    {
        priceInfo[pItemID] = new PriceInfo()
        {
            buyingPrice = pBuyingPrice,
            sellingPrice = pSellingPrice,
        };
    }

    void addItemElements(WMItemSystem.InfoNode pNode, float pDiscount)
    {
        foreach (var lElement in pNode.elements)
        {
            addItemElement(lElement.id,
                lElement.buyingPrice * pDiscount,
                lElement.sellingPrice * pDiscount);
        }

        foreach (var lNode in pNode.nodes)
        {
            float lDiscount;
            if (typeNameToDiscount.TryGetValue(lNode.name, out lDiscount))
                addItemElements(lNode, lDiscount);
            else
                addItemElements(lNode, pDiscount);
        }
    }
}