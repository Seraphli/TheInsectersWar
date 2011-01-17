
using UnityEngine;
using System.Collections;

public class zzTimer : MonoBehaviour
{


    public float interval;

    public zzFrequencyTimer frequencyTimer = new zzFrequencyTimer();

    public float timePos
    {
        get { return frequencyTimer.timePos; }
        set { frequencyTimer.timePos = value; }
    }

    void Start()
    {
        setInterval(interval);
    }

    public void setImpFunction(zzUtilities.voidFunction pFunc)
    {
        frequencyTimer.setImpFunction(pFunc);
    }

    public void setInterval(float pInterval)
    {
        //print(1.0f/pInterval);
        //防止在 Start 前执行,使interval未赋值执行setInterval
        interval = pInterval;
        frequencyTimer.setFrequencyOfImp(1.0f / pInterval);
    }

    void Update()
    {
        frequencyTimer.Update();
    }
}