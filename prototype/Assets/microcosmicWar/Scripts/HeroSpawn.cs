﻿
using UnityEngine;
using System.Collections;

public class HeroSpawn : MonoBehaviour
{

    public GameObject heroPrefab;
    public GameObject netSysnPrefab;

    public NetworkPlayer owner;
    public bool autoCreatePlayer = false;
    public int itemBagID;

    public GameObject hero;
    public zzInterfaceGUI rebirthClockUI;
    public GameObject SystemObject;

    //是否创建过
    protected bool haveFirstCreate = false;

    //重生所需的时间
    public int rebirthTime = 10;
    protected zzTimer rebirthClockTimer;
    //重生剩余的时间
    protected int rebirthTimeLeave = 0;
    protected zzTimer rebirthTimer;
    zzSceneObjectMap mUIObjectMap;
    zzGUIProgressBar mBloodBar;

    void updateBloodBar(Life life)
    {
        mBloodBar.rate = life.getRate();
    }

    void Start()
    {
        mUIObjectMap = zzObjectMap.getObject("UI").GetComponent<zzSceneObjectMap>();
        mBloodBar = mUIObjectMap.getObject("bloodBar").GetComponent<zzGUIProgressBar>();
        /*
            if( zzCreatorUtility.isHost() )
            {
                //setOwer(Network.player);
                zzCreatorUtility.sendMessage(gameObject,"setOwerImp",Network.player);
                createHero();
            }
        */
        if (autoCreatePlayer)
            createHeroFirstTime();
        if (!rebirthClockUI)
            rebirthClockUI = mUIObjectMap.getObject("rebirthClock").GetComponent<zzInterfaceGUI>();

        if (!SystemObject)
            //SystemObject = GameObject.Find("System");
            SystemObject = zzObjectMap.getObject("system");
    }

    public void setOwer(NetworkPlayer pOwner)
    {
        //owner = pOwner;
        zzCreatorUtility.sendMessage(gameObject, "setOwerImp", pOwner);
    }

    public void releaseHeroControl()
    {
        _releaseControl(hero);
    }

    [RPC]
    public void setOwerImp(NetworkPlayer pOwner)
    {
        owner = pOwner;
    }

    public void createHeroFirstTime()
    {
        if (haveFirstCreate)
            Debug.LogError("haveFirstCreate == true");
        hero = _createHero();
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
    void _rebirthHero()
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

        rebirthTimer.setImpFunction(_rebirthHeroCreate);
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

        if (Network.player == owner)
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
            networkView.RPC("RPCCreateHeroRebirthClock", owner);
    }

    [RPC]
    void RPCCreateHeroRebirthClock()
    {
        rebirthClockTimer = gameObject.AddComponent<zzTimer>();
        rebirthClockTimer.setImpFunction(_updateRebirthTimeLeave);
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

        hero = _createHero();

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

    //创建只能在服务器端调用
    protected GameObject _createHero()
    {
        GameObject lHeroObject = zzCreatorUtility.Instantiate(heroPrefab, transform.position, new Quaternion(), 0);

        //print(lHeroObject==null);
        //print(lHeroObject.GetInstanceID());
        //print(owner);
        //zzCreatorUtility.sendMessage(gameObject,"createNetControl",lHeroObject.networkView.viewID);
        if (Network.peerType == NetworkPeerType.Disconnected)
            createControl(lHeroObject);
        else
        {
            if (Network.player == owner)//服务器端
                createNetControl(lHeroObject);
            else
                networkView.RPC("RPCcreateNetControl", owner, lHeroObject.networkView.viewID);
        }

        IobjectListener lRemoveCall = (IobjectListener)lHeroObject.GetComponent<IobjectListener>();

        lRemoveCall.setRemovedCallFunc(_rebirthHero);

        return lHeroObject;
    }

    [RPC]
    protected void RPCcreateNetControl(NetworkViewID pHeroID)
    {
        GameObject lHeroObject = NetworkView.Find(pHeroID).gameObject;
        createNetControl(lHeroObject);
    }

    protected void createNetControl(GameObject pHeroObject)
    {
        //创建同步组件
        GameObject lnetSysn = zzCreatorUtility.Instantiate(netSysnPrefab, new Vector3(), new Quaternion(), 0);
        HeroNetView lHeroNetView = lnetSysn.GetComponent<HeroNetView>();
        lHeroNetView.setOwner(pHeroObject);

        createControl(pHeroObject);
    }

    protected void createControl(GameObject pHeroObject)
    {
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
        var lSoldierFactoryStateUI = pHeroObject.AddComponent<SoldierFactoryStateUI>();
        lSoldierFactoryStateUI.race = PlayerInfo.getRace(pHeroObject.layer);
        lSoldierFactoryStateUI.onwer = pHeroObject;
        pHeroObject.AddComponent<SoldierFactoryStateUIInput>();

        //使用血条UI
        GameObject.Destroy( pHeroObject.transform.FindChild("bloodBar").gameObject );
        Life lLife = pHeroObject.GetComponent<Life>();
        if (mBloodBar)
            updateBloodBar(lLife);
        lLife.addBloodValueChangeCallback(updateBloodBar);

        //绑定摄像机
        _2DCameraFollow lCameraFollow = GameObject.Find("Main Camera").GetComponent<_2DCameraFollow>();
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