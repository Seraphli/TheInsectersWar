
using UnityEngine;
using System.Collections;

public class timerTest : MonoBehaviour
{
    public zzCoroutineTimer timer;

    void Start()
    {
        timer.setImpFunction(outInfo);

    }

    void outInfo()
    {
        print("timerTest");
    }
}