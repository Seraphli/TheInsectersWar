
using UnityEngine;
using System.Collections;

public class zzGUIProgressBarBase : zzGUIGroup
{
    public zzGUIDirection direction = zzGUIDirection.horizontal;
    public float rate;

    public void setRate(float pRate)
    {
        rate = pRate;
    }

}