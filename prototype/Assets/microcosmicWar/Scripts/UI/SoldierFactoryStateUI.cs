
using UnityEngine;
using System.Collections;

public class SoldierFactoryStateUI:MonoBehaviour
{
    //0为未选中任何图标
    [SerializeField]
    int selectedIndex = 0;
    public zzInterfaceGUI UIroot;
    protected int numOfShowItem = 6;
    public zzGUITransform[] itemListUI;
    public zzInterfaceGUI[] imgListUI;
    public zzInterfaceGUI[] selectedListUI;
    public zzGUIAniToTargetScale[] animationList;

    [System.Serializable]
    public class SoldierFactoryUIControl
    {
        public float maxUiSize;
        public float minUiSize;
        public zzInterfaceGUI item;
        public zzInterfaceGUI soldierIcon;
        public zzInterfaceGUI unlockCostLabel;
        public zzInterfaceGUI costRoot;
        public zzGUIAniToTargetScale scaleAnimation;
        public zzInterfaceGUI selectedUI;
        public bool selected
        {
            set
            {
                if(value)
                {
                    selectedUI.visible = true;
                    scaleAnimation.scaleToTarget(maxUiSize);

                }
                else
                {
                    selectedUI.visible = false;
                    scaleAnimation.scaleToTarget(minUiSize);
                }
            }
        }
    }

    public SoldierFactoryUIControl[] soldierFactoryUI = new SoldierFactoryUIControl[] { };

    float minIconSize;
    float maxIconSize;

    public Race race;
    public GameObject owner;

    //SoldierFactoryState soldierFactoryState;

    public PlayerSoldierFactoryState playerSoldierFactoryState;

    void Start()
    {
        //soldierFactoryState = SoldierFactoryState.getSingleton();
        //soldierFactoryState.setChangedCall(refreshItemShow);

        zzSceneObjectMap lSceneObjectMap = GameScene.Singleton.playerInfo
            .UiRoot.GetComponent<zzSceneObjectMap>();

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
        soldierFactoryUI = new SoldierFactoryUIControl[numOfShowItem];
        zzInterfaceGUI lItemList = UIroot.getSubElement("itemList");
        zzInterfaceGUI lSelectedList = UIroot.getSubElement("selectedList");

        for (int i = 1; i <= numOfShowItem; ++i)
        {
            zzGUITransform lGUITransform = (zzGUITransform)lItemList.getSubElement(i.ToString());
            //itemListUI[i - 1] = lGUITransform;
            //imgListUI[i - 1] = itemListUI[i - 1].getSubElement("pic");
            //selectedListUI[i - 1] = lSelectedList.getSubElement(i.ToString());
            //animationList[i - 1] = lGUIAniToTargetScale;
            //初始中 先全部隐藏
            lGUITransform.visible = false;
            var lGUIAniToTargetScale = lGUITransform.GetComponent<zzGUIAniToTargetScale>();
            lGUIAniToTargetScale.enabled = false;
            lGUIAniToTargetScale.speed = lIconScaleSpeed;

            var lUIControl = new SoldierFactoryUIControl();
            soldierFactoryUI[i - 1] = lUIControl;
            lUIControl.item = lGUITransform;
            lUIControl.soldierIcon = lGUITransform.getSubElement("pic");
            lUIControl.scaleAnimation = lGUIAniToTargetScale;
            lUIControl.costRoot = lGUITransform.getSubElement("cost");
            lUIControl.unlockCostLabel = lUIControl.costRoot.getSubElement("costLabel");
            lUIControl.selectedUI = lSelectedList.getSubElement(i.ToString());
            lUIControl.maxUiSize = maxIconSize;
            lUIControl.minUiSize = minIconSize;
        }
        showImage();
        refreshItemShow();
        setSelected(1);
    }

