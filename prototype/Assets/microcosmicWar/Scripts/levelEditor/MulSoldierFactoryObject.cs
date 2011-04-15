using UnityEngine;
using System.Collections.Generic;

public class MulSoldierFactoryObject : zzEditableObject
{

    public Race race;

    public SingleSoldierFactoryObject.SoldierFactorySetting[] soldierFactorySettings;

    static MulSoldierFactoryObjectGUI sPropertyGUI = new MulSoldierFactoryObjectGUI();

    public static IPropertyGUI PropertyGUI
    {
        get
        {
            return sPropertyGUI;
        }
    }

    void Start()
    {
        if(soldierFactorySettings==null||soldierFactorySettings.Length==0)
        {
            //in editor mode
            createInfo();
        }

        if(inPlaying)
        {
            var  lSoldierFactories =new List<SoldierFactory>();
            foreach (var lSetting in soldierFactorySettings)
            {
                if(lSetting.selected)
                {
                    lSoldierFactories.Add(lSetting.addFactory(gameObject));
                }
            }
            var lStronghold = SoldierFactoryState.Singleton.canCreate(race, transform.position);
            if (lStronghold && !lStronghold.soldierFactory)
            {
                var lFactoryListener = lStronghold
                    .GetComponent<SoldierFactoryListener>().interfaceObject;
                foreach (var lFactory in lSoldierFactories)
                {
                    lFactory.listener = lFactoryListener;
                }
                lStronghold.soldierFactory = gameObject;
            }
        }
    }

    void createInfo()
    {
        var lSoldierInfoList = SoldierFactorySystem.Singleton.getSoldierInfoList(race);
        soldierFactorySettings = new SingleSoldierFactoryObject
            .SoldierFactorySetting[lSoldierInfoList.Length];
        int i = 0;
        foreach (var lInfo in lSoldierInfoList)
        {
            var lSoldierFactoryInfo = new SingleSoldierFactoryObject.SoldierFactorySetting();
            lSoldierFactoryInfo.soldierName = lInfo.name;
            lSoldierFactoryInfo.produceInterval = lInfo.produceInterval;
            lSoldierFactoryInfo.firstTimeOffset = lInfo.firstTimeOffset;
            lSoldierFactoryInfo.race = race;
            soldierFactorySettings[i] = lSoldierFactoryInfo;
            ++i;
        }

    }
}

public class MulSoldierFactoryObjectGUI : IPropertyGUI
{

    public override void OnPropertyGUI(MonoBehaviour pObject)
    {
        var lFactoryObject = (MulSoldierFactoryObject)pObject;
        var race = lFactoryObject.race;
        foreach (var lSettingInfo in lFactoryObject.soldierFactorySettings)
        {
            var lSoldierName = lSettingInfo.soldierName;
            bool lSelected = lSettingInfo.selected;
            var lNewSelected = drawSoldier(
                SoldierFactoryState.Singleton.getSoldierInfo(race,lSoldierName ),
                lSelected);
            if (lSelected)
                SingleSoldierFactoryObjectGUI.drawProduceSetting(lSettingInfo);
            lSettingInfo.selected = lNewSelected;
        }
    }

    bool drawSoldier(SoldierFactorySystem.SoldierInfo pInfo, bool pSelected)
    {
        GUILayout.BeginHorizontal();
        float lImageWidth = windowRect.width / 3f;
        GUILayout.Box(pInfo.image, GUILayout.Width(lImageWidth), GUILayout.Height(lImageWidth));
        GUILayout.Label(pInfo.showName);
        bool lNewSelected = GUILayout.Toggle(pSelected, "");
        GUILayout.EndHorizontal();

        return lNewSelected;
    }

}