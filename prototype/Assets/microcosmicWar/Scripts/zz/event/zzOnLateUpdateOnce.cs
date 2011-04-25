using UnityEngine;

public class zzOnLateUpdateOnce : zzOnEventBase
{
    void LateUpdate()
    {
        onEvent();
        enabled = false;
        Destroy(this);
    }
}