
using UnityEngine;
using System.Collections;

/// <summary>
/// 使用协同的计时器,也可用于某些含有协同的函数的Update中的调用
/// </summary>
public class zzTimerCoroutine : MonoBehaviour
{
    public float interval;

    public delegate IEnumerator ImpFunc();

    ImpFunc impFunction;

    bool inPlaying = true;

    public void endTimer()
    {
        inPlaying = false;
    }

    IEnumerator Start()
    {
        while (inPlaying)
        {
            yield return StartCoroutine(impFunction());
            yield return new WaitForSeconds(interval);
        }
        Destroy(this);
    }

    public void setImpFunction(ImpFunc pFunc)
    {
        impFunction = pFunc;
    }

    public void setInterval(float pInterval)
    {
        interval = pInterval;
    }

}