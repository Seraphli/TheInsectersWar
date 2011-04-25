using UnityEngine;


public class zzOnUpdateOnce : zzOnEventBase
{

    void Update()
    {
        onEvent();
        enabled = false;
        Destroy(this);
    }
}