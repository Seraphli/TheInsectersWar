using UnityEngine;
using System.Collections;

class GuidedMissileLauncher : DefenseTower
{
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

    protected void fireAndSetNextTime()
    {
        emitter.EmitBullet();
        fireTimer.setInterval(getIntervalAndMove());
    }

    public override int getBulletLayer()
    {
        return PlayerInfo.getMissileLayer(race);
        //return LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer) + "Missile");
    }

    public override void setAdversaryLayerMask(LayerMask pLayer)
    {

        if (zzCreatorUtility.isHost())
        {
            GuidedMissileLauncherAI lAi = GetComponentInChildren<GuidedMissileLauncherAI>();
            lAi.setAdversaryLayerMask(pLayer);
        }
    }

    public override void Start()
    {
        base.Start();
        if (!fireTimer)
            fireTimer = gameObject.AddComponent<zzTimer>();
        fireTimer.setImpFunction(fireAndSetNextTime);
        fireTimer.setInterval(getIntervalAndMove());
    }

    //virtual function Update () 
    //{
    //	super.Update();
    //}


}