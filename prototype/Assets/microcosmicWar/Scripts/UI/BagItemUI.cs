
using UnityEngine;
using System.Collections;

public class BagItemUI : MonoBehaviour
{


    public zzItemBagControl bagControl;
    public zzInterfaceGUI UIroot;

    protected int numOfShowItem = 5;
    public zzInterfaceGUI[] itemListUI;
    public zzInterfaceGUI[] selectedListUI;
    public int selectedIndex = 0;
    public int itemNum = 0;

    //显示的工具的索引
    public ArrayList itemIndexList = new ArrayList();

    public bool showSelected = true;

    void Start()
    {
        zzSceneObjectMap lSceneObjectMap = GameScene.Singleton.playerInfo
            .UiRoot.GetComponent<zzSceneObjectMap>();

        if (!bagControl)
            bagControl = gameObject.GetComponent<zzItemBagControl>();

        if (!UIroot)
            UIroot = lSceneObjectMap.getObject("ItemInventory").GetComponent<zzInterfaceGUI>();

        itemListUI = new zzInterfaceGUI[numOfShowItem];
        selectedListUI = new zzInterfaceGUI[numOfShowItem];

        zzInterfaceGUI itemList = UIroot.getSubElement("itemList");
        zzInterfaceGUI selectedList = UIroot.getSubElement("selectedList");

        for (int i = 1; i <= numOfShowItem; ++i)
        {
            itemListUI[i - 1] = itemList.getSubElement(i.ToString());
            selectedListUI[i - 1] = selectedList.getSubElement(i.ToString());
        }
        if (showSelected)
            setSelected(1);
        else
            setSelected(0);

        //print(gameObject.name+bagControl+(bagControl==null));
        bagControl.addCallAfterStart(afterBagStartCall);
    }

    public void afterBagStartCall()
    {
        refreshItemShow();
        bagControl.setItemChangedCall(refreshItemShow);
        //Debug.Log(selectedIndex);
        //Debug.Log(gameObject.name);
    }

    public void Reset()
    {
        bagControl = gameObject.GetComponent<zzItemBagControl>();
    }

    public void refreshItemShow()
    {
        ArrayList lItemList = bagControl.getItemList();
        //Debug.Log(lItemList);
        itemIndexList = lItemList;
        int lItemUIIndex = 0;
        zzIndexTable lItemTypeTable = bagControl
                                        .getItemSystem().getItemTypeTable();

        foreach (int i in lItemList)
        {
            ItemTypeInfo lItemType = (ItemTypeInfo)lItemTypeTable.getData(i);
            itemListUI[lItemUIIndex].setImage(lItemType.getImage());
            itemListUI[lItemUIIndex].setVisible(true);
            ++lItemUIIndex;
        }

        itemNum = lItemUIIndex;

        //Debug.Log(lItemUIIndex);
        //更新选择的位置
        if (showSelected && lItemUIIndex < selectedIndex)
            setSelected(lItemUIIndex);

        //将剩余的图标空间清空
        for (; lItemUIIndex < numOfShowItem; ++lItemUIIndex)
            itemListUI[lItemUIIndex].setVisible(false);
    }

    //从1开始, 0为没有选, 比如包中为空
    public void setSelected(int pIndex)
    {
        //if(pIndex<0)
        //	pIndex = 0;
        //if(pIndex>itemNum)
        //	pIndex = itemNum;

        //if(selectedIndex == pIndex)
        //	return;
        foreach (zzInterfaceGUI i in selectedListUI)
            i.setVisible(false);
        if (pIndex > 0)
            selectedListUI[pIndex - 1].setVisible(true);

        selectedIndex = pIndex;
        //Debug.Log(selectedIndex);
    }

    public bool haveItem()
    {
        //Debug.Log(selectedIndex);
        //Debug.Log(gameObject.name);
        return selectedIndex > 0;
    }

    //左移
    public void selecteDown()
    {
        if (haveItem())
        {
            int lNowIndex = selectedIndex - 1;
            if (lNowIndex < 1)
                lNowIndex = itemNum;
            setSelected(lNowIndex);
        }
    }

    //右移
    public void selecteUp()
    {
        if (haveItem())
        {
            int lNowIndex = selectedIndex + 1;
            if (lNowIndex > itemNum)
                lNowIndex = 1;
            setSelected(lNowIndex);
        }
    }

    public void useSelected()
    {
        if (selectedIndex > 0)
        {
            int lIndex =(int) itemIndexList[selectedIndex - 1];
            bagControl.useItemOne(lIndex);
        }
    }

    /// <summary>
    /// 从0开始
    /// </summary>
    /// <param name="index"></param>
    public void useByIndex(int index)
    {
        if (index < itemIndexList.Count)
        {
            int lIndex = (int)itemIndexList[index];
            bagControl.useItemOne(lIndex);
        }
    }
}