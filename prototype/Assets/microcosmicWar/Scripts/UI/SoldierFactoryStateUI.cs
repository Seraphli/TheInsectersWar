
using UnityEngine;
using System.Collections;

public class SoldierFactoryStateUI:MonoBehaviour
{
    //0为未选中任何图标
    [SerializeField]
    int selectedIndex = 0;
    public int itemNum = 0;
    public zzInterfaceGUI UIroot;
    protected int numOfShowItem = 6;
    public zzGUITransform[] itemListUI;
    public zzInterfaceGUI[] imgListUI;
    public zzInterfaceGUI[] selectedListUI;
    public zzGUIAniToTargetScale[] animationList;

    float minIconSize;
    float maxIconSize;

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

        zzValueMap lValueMap = UIroot.GetComponent<zzValueMap>();
        minIconSize = float.Parse(lValueMap.getValue("minIconSize"));
        maxIconSize = float.Parse(lValueMap.getValue("maxIconSize"));
        float lIconScaleSpeed = float.Parse(lValueMap.getValue("iconScaleSpeed"));

        itemListUI = new zzGUITransform[numOfShowItem];
        imgListUI = new zzInterfaceGUI[numOfShowItem];
        selectedListUI = new zzInterfaceGUI[numOfShowItem];
        animationList = new zzGUIAniToTargetScale[numOfShowItem];

        zzInterfaceGUI itemList = UIroot.getSubElement("itemList");
        zzInterfaceGUI selectedList = UIroot.getSubElement("selectedList");

        for (int i = 1; i <= numOfShowItem; ++i)
        {
            zzGUITransform lGUITransform = (zzGUITransform)itemList.getSubElement(i.ToString());
            itemListUI[i - 1] = lGUITransform;
            imgListUI[i - 1] = itemListUI[i - 1].getSubElement("pic");
            selectedListUI[i - 1] = selectedList.getSubElement(i.ToString());
            zzGUIAniToTargetScale lGUIAniToTargetScale = lGUITransform.GetComponent<zzGUIAniToTargetScale>();
            animationList[i - 1] = lGUIAniToTargetScale;
            lGUIAniToTargetScale.enabled = false;
            lGUIAniToTargetScale.speed = lIconScaleSpeed;
        }

        refreshItemShow();
    }

    public void refreshItemShow()
    {
        int i = 0;
        foreach (var lStateInfo in soldierFactoryState.getFactoryStates(race))
        {
            //if (lStateInfo.building)
            //    print(lStateInfo.building.name);
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
        int i = 0;
        foreach (zzInterfaceGUI lSelectedUI in selectedListUI)
        {
            lSelectedUI.setVisible(false);
            animationList[i].scaleToTarget(minIconSize);
            ++i;
        }
        if (pIndex > 0)
        {
            selectedListUI[pIndex - 1].setVisible(true);
            animationList[pIndex - 1].scaleToTarget(maxIconSize);
        }

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
            soldierFactoryState.tryCreateFactory(race, lIndex, onwer);
        }
    }
}