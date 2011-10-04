using UnityEngine;
using System.Collections.Generic;

public class ShopBuilding : MonoBehaviour
{
    public WMPurse purse;
    public WMPriceList priceList;
    public WMItemBag itemBag;

    public string[] goodsType;
    public int maxGoodsCount = 8;
    public int[] goodsId = new int[0];
    public override bool operateThis(ObjectOperatives pUser)
    {
        var lHero = pUser.GetComponent<Hero>();
        if (!lHero)
            return false;
        purse = lHero.purse;
        priceList = lHero.priceList;
        itemBag = lHero.itemBag;

        //创建商品列表
        {
            typeIncluded = new HashSet<string>();
            var lItemSystem = WMItemSystem.Singleton;
            List<int> lGoodsId = new List<int>();
            foreach (var lGoodsType in goodsId)
            {
                var lTypeTree = lItemSystem.getTypeTree(lNode);
                if (lTypeTree != null
                    && !typeIncluded.Contains(lGoodsType))
                {
                    typeIncluded.Add(lGoodsType);
                    lGoodsId += createGoodsId(lTypeTree);
                }
            }
            if (lGoodsId.Count > maxGoodsCount)
                Debug.LogError("too much goods");
            goodsId = lGoodsId.GetRange(0, Mathf.Min(lGoodsId.Count, maxGoodsCount)).ToArray();
        }
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
                lOut += createGoodsId(lNode);
            }
        }
        return lOut;
    }
}