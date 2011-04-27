using UnityEngine;

public class SingleSoldierFactoryObject : zzEditableObject
{

    //public override void applyPlayState()
    //{
    //    //工厂的设置,必须在工程执行Start前执行
    //    var lDefaultInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, soldierName);
    //    if (useDefaultProduceSetting)
    //    {
    //        factory.firstTimeOffset = lDefaultInfo.firstTimeOffset;
    //        factory.produceInterval = lDefaultInfo.produceInterval;
    //    }
    //    else
    //    {
    //        factory.firstTimeOffset = firstTimeOffset;
    //        factory.produceInterval = produceInterval;
    //    }
    //    factory.soldierToProduce = lDefaultInfo.soldierPrefab;

    //}


    [System.Serializable]
    public class SoldierFactorySetting
    {

        public Race race;

        public void setData(SoldierFactorySetting pOther)
        {
            _soldierName = pOther._soldierName;
            _useDefaultProduceSetting = pOther._useDefaultProduceSetting;
            _produceInterval = pOther._produceInterval;
            _firstTimeOffset = pOther._firstTimeOffset;
            _selected = pOther._selected;
        }

        [SerializeField]
        string _soldierName;

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
            }
        }

        [SerializeField]
        bool _useDefaultProduceSetting = true;

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
        float _produceInterval;

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
        float _firstTimeOffset;

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

        [SerializeField]
        bool _selected = false;

        [zzSerialize]
        public bool selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        public SoldierFactory addFactory(GameObject pObject)
        {
            var lDefaultInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, soldierName);
            float lFirstTimeOffset;
            float lProduceInterval;
            if (useDefaultProduceSetting)
            {
                lFirstTimeOffset = lDefaultInfo.firstTimeOffset;
                lProduceInterval = lDefaultInfo.produceInterval;
            }
            else
            {
                lFirstTimeOffset = firstTimeOffset;
                lProduceInterval = produceInterval;
            }

            return SoldierFactory.addFactory(pObject, lDefaultInfo.soldierPrefab,
                lProduceInterval, lFirstTimeOffset);
        }
    }

    public SoldierFactorySetting soldierFactorySetting
        = new SoldierFactorySetting();

    public Race race
    {
        get
        {
            return soldierFactorySetting.race;
        }
        set
        {
            soldierFactorySetting.race = value;
        }
    }
    //public GameObject buildingObject;
    public GameObject signObject;
    //public SoldierFactory factory;


    //只有在运行时才会被赋值
    //public GameObject factoryObject;改为成为父物体

    //[SerializeField]
    //string _soldierName;

    //[SerializeField]
    //bool _useDefaultProduceSetting;
    [zzSerialize]
    public bool useDefaultProduceSetting
    {
        get
        {
            return soldierFactorySetting.useDefaultProduceSetting;
        }
        set
        {
            soldierFactorySetting.useDefaultProduceSetting = value;
        }
    }

    //[SerializeField]
    //public float _produceInterval;
    [zzSerialize]
    public float produceInterval
    {
        get
        {
            return soldierFactorySetting.produceInterval;
        }
        set
        {
            soldierFactorySetting.produceInterval = value;
        }
    }

    //[SerializeField]
    //public float _firstTimeOffset;
    [zzSerialize]
    public float firstTimeOffset
    {
        get
        {
            return soldierFactorySetting.firstTimeOffset;
        }
        set
        {
            soldierFactorySetting.firstTimeOffset = value;
        }
    }


    [zzSerialize]
    public string soldierName
    {
        get 
        {
            return soldierFactorySetting.soldierName; 
        }

        set 
        {
            if (soldierFactorySetting.soldierName == name)
                return;
            soldierFactorySetting.soldierName = value;
            Destroy(signObject);
            var lTransform = transform;
            var lSoldierInfo = SoldierFactoryState.Singleton.getSoldierInfo(race, soldierName);
            signObject = (GameObject)Instantiate(lSoldierInfo.signPrefab,
                lTransform.position, lTransform.rotation);
            signObject.transform.parent = lTransform;

            //factory.soldierToProduce = lSoldierInfo.soldierPrefab;
        }
    }

    //考虑到场景创建初始阶段,物体物体不一定已创建,所以将物体探测部分放在Start中
    void LateUpdate()
    {
        if(zzCreatorUtility.isHost())
        {
            var lFactory = soldierFactorySetting.addFactory(gameObject);
            var lStronghold = SoldierFactoryState.Singleton
                .canCreate(race, transform.position,false);
            if (lStronghold)
            {
                lFactory.listener = lStronghold
                    .GetComponent<SoldierFactoryListener>().interfaceObject;
                lStronghold.soldierFactory = gameObject;
            }
        }
        enabled = false;
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

        drawProduceSetting( lFactoryObject.soldierFactorySetting);

        var lNewSelected = drawSoldierList(lFactoryObject.race, lSelectedSoldier);
        if (lNewSelected != lSelectedSoldier)
            lFactoryObject.soldierName = lNewSelected;
    }

    public static void drawProduceSetting(
        SingleSoldierFactoryObject.SoldierFactorySetting pSoldierFactorySetting)
    {
        var race = pSoldierFactorySetting.race;
        string lSelectedSoldier = pSoldierFactorySetting.soldierName;
        float lProduceInterval = pSoldierFactorySetting.produceInterval;
        float lFirstTimeOffset = pSoldierFactorySetting.firstTimeOffset;
        bool lUseDefault = pSoldierFactorySetting.useDefaultProduceSetting;
        if (lUseDefault)
        {
            var lDefaultInfo = SoldierFactoryState.Singleton
                .getSoldierInfo(race, lSelectedSoldier);
            lProduceInterval = lDefaultInfo.produceInterval;
            lFirstTimeOffset = lDefaultInfo.firstTimeOffset;
        }
        bool lNewUse;
        GUILayout.BeginVertical();
        {
            lNewUse = GUILayout.Toggle(lUseDefault, "使用默认生产设置");

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

        if (!lUseDefault)
        {
            pSoldierFactorySetting.produceInterval = lProduceInterval;
            pSoldierFactorySetting.firstTimeOffset = lFirstTimeOffset;

        }
        pSoldierFactorySetting.useDefaultProduceSetting = lNewUse;
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