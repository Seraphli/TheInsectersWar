using UnityEngine;

public class SingleSoldierFactoryObject : zzEditableObject
{
    public Race race;
    //public GameObject buildingObject;
    public GameObject signObject;
    public SoldierFactory factory;

    //只有在运行时才会被赋值
    //public GameObject factoryObject;改为成为父物体


    [SerializeField]
    string _soldierName;

    [SerializeField]
    bool _useDefaultProduceSetting;

    [SerializeField]
    float _produceInterval;

    [SerializeField]
    float _firstTimeOffset;

    [zzSerialize]
    public string soldierName
    {
        get 
        { 
            return _soldierName; 
        }

        set 
        { 
            _soldierName = value;
            Destroy(signObject);
            var lTransform = transform;
            var lSoldierInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, _soldierName);
            signObject = (GameObject)Instantiate(lSoldierInfo.signPrefab,
                lTransform.position, lTransform.rotation);
            signObject.transform.parent = lTransform;

            factory.soldierToProduce = lSoldierInfo.soldierPrefab;
        }
    }

    static SingleSoldierFactoryObjectGUI sPropertyGUI = new SingleSoldierFactoryObjectGUI();

    public static IPropertyGUI PropertyGUI
    {
        get
        {
            return sPropertyGUI;
        }
    }
}

public class SingleSoldierFactoryObjectGUI:IPropertyGUI
{

    public override void OnPropertyGUI(MonoBehaviour pObject)
    {
        var lFactoryObject = (SingleSoldierFactoryObject)pObject;
        var lSelectedSoldier = lFactoryObject.soldierName;
        var lNewSelected = drawSoldierList(lFactoryObject.race, lSelectedSoldier);
        if (lNewSelected != lSelectedSoldier)
            lFactoryObject.soldierName = lNewSelected;
    }

    string drawSoldierList(Race pRace,string pSelected)
    {
        //var lSoldierFactoryState = SoldierFactoryState.Singleton;
        //var lStateIndex = lSoldierFactoryState.getSoldierIndex(pRace, pSelectedpSelected);
        var lSoldierInfoList = SoldierFactorySystem.Singleton.getSoldierInfoList(pRace);
        string lOut = pSelected;
        GUILayout.BeginVertical();
        GUILayout.Label("模块生产兵种选择");
        foreach (var llSoldierInfo in lSoldierInfoList)
        {
            if (drawSoldier(llSoldierInfo, pSelected))
                lOut = llSoldierInfo.name;
        }
        GUILayout.EndVertical();
        return lOut;
    }

    bool drawSoldier(SoldierFactorySystem.SoldierInfo pInfo,string pSelected)
    {
        bool lSelected = pInfo.name == pSelected;
        GUILayout.BeginHorizontal();
        float lImageWidth = windowRect.width / 3f;
        GUILayout.Box(pInfo.image, GUILayout.Width(lImageWidth), GUILayout.Height(lImageWidth));
        GUILayout.Label(pInfo.showName);
        bool lNewSelected = GUILayout.Toggle(lSelected,"");
        GUILayout.EndHorizontal();
        if (!lSelected && lNewSelected)
            return true;
        return false;
    }

}