
using UnityEngine;
using System.Collections;

/// <summary>
/// 使用协同的计时器,也可用于某些含有协同的函数的Update中的调用
/// </summary>
public class zzCoroutineTimer : MonoBehaviour
{
    public float interval;

    protected zzUtilities.voidFunction impFunction = zzUtilities.nullFunction;

    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            impFunction();
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