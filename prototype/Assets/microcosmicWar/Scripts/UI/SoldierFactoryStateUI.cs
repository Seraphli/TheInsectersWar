
using UnityEngine;
using System.Collections;

public class SoldierFactoryStateUI:MonoBehaviour
{
    //0为未选中任何图标
    int selectedIndex = 0;
    public int itemNum = 0;
    public zzInterfaceGUI UIroot;
    protected int numOfShowItem = 6;
    public zzGUITransform[] itemListUI;
    public zzInterfaceGUI[] imgListUI;
    public zzInterfaceGUI[] selectedListUI;

    public Race race;
    public GameObject onwer;

    SoldierFactoryState soldierFactoryState;

    void Start()
    {
        soldierFactoryState = SoldierFactoryState.getSingleton();
        soldierFactoryState.setChangedCall(refreshItemShow);

        zzSceneObjectMap lSceneObjectMap = zzObjectMap.getObject("UI").GetComponent<zzSceneObjectMap>();

        if (!UIroot)
            UIroot = lSceneObjectMap.getObject("soldierModule")
                .GetComponent<zzInterfaceGUI>();

        itemListUI = new zzGUITransform[numOfShowItem];
        imgListUI = new zzInterfaceGUI[numOfShowItem];
        selectedListUI = new zzInterfaceGUI[numOfShowItem];

        zzInterfaceGUI itemList = UIroot.getSubElement("itemList");
        zzInterfaceGUI selectedList = UIroot.getSubElement("selectedList");

        for (int i = 1; i <= numOfShowItem; ++i)
        {
            itemListUI[i - 1] = (zzGUITransform)itemList.getSubElement(i.ToString());
            imgListUI[i - 1] = itemListUI[i - 1].getSubElement("pic");
            selectedListUI[i - 1] = selectedList.getSubElement(i.ToString());
        }

        refreshItemShow();
    }

    public void refreshItemShow()
    {
        int i = 0;
        foreach (var lStateInfo in soldierFactoryState.getFactoryStates(race))
        {
            if (lStateInfo.building)
                print(lStateInfo.building.name);
            if (lStateInfo.canBuild())
                imgListUI[i].setImage(lStateInfo.info.activeImage);
            else
                imgListUI[i].setImage(lStateInfo.info.inactivityImage);
            ++i;
        }
        itemNum = i;
        for (; i < numOfShowItem;++i )
        {
            imgListUI[i].setImage(null);
        }
        selecteUp();

    }

    //从1开始, 0为没有选, 比如包中为空
    public void setSelected(int pIndex)
    {
        foreach (zzInterfaceGUI i in selectedListUI)
            i.setVisible(false);
        if (pIndex > 0)
            selectedListUI[pIndex - 1].setVisible(true);

        selectedIndex = pIndex;
    }

    public int itemShowNum
    {
        get
        {
            int lOut = 0;
            foreach (var lStateInfo in soldierFactoryState.getFactoryStates(race))
            {
                if (lStateInfo.canBuild())
                    ++lOut;
            }
            return lOut;
        }
    }

    //public bool haveItem()
    //{
    //    return
    //}

    //左移
    public void selecteDown()
    {
        int lItemShowNum = itemShowNum;
        //if (lItemShowNum==1&&selectedIndex==0)
        //    setSelected(1);
        //else 
            if (lItemShowNum > 0)
        {
            var lStateInfos = soldierFactoryState.getFactoryStates(race);
            int lNowIndex = selectedIndex - 1;
            while (true)
            {
                if (lNowIndex < 1)
                    lNowIndex = itemNum;
                if (lStateInfos[lNowIndex-1].canBuild())
                    break;
                --lNowIndex;
            }
            setSelected(lNowIndex);
        }
        else
                setSelected(0);
    }

    //右移
    public void selecteUp()
    {
        int lItemShowNum = itemShowNum;
        //if (lItemShowNum == 1 && selectedIndex == 0)
        //    setSelected(1);
        //else 
            if (lItemShowNum > 0)
        {
            var lStateInfos = soldierFactoryState.getFactoryStates(race);
            int lNowIndex = selectedIndex + 1;
            while (true)
            {
                if (lNowIndex > itemNum)
                    lNowIndex = 1;
                if (lStateInfos[lNowIndex - 1].canBuild())
                    break;
                ++lNowIndex;
            }
            setSelected(lNowIndex);
        }
            else
                setSelected(0);
    }

    public void useSelected()
    {
        if (selectedIndex > 0)
        {
            int lIndex = selectedIndex - 1;
            soldierFactoryState.createFactory(race, lIndex, onwer);
        }
    }
}