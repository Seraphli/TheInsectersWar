using UnityEngine;

public class SingleSoldierFactoryObject : zzEditableObject
{

    public override void applyPlayState()
    {
        //工厂的设置,必须在工程执行Start前执行
        var lDefaultInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, _soldierName);
        if (useDefaultProduceSetting)
        {
            factory.firstTimeOffset = lDefaultInfo.firstTimeOffset;
            factory.produceInterval = lDefaultInfo.produceInterval;
        }
        else
        {
            factory.firstTimeOffset = firstTimeOffset;
            factory.produceInterval = produceInterval;
        }
        factory.soldierToProduce = lDefaultInfo.soldierPrefab;

    }


    [SerializeField]
    public class SoldierFactoryInfo
    {
        public string soldierName;

        public bool useDefaultProduceSetting = true;

        public float produceInterval;

        public float firstTimeOffset;

        public bool selected = false;
    }

    public Race race;
    //public GameObject buildingObject;
    public GameObject signObject;
    public SoldierFactory factory;

    public SoldierFactoryInfo soldierFactoryInfo = new SoldierFactoryInfo();

    //只有在运行时才会被赋值
    //public GameObject factoryObject;改为成为父物体

    [SerializeField]
    string _soldierName;

    [SerializeField]
    public bool _useDefaultProduceSetting;
    [zzSerialize]
    public bool useDefaultProduceSetting
    {
        get
        {
            return _useDefaultProduceSetting;
        }
        set
        {
            _useDefaultProduceSetting = value;
        }
    }

    [SerializeField]
    public float _produceInterval;
    [zzSerialize]
    public float produceInterval
    {
        get
        {
            return _produceInterval;
        }
        set
        {
            _produceInterval = value;
        }
    }

    [SerializeField]
    public float _firstTimeOffset;
    [zzSerialize]
    public float firstTimeOffset
    {
        get
        {
            return _firstTimeOffset;
        }
        set
        {
            _firstTimeOffset = value;
        }
    }


    [zzSerialize]
    public string soldierName
    {
        get 
        { 
            return _soldierName; 
        }

        set 
        {
            if (_soldierName == name)
                return;
            _soldierName = value;
            Destroy(signObject);
            var lTransform = transform;
            var lSoldierInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, _soldierName);
            signObject = (GameObject)Instantiate(lSoldierInfo.signPrefab,
                lTransform.position, lTransform.rotation);
            signObject.transform.parent = lTransform;

            //factory.soldierToProduce = lSoldierInfo.soldierPrefab;
        }
    }

    //考虑到场景创建初始阶段,物体物体不一定已创建,所以将物体探测部分放在Start中
    void Start()
    {

        var lStronghold = SoldierFactoryState.Singleton.canCreate(race, transform.position);
        if(lStronghold)
        {

            factory.listener = lStronghold
                .GetComponent<SoldierFactoryListener>().interfaceObject;
            lStronghold.soldierFactory = gameObject;
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

        drawProduceSetting(lFactoryObject.race,lSelectedSoldier, ref lFactoryObject._useDefaultProduceSetting,
            ref lFactoryObject._produceInterval, ref lFactoryObject._firstTimeOffset);

        var lNewSelected = drawSoldierList(lFactoryObject.race, lSelectedSoldier);
        if (lNewSelected != lSelectedSoldier)
            lFactoryObject.soldierName = lNewSelected;
    }

    public static void drawProduceSetting(Race race, string lSelectedSoldier, ref bool pUseDefault,
        ref float pProduceInterval,ref float pFirstTimeOffset)
    {
        float lProduceInterval = pProduceInterval;
        float lFirstTimeOffset = pFirstTimeOffset;
        if (pUseDefault)
        {
            var lDefaultInfo = SoldierFactoryState.Singleton
                .getSoldierInfo(race, lSelectedSoldier);
            lProduceInterval = lDefaultInfo.produceInterval;
            lFirstTimeOffset = lDefaultInfo.firstTimeOffset;
        }
        bool lNewUse;
        GUILayout.BeginVertical();
        {
            lNewUse = GUILayout.Toggle(pUseDefault,"使用默认生产设置");

            GUILayout.BeginHorizontal();
            GUILayout.Label("生产间隔时间", GUILayout.ExpandWidth(false));
            GUILayout.Label(lProduceInterval.ToString("f1"), GUILayout.Width(40f));
            GUILayout.EndHorizontal();
            lProduceInterval
                = GUILayout.HorizontalSlider(lProduceInterval, 0f, 200f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("首次时间偏移", GUILayout.ExpandWidth(false));
            GUILayout.Label(lFirstTimeOffset.ToString("f1"), GUILayout.Width(40f));
            GUILayout.EndHorizontal();
            lFirstTimeOffset
                = GUILayout.HorizontalSlider(lFirstTimeOffset, 0f, 200f);

        }
        GUILayout.EndVertical();

        if (!pUseDefault)
        {
            pProduceInterval = lProduceInterval;
            pFirstTimeOffset = lFirstTimeOffset;

        }
        pUseDefault = lNewUse;
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