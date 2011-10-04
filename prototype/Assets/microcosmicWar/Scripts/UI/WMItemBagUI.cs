using UnityEngine;
using System.Collections;

public class WMItemBagUI : MonoBehaviour
{
    public class ItemUIControl
    {
        public ItemUIControl(){}
        public ItemUIControl(zzInterfaceGUI pItem)
        {
            item = pItem;
        }
        [SerializeField]
        zzInterfaceGUI _item;
        public zzInterfaceGUI item
        {
            get { return _item; }
            set
            {
                _item = value;
                icon = value.getSubElement("icon");
                countLabel = value.getSubElement("count");
            }
        }
        public zzInterfaceGUI icon;
        public zzInterfaceGUI countLabel;
    }
    public WMItemBag itemBag;
    public zzItemBagControl bagUiControl;
    public zzInterfaceGUI UIroot;

    protected int numOfShowItem = 5;
    public ItemUIControl[] itemListUI = new ItemUIControl[]{};
    //public zzInterfaceGUI[] selectedListUI;
    //public int selectedIndex = 0;
    public int itemNum = 0;

    ////显示的工具的索引
    //public ArrayList itemIndexList = new ArrayList();

    //public bool showSelected = true;
    void Start()
    {
        itemBag.addBagInitEndReceiver(Init);
    }

    void Init()
    {
        zzSceneObjectMap lSceneObjectMap = GameScene.Singleton.playerInfo
            .UiRoot.GetComponent<zzSceneObjectMap>();

        if (!bagUiControl)
            bagUiControl = gameObject.GetComponent<zzItemBagControl>();

        if (!UIroot)
            UIroot = lSceneObjectMap.getObject("ItemInventory").GetComponent<zzInterfaceGUI>();

        itemListUI = new ItemUIControl[numOfShowItem];
        //selectedListUI = new zzInterfaceGUI[numOfShowItem];

        zzInterfaceGUI itemList = UIroot.getSubElement("itemList");
        zzInterfaceGUI selectedList = UIroot.getSubElement("selectedList");
        var lItems = itemBag.items;
        for (int i = 1; i <= numOfShowItem; ++i)
        {
            var lUI = new ItemUIControl(
                itemList.getSubElement(i.ToString()) );
            itemListUI[i - 1] = lUI;
            var lItem = lItems[i - 1];
            lItem.addChangedReceiver(() => refreshItemShow(lItem, lUI));
            //selectedListUI[i - 1] = selectedList.getSubElement(i.ToString());
        }
        //afterBagStartCall();
        //if (showSelected)
        //    setSelected(1);
        //else
        //    setSelected(0);
        refreshItemAllShow();
        //print(gameObject.name+bagControl+(bagControl==null));
        //bagControl.addCallAfterStart(afterBagStartCall);
    }

    //public void afterBagStartCall()
    //{
    //    refreshItemShow();
    //    bagUiControl.setItemChangedCall(refreshItemShow);
    //    //Debug.Log(selectedIndex);
    //    //Debug.Log(gameObject.name);
    //}

    public void Reset()
    {
        bagUiControl = gameObject.GetComponent<zzItemBagControl>();
    }

    public void refreshItemAllShow()
    {
        for (int i = 0; i < itemListUI.Length;++i )
        {
            refreshItemShow(i);
        }
    }

    public void refreshItemShow(WMItemBag.BagSpace lItem,ItemUIControl lUI)
    {
        if (lItem.isEmpty)
        {
            lUI.icon.setImage(null);
            lUI.countLabel.setText("");
        }
        else
        {
            //print(lItem.itemId);
            //print(lUI.item.name);
            //print(WMItemSystem.Singleton.getItem(lItem.itemId).name);
            lUI.icon.setImage(WMItemSystem.Singleton.getItem(lItem.itemId).image);
            lUI.countLabel.setText(lItem.count.ToString());
        }

    }

    public void refreshItemShow(int pIndex)
    {
        refreshItemShow(itemBag.items[pIndex], itemListUI[pIndex]);
    }

    //public void refreshItemShow()
    //{
    //    ArrayList lItemList = bagUiControl.getItemList();
    //    //Debug.Log(lItemList);
    //    itemIndexList = lItemList;
    //    int lItemUIIndex = 0;
    //    zzIndexTable lItemTypeTable = bagUiControl
    //                                    .getItemSystem().getItemTypeTable();

    //    foreach (int i in lItemList)
    //    {
    //        ItemTypeInfo lItemType = (ItemTypeInfo)lItemTypeTable.getData(i);
    //        itemListUI[lItemUIIndex].setImage(lItemType.getImage());
    //        itemListUI[lItemUIIndex].setVisible(true);
    //        ++lItemUIIndex;
    //    }

    //    itemNum = lItemUIIndex;

    //    //Debug.Log(lItemUIIndex);
    //    //更新选择的位置
    //    if (showSelected && lItemUIIndex < selectedIndex)
    //        setSelected(lItemUIIndex);

    //    //将剩余的图标空间清空
    //    for (; lItemUIIndex < numOfShowItem; ++lItemUIIndex)
    //        itemListUI[lItemUIIndex].setVisible(false);
    //}

    ////从1开始, 0为没有选, 比如包中为空
    //public void setSelected(int pIndex)
    //{
    //    //if(pIndex<0)
    //    //	pIndex = 0;
    //    //if(pIndex>itemNum)
    //    //	pIndex = itemNum;

    //    //if(selectedIndex == pIndex)
    //    //	return;
    //    foreach (zzInterfaceGUI i in selectedListUI)
    //        i.setVisible(false);
    //    if (pIndex > 0)
    //        selectedListUI[pIndex - 1].setVisible(true);

    //    selectedIndex = pIndex;
    //    //Debug.Log(selectedIndex);
    //}

    //public bool haveItem()
    //{
    //    //Debug.Log(selectedIndex);
    //    //Debug.Log(gameObject.name);
    //    return selectedIndex > 0;
    //}

    ////左移
    //public void selecteDown()
    //{
    //    if (haveItem())
    //    {
    //        int lNowIndex = selectedIndex - 1;
    //        if (lNowIndex < 1)
    //            lNowIndex = itemNum;
    //        setSelected(lNowIndex);
    //    }
    //}

    ////右移
    //public void selecteUp()
    //{
    //    if (haveItem())
    //    {
    //        int lNowIndex = selectedIndex + 1;
    //        if (lNowIndex > itemNum)
    //            lNowIndex = 1;
    //        setSelected(lNowIndex);
    //    }
    //}

    //public void useSelected()
    //{
    //    if (selectedIndex > 0)
    //    {
    //        int lIndex = (int)itemIndexList[selectedIndex - 1];
    //        bagControl.useItemOne(lIndex);
    //    }
    //}

    /// <summary>
    /// 从0开始
    /// </summary>
    /// <param name="index"></param>
    public void useByIndex(int index)
    {
        //itemBag.items[index].use()

        //if (index < itemIndexList.Count)
        //{
        //    int lIndex = (int)itemIndexList[index];
        //    bagUiControl.useItemOne(lIndex);
        //}
    }
}