    public void showImage()
    {
        //foreach (var lImgUI in imgListUI)
        //{
        //    lImgUI.setImage(null);
        //}

        //int i = 0;
        //foreach (var lStateInfo in soldierFactoryState.getFactoryStates(race))
        //{
        //    imgListUI[i].setImage(lStateInfo.info.activeImage);
        //    ++i;
        //}

        var lSoldierFactorySystem = SoldierFactorySystem.Singleton;
        var lRace = playerSoldierFactoryState.race;
        int i = 0;
        foreach (var lSoldierFactory in playerSoldierFactoryState.soldierFactory)
        {
            var lSoldierInfo = lSoldierFactorySystem.getSoldierInfo(lRace, lSoldierFactory.name);
            var lUIControl = soldierFactoryUI[i];
            lUIControl.item.visible = true;
            if(lSoldierFactory.locked)
            {
                lUIControl.costRoot.visible = true;
                lUIControl.unlockCostLabel.setText(lSoldierFactory.unlockCost.ToString());
                lUIControl.soldierIcon.setImage(lSoldierInfo.inactivityImage);
            }
            else
            {
                lUIControl.costRoot.visible = false;
                lUIControl.soldierIcon.setImage(lSoldierInfo.activeImage);
            }
            ++i;
        }

    }

    public void refreshItemShow()
    {
        //暂无内容

    }

    //从1开始, 0为没有选, 比如包中为空
    public void setSelected(int pIndex)
    {
        int i = 0;
        //foreach (zzInterfaceGUI lSelectedUI in selectedListUI)
        //{
        //    lSelectedUI.setVisible(false);
        //    animationList[i].scaleToTarget(minIconSize);
        //    ++i;
        //}
        //if (pIndex > 0)
        //{
        //    selectedListUI[pIndex - 1].setVisible(true);
        //    animationList[pIndex - 1].scaleToTarget(maxIconSize);
        //}

        foreach (var lUIControl in soldierFactoryUI)
        {
            lUIControl.selected = false;
        }
        if (pIndex > 0)
            soldierFactoryUI[pIndex - 1].selected = true;
        selectedIndex = pIndex;
    }

    public int itemNum
    {
        get
        {
            return playerSoldierFactoryState.soldierFactory.Length;
        }
    }

    //public int itemShowNum
    //{
    //    get
    //    {
    //        return soldierFactoryState.getFactoryStates(race).Count;
    //        //int lOut = 0;
    //        //foreach (var lStateInfo in soldierFactoryState.getFactoryStates(race))
    //        //{
    //        //    if (lStateInfo.canBuild())
    //        //        ++lOut;
    //        //}
    //        //return lOut;
    //    }
    //}


    //左移
    public void selecteDown()
    {
        //int lItemShowNum = itemShowNum;
        //if (lItemShowNum==1&&selectedIndex==0)
        //    setSelected(1);
        //else 
            //if (lItemShowNum > 0)
        {
            //var lStateInfos = soldierFactoryState.getFactoryStates(race);
            int lNowIndex = selectedIndex - 1;
            if (lNowIndex < 1)
                lNowIndex = itemNum;
            //while (true)
            //{
            //    if (lStateInfos[lNowIndex-1].canBuild())
            //        break;
            //    --lNowIndex;
            //}
            setSelected(lNowIndex);
        }
        //else
        //        setSelected(0);
    }

    //右移
    public void selecteUp()
    {
        //int lItemShowNum = itemShowNum;
        ////if (lItemShowNum == 1 && selectedIndex == 0)
        ////    setSelected(1);
        ////else 
        //    if (lItemShowNum > 0)
        //{
        //    var lStateInfos = soldierFactoryState.getFactoryStates(race);
            int lNowIndex = selectedIndex + 1;
            if (lNowIndex > itemNum)
                lNowIndex = 1;
        //    while (true)
        //    {
        //        if (lStateInfos[lNowIndex - 1].canBuild())
        //            break;
        //        ++lNowIndex;
        //    }
            setSelected(lNowIndex);
        //}
        //    else
        //        setSelected(0);
    }

    public void useSelected()
    {
        if (selectedIndex > 0)
        {
            //int lIndex = selectedIndex - 1;
            //soldierFactoryState.tryCreateFactory(race, lIndex, owner);
            int lIndex = selectedIndex - 1;
            var lSoldierFactory = playerSoldierFactoryState.soldierFactory[lIndex];
            if(lSoldierFactory.locked)
            {
                if (playerSoldierFactoryState.tryUnlockSoldier(lIndex))
                {
                    var lSoldierInfo = SoldierFactorySystem.Singleton.getSoldierInfo(race, lSoldierFactory.name);
                    soldierFactoryUI[lIndex].soldierIcon.setImage(lSoldierInfo.activeImage);
                    soldierFactoryUI[lIndex].costRoot.visible = false;
                }
                else
                    return;
            }
            SoldierFactoryState.Singleton.tryCreateFactory(race, lSoldierFactory.name, owner);
        }
    }
}