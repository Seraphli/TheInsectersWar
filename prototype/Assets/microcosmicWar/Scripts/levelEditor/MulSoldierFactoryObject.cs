using UnityEngine;
using System.Collections.Generic;

public class MulSoldierFactoryObject : zzEditableObject
{

    public Race race;

    [SerializeField]
    SingleSoldierFactoryObject.SoldierFactorySetting[] _soldierFactorySettings;

    [zzSerialize]
    public SingleSoldierFactoryObject.SoldierFactorySetting[] soldierFactorySettings
    {
        get
        {
            return _soldierFactorySettings;
        }

        set
        {
            if(_soldierFactorySettings==null||_soldierFactorySettings.Length==0)
            {
                createInfo();
            }
            foreach (var lSettingValue in value)
            {
                foreach(var lSetting in _soldierFactorySettings)
                {
                    if (lSetting.soldierName == lSettingValue.soldierName)
                    {
                        lSetting.setData(lSettingValue);
                        break;
                    }
                }
            }
        }
    }

    static MulSoldierFactoryObjectGUI sPropertyGUI = new MulSoldierFactoryObjectGUI();

    public static IPropertyGUI PropertyGUI
    {
        get
        {
            return sPropertyGUI;
        }
    }

    public override void applyPlayState()
    {
        enabled = true;
    }

    //public string soldierFactorySettingsData
    //{
    //    get
    //    {
    //        string lOut;
    //        foreach (var lSetting in soldierFactorySettingsData)
    //        {
    //            lOut += "|soldierName:" + soldierFactorySettingsData.soldierName;
    //            lOut += ",useDefaultProduceSetting:" + soldierFactorySettingsData.useDefaultProduceSetting;
    //            lOut += ",produceInterval:" + soldierFactorySettingsData.produceInterval;
    //            lOut += ",firstTimeOffset:" + soldierFactorySettingsData.firstTimeOffset;
    //            lOut += ",selected:" + soldierFactorySettingsData.selected;
    //        }
    //        return lOut;
    //    }

    //    set
    //    {
    //        var lSettings = value.Split(new char[]{'|'});
    //    }
    //}

    void LateUpdate()
    {
        if(_soldierFactorySettings==null||_soldierFactorySettings.Length==0)
        {
            //in editor mode
            createInfo();
        }

        if(inPlaying)
        {
            var  lSoldierFactories =new List<SoldierFactory>();
            foreach (var lSetting in _soldierFactorySettings)
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

        enabled = false;
    }

    void createInfo()
    {
        var lSoldierInfoList = SoldierFactorySystem.Singleton.getSoldierInfoList(race);
        _soldierFactorySettings = new SingleSoldierFactoryObject
            .SoldierFactorySetting[lSoldierInfoList.Length];
        int i = 0;
        foreach (var lInfo in lSoldierInfoList)
        {
            var lSoldierFactoryInfo = new SingleSoldierFactoryObject.SoldierFactorySetting();
            lSoldierFactoryInfo.soldierName = lInfo.name;
            lSoldierFactoryInfo.produceInterval = lInfo.produceInterval;
            lSoldierFactoryInfo.firstTimeOffset = lInfo.firstTimeOffset;
            lSoldierFactoryInfo.race = race;
            _soldierFactorySettings[i] = lSoldierFactoryInfo;
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