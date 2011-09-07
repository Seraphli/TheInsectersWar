
using UnityEngine;
using System.Collections;

public class HeroSpawn : MonoBehaviour
{
    [SerializeField]
    string _playerName;

    [SerializeField]
    Color _playerNameColor;

    public string nameLabelObjectName = "nameLabel";

    public string playerName
    {
        get { return _playerName; }
        set
        {
            _playerName = value;
            if (hero)
                setPlayerName(_playerName);
        }
    }

    public Color playerNameColor
    {
        get { return _playerNameColor; }
        set
        {
            _playerNameColor = value;
            if (hero)
                setPlayerNameColor(_playerNameColor);
        }
    }

    public void setPlayerName(string pName)
    {
        hero.transform.FindChild(nameLabelObjectName)
            .GetComponent<TextMesh>().text = pName;
    }

    public void setPlayerNameColor(Color pColor)
    {
        hero.transform.FindChild(nameLabelObjectName)
            .renderer.material.color = pColor;
    }

    public GameObject heroPrefab;
    public GameObject netSysnPrefab;

    public NetworkPlayer _owner;
    public bool autoCreatePlayer = false;
    public int itemBagID;

    public GameObject hero;
    public zzInterfaceGUI rebirthClockUI;
    [SerializeField]
    GameObject _SystemObject;
    public GameObject SystemObject
    {
        get
        {
            if (!_SystemObject)
                _SystemObject = zzObjectMap.getObject("system");
            return _SystemObject;
        }
    }

    //是否创建过
    protected bool haveFirstCreate = false;

    //重生所需的时间
    public int rebirthTime = 10;
    protected zzTimer rebirthClockTimer;
    //重生剩余的时间
    protected int rebirthTimeLeave = 0;
    protected zzTimer rebirthTimer;
    zzSceneObjectMap mUIObjectMap;
    public zzGUIProgressBarBase bloodBar;

    void updateBloodBar(Life life)
    {
        bloodBar.rate = life.rate;
    }

    zzGUIProgressBarBase skillBar;

    void getUI()
    {
        var lUiRoot = GameScene.Singleton.playerInfo.UiRoot;

        lUiRoot.GetComponent<zzInterfaceGUI>().setVisible(true);

        mUIObjectMap = lUiRoot.GetComponent<zzSceneObjectMap>();

        bloodBar = mUIObjectMap.getObject("bloodBar").GetComponent<zzGUIProgressBarBase>();

        skillBar = mUIObjectMap.getObject("skillValue").GetComponent<zzGUIProgressBarBase>();

        if (!rebirthClockUI)
            rebirthClockUI = mUIObjectMap.getObject("rebirthClock").GetComponent<zzInterfaceGUI>();

        var lManagerTransform = GameSceneManager.Singleton.getManager(GameScene.Singleton.playerInfo.race,
            GameSceneManager.UnitManagerType.raceBase).managerRoot;
        if(lManagerTransform.childCount>0)
        {
            var lRaceBaseBloodRateUI = mUIObjectMap.getObject("coreValue").GetComponent<zzGUIProgressBarBase>();
            lManagerTransform.GetChild(0).GetComponent<Life>().addBloodValueChangeCallback(
                (x) => lRaceBaseBloodRateUI.rate = x.rate);
        }

    }

    void Start()
    {
        /*
            if( zzCreatorUtility.isHost() )
            {
                //setOwer(Network.player);
                zzCreatorUtility.sendMessage(gameObject,"setOwerImp",Network.player);
                createHero();
            }
        */
        //getUI();
        if (autoCreatePlayer)
            createHeroFirstTime();
    }

    public NetworkPlayer owner
    {
        set
        {
            setOwerImp(value);
            if(Network.isServer)
            {
                GetComponent<BoundNetworkScope>().networkPlayer = value;
            }
        }

        get { return _owner; }
        //zzCreatorUtility.sendMessage(gameObject, "setOwerImp", pOwner);
    }

    public void releaseHeroControl()
    {
        _releaseControl(hero);
    }

    public void setOwerImp(NetworkPlayer pOwner)
    {
        _owner = pOwner;
        if (HeroBelongToThePlayer())
            getUI();
    }

    public void createHeroFirstTime()
    {
        if (haveFirstCreate)
            Debug.LogError("haveFirstCreate == true");


        _createHero();
        zzItemBagControl itemBagControl = hero.GetComponent<zzItemBagControl>();
        itemBagControl.addCallAfterStart(_toGetItemBagID);
        haveFirstCreate = true;
    }

