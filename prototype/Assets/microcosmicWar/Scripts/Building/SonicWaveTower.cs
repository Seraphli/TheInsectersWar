
using UnityEngine;
using System.Collections;

public class SonicWaveTower : MonoBehaviour
{

    public float attackInterval = 1.0f;
    public Race race = Race.eNone;

    //半径为0.5 的plane
    public GameObject WaveObject;
    public Transform center;
    public float waveCreatedInterval = 1.0f;

    //波运行的速度
    public float waveSpeed = 1.0f;


    public float harmRadius = 5.0f;
    public float harmValueInCentre = 400.0f;
    public int harmLayerMask;

    public float restTimeLong = 4.1f;
    public float fireTimeLong = 4.1f;

    public float timeOffset = 3f;

    [RPC]
    void RPCSonicWaveTowerOnFire(NetworkMessageInfo pInfo )
    {
        fireOn((float)(Network.time - pInfo.timestamp));
    }

    void fireOn()
    {
        fireOn(0f);
    }

    void fireOn(float pTimeOffset)
    {
        waveTimer.enabled = true;
        fireTimer.timePos = pTimeOffset;
        fireTimer.setInterval(fireTimeLong);
        fireTimer.setImpFunction((zzUtilities.voidFunction)fireOff);
        if (Network.isServer)
            networkView.RPC("RPCSonicWaveTowerOnFire", RPCMode.Others);
    }

    void fireOff()
    {
        fireOff(0f);
    }

    void fireOff(float pTimeOffset)
    {
        waveTimer.enabled = false;
        fireTimer.timePos = pTimeOffset;
        fireTimer.setInterval(restTimeLong);
        fireTimer.setImpFunction((zzUtilities.voidFunction)fireOn);
    }

    void Awake()
    {
        initRace(race);
    }

    void createWave()
    {
        GameObject lWave = (GameObject)Instantiate(WaveObject, center.position, new Quaternion());
        lWave.transform.localScale = Vector3.zero;

        zzScaleInTime lScaleInTime = lWave.AddComponent<zzScaleInTime>();
        lScaleInTime.scaleVelocity = new Vector3(waveSpeed, waveSpeed, 1.0f );

        //zzDestroySelfInTime lDestroySelfInTime = lWave.AddComponent<zzDestroySelfInTime>();
        //lDestroySelfInTime.time = harmRadius / waveSpeed

        //让波只能运行到可伤害的范围
        Destroy(lWave, harmRadius / waveSpeed * 2 );
    }

    //void My()
    //{
    //    SonicAttack sonicAttackAI = gameObject.GetComponentInChildren<SonicAttack>();
    //    sonicAttackAI.Attack();
    //}
    zzTimer waveTimer;

    zzTimer fireTimer;
    void Start()
    {
        if (zzCreatorUtility.isHost())
        {
            zzTimer lAttackTimer = gameObject.AddComponent<zzTimer>();
            lAttackTimer.setInterval(attackInterval);
            //zTimer.setImpFunction(My);
            lAttackTimer.addImpFunction(Attack);
        }

        waveTimer = gameObject.AddComponent<zzTimer>();
        waveTimer.setInterval(waveCreatedInterval);
        waveTimer.addImpFunction(createWave);
        fireTimer = gameObject.AddComponent<zzTimer>();
        fireOff(timeOffset);
    }

    public void initRace(Race pRace)
    {
        if (pRace == Race.eNone)
            return;
        race = pRace;
        GameObject lTowerObject = gameObject;
        //AcousticTower lAcousticTower = lTowerObject.GetComponent<AcousticTower>();
        //SonicAttack lSonicAttack = lTowerObject.GetComponent<SonicAttack>();

        lTowerObject.layer = PlayerInfo.getBuildingLayer(race);
        //lSonicAttack.setAdversaryLayer(PlayerInfo.getAdversaryRaceLayer(race));
        //lSonicAttack.setHarmLayerMask(1 << PlayerInfo.getAdversaryRaceLayer(pRace));
        setHarmLayerMask(1 << PlayerInfo.getAdversaryRaceLayer(pRace));

        foreach(Transform subShape in transform.Find("shape"))
            subShape.gameObject.layer = lTowerObject.layer;
        //collisionLayer.addCollider(gameObject);

    }

    public virtual void init(Hashtable info)
    {
        initRace((Race)info["race"]);
    }

    public void setHarmLayerMask(int pMark)
    {
        harmLayerMask = pMark;
    }

    //判断是否可以伤害
    //bool canHarm(Life pLife)
    //{
    //    ObjectProperty AcousticTowerTemp = pLife.gameObject.GetComponent<ObjectProperty>();
    //    //print(pLife.gameObject.name);
    //    if (AcousticTowerTemp
    //        && AcousticTowerTemp.identity == Identitys.Structure)
    //    {
    //        return false;
    //    }
    //    return true;

    //}

    public void Attack()
    {
        SphereAreaHarm.impSphereAreaHarm(transform.position, harmRadius, harmValueInCentre, harmLayerMask );
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(center.position, harmRadius);
    }
}