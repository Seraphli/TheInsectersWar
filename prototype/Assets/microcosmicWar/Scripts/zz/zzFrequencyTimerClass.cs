
using UnityEngine;
using System.Collections;

[System.Serializable]
public class zzFrequencyTimer
{

    //执行频率/每秒执行的次数
    public float frequencyOfImplement = 3.0f;

    //protected float timeToWait;//=1.0f/frequencyOfImplement

    public float timePos = 0.0f;

    protected zzUtilities.voidFunction impFunction = zzUtilities.nullFunction;

    public void setFrequencyOfImp(float pFrequency)
    {
        frequencyOfImplement = pFrequency;
    }


    public void setImpFunction(zzUtilities.voidFunction pFunc)
    {
        impFunction = pFunc;
    }

    public void Update()
    {
        timePos += Time.deltaTime;
        if (timePos > 1.0f / frequencyOfImplement)
        {
            impFunction();
            timePos = 0.0f;
        }
    }

}