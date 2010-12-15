
using UnityEngine;
using System.Collections;

[System.Serializable]
class zzTimerClass
{

    public float interval = 1.0f;

    //protected float timeToWait;//=1.0f/frequencyOfImplement

    public float timePos = 0.0f;

    protected zzUtilities.voidFunction impFunction = zzUtilities.nullFunction;

    public bool enable=true;

    //public void setFrequencyOfImp(float pFrequency)
    //{
    //    frequencyOfImplement = pFrequency;
    //}

    public void setInterval(float pInterval)
    {
        interval = pInterval;
    }


    public void setImpFunction(zzUtilities.voidFunction pFunc)
    {
        impFunction = pFunc;
    }

    public void Update()
    {
        timePos += Time.deltaTime;
        if (enable&&timePos > interval)
        {
            impFunction();
            timePos = 0.0f;
        }
    }

}