    void _toGetItemBagID()
    {
        zzItemBagControl itemBagControl = hero.GetComponent<zzItemBagControl>();
        itemBagID = itemBagControl.getBagID();
    }

    //function _theHeroDead()


    //创建只能在服务器端调用
    void _rebirthHero(Life p)
    {
        if (!haveFirstCreate)
            Debug.LogError("haveFirstCreate == false");

        if (HeroBelongToThePlayer())
        {
            //释放之前的输入
            _releaseControl(hero);
        }

        _createHeroRebirthClock();

        //重生的延迟执行
        rebirthTimer = gameObject.AddComponent<zzTimer>();

        rebirthTimer.addImpFunction(_rebirthHeroCreate);
        rebirthTimer.setInterval(rebirthTime);
        //if(Network.peerType !=NetworkPeerType.Disconnected)
        //	GameObject lHeroObject = Network.Instantiate(heroPrefab,transform.position,Quaternion(),0);

        //haveFirstCreate = true;
    }

    //此英雄是否属于此玩家
    public bool HeroBelongToThePlayer()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            return true;
        }

        if (Network.player == _owner)
            return true;

        return false;
    }

    //计时器的显示更新
    protected void _createHeroRebirthClock()
    {
        /*
            if(Network.peerType ==NetworkPeerType.Disconnected)
                RPCCreateHeroRebirthClock();
            else
            {
                if(Network.player == owner )//服务器端
                    RPCCreateHeroRebirthClock();
                else
                    networkView.RPC("RPCCreateHeroRebirthClock",owner);
            }
        */
        if (HeroBelongToThePlayer())
            RPCCreateHeroRebirthClock();
        else
            networkView.RPC("RPCCreateHeroRebirthClock", _owner);
    }

    [RPC]
    void RPCCreateHeroRebirthClock()
    {
        rebirthClockTimer = gameObject.AddComponent<zzTimer>();
        rebirthClockTimer.addImpFunction(_updateRebirthTimeLeave);
        rebirthClockTimer.setInterval(1.0f);

        rebirthClockUI.setVisible(true);
        rebirthTimeLeave = rebirthTime;
        rebirthClockUI.setText(rebirthTimeLeave.ToString());
    }

    //被rebirthTimer调用
    protected void _rebirthHeroCreate()
    {
        //计时结束 rebirthTimer 的Update,并销毁
        rebirthTimer.enabled = false;
        Destroy(rebirthTimer);

        _createHero();

        zzItemBagControl itemBagControl = hero.GetComponent<zzItemBagControl>();
        itemBagControl.setUseExistBag(itemBagID);
    }

    //被rebirthClockTimer调用
    protected void _updateRebirthTimeLeave()
    {
        --rebirthTimeLeave;
        if (rebirthTimeLeave <= 0)
        {
            //计时结束 关闭rebirthClockTimer 的Update,并销毁
            rebirthClockUI.setVisible(false);
            rebirthClockTimer.enabled = false;
            Destroy(rebirthClockTimer);
        }
        rebirthClockUI.setText(rebirthTimeLeave.ToString());
    }

    GameObject CreateHeroObject()
    {
        GameObject lHeroObject;
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            lHeroObject = (GameObject)Instantiate(heroPrefab,
                transform.position, Quaternion.identity);
            InitHeroObject(lHeroObject);
        }
        else
        {
            var lViewID = Network.AllocateViewID();
            lHeroObject = HeroSpawnCreateHeroObject(lViewID);
            networkView.RPC("HeroSpawnCreateHeroObject", RPCMode.Others, lViewID);
        }
        return lHeroObject;
    }

    [RPC]
    GameObject HeroSpawnCreateHeroObject(NetworkViewID pViewID)
    {
        GameObject lHeroObject = (GameObject)Instantiate(heroPrefab,
            transform.position, Quaternion.identity);
        lHeroObject.networkView.viewID = pViewID;
        InitHeroObject(lHeroObject);
        return lHeroObject;
    }

    void InitHeroObject(GameObject pObject)
    {
        hero = pObject;
        setPlayerName(_playerName);
        setPlayerNameColor(_playerNameColor);
    }

    //创建只能在服务器端调用
    protected GameObject _createHero()
    {
        GameObject lHeroObject = CreateHeroObject();

        //创建控制器
        if (Network.peerType == NetworkPeerType.Disconnected)
            createControl(lHeroObject);
        else
        {
            if (Network.player == _owner)//服务器端
            {
                RPCcreateNetControl(lHeroObject.networkView.viewID);
            }
            else
            {
                networkView.RPC("RPCcreateNetControl", _owner, lHeroObject.networkView.viewID);
                var lPlayerScope = GetComponent<PlayerScope>();
                lPlayerScope.actionCommandControl = lHeroObject.GetComponent<ActionCommandControl>();
                lPlayerScope.playerTransform = lHeroObject.transform;
                lPlayerScope.enabled = true;
            }
        }

        lHeroObject.GetComponent<Life>().addDieCallback(_rebirthHero);

        return lHeroObject;
    }

    public void destroyTheSpawn()
    {
        if(hero)
        {
            var lLife = hero.GetComponent<Life>();
            lLife.removeDieCallback(_rebirthHero);
            lLife.makeDead();
        }
        Destroy(gameObject);
    }

    //client
    [RPC]
    protected void RPCcreateNetControl(NetworkViewID pHeroID)
    {
        GameObject lHeroObject = NetworkView.Find(pHeroID).gameObject;
        var lViewID = Network.AllocateViewID();
        createNetControl(lHeroObject, lViewID);
        networkView.RPC("HeroSpawnLinkControl", RPCMode.Others, pHeroID, lViewID);
    }

    //all
    [RPC]
    void HeroSpawnLinkControl(NetworkViewID pHeroID, NetworkViewID pNetSysnID)
    {
        GameObject lHeroObject = NetworkView.Find(pHeroID).gameObject;
        createNetControl(lHeroObject, pNetSysnID);
    }

    protected void createNetControl(GameObject pHeroObject, NetworkViewID pNetSysnID)
    {
        //创建同步组件
        GameObject lnetSysn = (GameObject)Instantiate(netSysnPrefab);
        lnetSysn.networkView.viewID = pNetSysnID;
        HeroNetView lHeroNetView = lnetSysn.GetComponent<HeroNetView>();
        lHeroNetView.setOwner(pHeroObject);
        //在start化后再开启
        //lHeroNetView.networkView.enabled = false;
        if(_owner==Network.player)
            createControl(pHeroObject);
    }

    protected void createControl(GameObject pHeroObject)
    {
        //for debug
        var lPlayerScope = GetComponent<PlayerScope>();
        lPlayerScope.actionCommandControl = pHeroObject.GetComponent<ActionCommandControl>();
        lPlayerScope.playerTransform = pHeroObject.transform;

        //绑定输入
        //GameObject lSystem = GameObject.Find("System");
        mainInput lMainInput = SystemObject.GetComponent<mainInput>();
        lMainInput.setToControl(pHeroObject.GetComponent<ActionCommandControl>());
        //print(pHeroObject.name);
        //print(pHeroObject.GetInstanceID());

        //绑定UI
        pHeroObject.AddComponent<BagItemUI>().showSelected = false;
        pHeroObject.AddComponent<MoneyUI>();
        pHeroObject.AddComponent<bagItemUIInput>();
        var lActionEnergyValue = pHeroObject.AddComponent<ActionEnergyValue>();
        lActionEnergyValue.addValueChangedReceiver(skillBar.setRate);
        pHeroObject.GetComponent<Hero>().actionEnergyValue = lActionEnergyValue;
        var lSoldierFactoryStateUI = pHeroObject.AddComponent<SoldierFactoryStateUI>();
        lSoldierFactoryStateUI.race = PlayerInfo.getRace(pHeroObject.layer);
        lSoldierFactoryStateUI.owner = pHeroObject;
        pHeroObject.AddComponent<SoldierFactoryStateUIInput>();

        //使用血条UI
        //GameObject.Destroy( pHeroObject.transform.FindChild("bloodBar").gameObject );
        //防止血值改变时,回调bloodBar导致错误
        pHeroObject.transform.FindChild("bloodBar").renderer.enabled = false;
        Life lLife = pHeroObject.GetComponent<Life>();
        if (bloodBar)
            updateBloodBar(lLife);
        lLife.addBloodValueChangeCallback(updateBloodBar);

        //绑定摄像机
        _2DCameraFollow lCameraFollow = zzObjectMap.getObject("GameCamera").GetComponent<_2DCameraFollow>();
        lCameraFollow.setTaget(pHeroObject.transform);

    }

    protected void _releaseControl(GameObject pHeroObject)
    {
        mainInput lMainInput = SystemObject.GetComponent<mainInput>();
        lMainInput.setToControl(null);
        if(pHeroObject)
        {
            Destroy(pHeroObject.GetComponent<bagItemUIInput>());
            Destroy(pHeroObject.GetComponent<SoldierFactoryStateUIInput>());
        }

    }

}