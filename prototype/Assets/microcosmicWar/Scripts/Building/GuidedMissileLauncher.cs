using UnityEngine;
using System.Collections;

class GuidedMissileLauncher : DefenseTower
{
    public delegate Transform GetTargetFunc();
    public GetTargetFunc getTargetFunc = nullGetTargetFunc;

    static Transform nullGetTargetFunc()
    {
        return null;
    }

    public float[] fireInterval = new float[]{};

    public zzTimer fireTimer;

    //现在间隔的索引
    protected int fireIntervalIndex = 0;

    protected int moveToNextIntervalIndex()
    {
        if (fireIntervalIndex >= fireInterval.Length)
            fireIntervalIndex = 0;
        return fireIntervalIndex++;

    }

    protected float getIntervalAndMove()
    {
        int lIndex = moveToNextIntervalIndex();
        return fireInterval[lIndex];
    }

    public SphereAreaHarm sphereAreaHarm;

    public void createSphereAreaHarm(Life pLife)
    {
        GameObject lAreaHarm = (GameObject)GameObject.Instantiate(sphereAreaHarm.gameObject, pLife.transform.position, pLife.transform.rotation);

        SphereAreaHarm lSphereAreaHarm = lAreaHarm.GetComponent<SphereAreaHarm>();
        lSphereAreaHarm.setHarmLayerMask(adversaryLayerMask.value);
    }

    protected void fireAndSetNextTime()
    {
        var lTarget = getTargetFunc();
        if (lTarget)
        {
            var lBulletObject = emitter.EmitBullet();
            BulletFollowAI lBulletFollowAI = lBulletObject[0].GetComponent<BulletFollowAI>();
            lBulletFollowAI.setTarget(lTarget);

            Life lBulletLife = lBulletObject[0].gameObject.GetComponent<Life>();
            lBulletLife.addDieCallback(createSphereAreaHarm);

        }
        fireTimer.setInterval(getIntervalAndMove());
    }

    public override int getBulletLayer()
    {
        return PlayerInfo.getMissileLayer(race);
        //return LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer) + "Missile");
    }

    public LayerMask adversaryLayerMask;

    public override void setAdversaryLayerMask(LayerMask pLayer)
    {

        if (zzCreatorUtility.isHost())
        {
            adversaryLayerMask = pLayer;
            GuidedMissileLauncherAI lAi = GetComponentInChildren<GuidedMissileLauncherAI>();
            lAi.setAdversaryLayerMask(pLayer);
        }
    }


    protected override void initWhenHost()
    {
        fireTimer = gameObject.AddComponent<zzTimer>();
        fireTimer.addImpFunction(fireAndSetNextTime);
        fireTimer.setInterval(getIntervalAndMove());
    }


    public override void Start()
    {
        base.Start();
    }

    //virtual function Update () 
    //{
    //	super.Update();
    //}


}