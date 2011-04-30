
using UnityEngine;
using System.Collections;

public class zzTimer : MonoBehaviour
{


    public float interval;

    public zzTimerClass timer = new zzTimerClass();

    public float timePos
    {
        get { return timer.timePos; }
        set { timer.timePos = value; }
    }

    void Start()
    {
        setInterval(interval);
    }

    public void addImpFunction(zzUtilities.voidFunction pFunc)
    {
        timer.addImpFunction(pFunc);
    }

    public void setInterval(float pInterval)
    {
        timer.setInterval(pInterval);
    }

    void Update()
    {
        timer.Update();
    }
}