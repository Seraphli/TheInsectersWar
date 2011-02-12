
using UnityEngine;
using System.Collections;

public class zzCoroutineTimer : MonoBehaviour
{
    public float interval;

    protected zzUtilities.voidFunction impFunction = zzUtilities.nullFunction;

    IEnumerator Start()
    {
        while(true)
        {
            impFunction();
            yield return new WaitForSeconds(interval);
        }
    }

    public void setImpFunction(zzUtilities.voidFunction pFunc)
    {
        impFunction = pFunc;
    }

    public void setInterval(float pInterval)
    {
        interval = pInterval;
    }

}