
using UnityEngine;
using System.Collections;


///AudioSource fireSound;


public class EmitterContainer : Emitter
{
    public Emitter[] emitList;

    public override void setInjureInfo(Hashtable pInjureInfo)
    {
        base.setInjureInfo(pInjureInfo);
        foreach (Emitter iEmit in emitList)
        {
            iEmit.setInjureInfo(pInjureInfo);
        }
    }

    //void  Start (){
    //}

    public override void EmitBullet()
    {
        foreach (Emitter iEmit in emitList)
        {
            iEmit.EmitBullet();
        }
        if (fireSound)
        {
            fireSound.Play();
        }
    }

    public override void setBulletLayer(int pBulletLayer)
    {
        //print("EmitterContainer.setBulletLayer");
        foreach (Emitter iEmit in emitList)
        {
            iEmit.setBulletLayer(pBulletLayer);
        }
    }
}
