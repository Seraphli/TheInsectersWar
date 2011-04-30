
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    public override GameObject[] EmitBullet()
    {
        List<GameObject> lOut = new List<GameObject>();
        foreach (Emitter iEmit in emitList)
        {
            lOut.AddRange(iEmit.EmitBullet());
        }
        if (fireSound)
        {
            fireSound.Play();
        }
        playFireSpark();
        return lOut.ToArray();
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
