using UnityEngine;

public class MulSoldierFactoryObject : zzEditableObject
{
    [SerializeField]
    public class SoldierFactoryInfo
    {
        [SerializeField]
        public string soldierName;

        [SerializeField]
        public bool useDefaultProduceSetting = true;

        [SerializeField]
        public float produceInterval;

        [SerializeField]
        public float firstTimeOffset;

        public bool selected = false;
    }


    public Race race;

    public SoldierFactoryInfo[] soldierFactoryInfos;

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
        if(soldierFactoryInfos==null||soldierFactoryInfos.Length==0)
        {
            //in editor mode
            createInfo();
        }

        if(inPlaying)
        {

        }
    }

    void createInfo()
    {
        var lSoldierInfoList = SoldierFactorySystem.Singleton.getSoldierInfoList(race);
        soldierFactoryInfos = new SoldierFactoryInfo[lSoldierInfoList.Length];
        int i = 0;
        foreach (var lInfo in soldierFactoryInfos)
        {
            var lSoldierFactoryInfo = new SoldierFactoryInfo();
            lSoldierFactoryInfo.soldierName = lInfo.soldierName;
            lSoldierFactoryInfo.produceInterval = lInfo.produceInterval;
            lSoldierFactoryInfo.firstTimeOffset = lInfo.firstTimeOffset;
            soldierFactoryInfos[i] = lSoldierFactoryInfo;
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
        foreach (var lSettingInfo in lFactoryObject.soldierFactoryInfos)
        {
            var lSoldierName = lSettingInfo.soldierName;
            bool lSelected = lSettingInfo.selected;
            var lNewSelected = drawSoldier(
                SoldierFactoryState.Singleton.getSoldierInfo(race,lSoldierName ),
                lSelected);
            if (lSelected)
                SingleSoldierFactoryObjectGUI.drawProduceSetting(race,
                    lSoldierName, ref lSettingInfo.useDefaultProduceSetting,
                    ref lSettingInfo.produceInterval, ref lSettingInfo.firstTimeOffset);
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