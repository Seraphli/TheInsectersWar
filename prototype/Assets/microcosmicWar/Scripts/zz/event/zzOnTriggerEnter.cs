using UnityEngine;

public class zzOnTriggerEnter : zzOnEventBase
{
    void OnTriggerEnter(Collider pCollider)
    {
        onEvent();
    }

}