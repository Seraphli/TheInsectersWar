
using UnityEngine;
using System.Collections;

public class zzTimer : MonoBehaviour
{

    [SerializeField]
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

    public void setImpFunction(zzUtilities.voidFunction pFunc)
    {
        timer.setImpFunction(pFunc);
    }

    public void addImpFunction(zzUtilities.voidFunction pFunc)
    {
        timer.addImpFunction(pFunc);
    }

    public void setInterval(float pInterval)
    {
        interval = pInterval;
        timer.setInterval(pInterval);
    }

    void Update()
    {
        timer.Update();
    }
}