using UnityEngine;

public class OffNetStateSynWhenDie : MonoBehaviour
{
    void Start(){}
    void Awake()
    {
        if(enabled)
        {
            Life life = gameObject.GetComponentInChildren<Life>();
            life.addDieCallback(deadAction);
        }
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        networkView.stateSynchronization = NetworkStateSynchronization.Off;
    }
}