using UnityEngine;

public class OffNetStateSynWhenDie : MonoBehaviour
{
    void Awake()
    {
        Life life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        networkView.stateSynchronization = NetworkStateSynchronization.Off;
    }
}