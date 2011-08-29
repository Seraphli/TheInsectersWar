using UnityEngine;

public class SetEnabledWhenDie:MonoBehaviour
{
    public bool enabledToSet;
    public Behaviour objectToSet;

    void Awake()
    {
        Life life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        objectToSet.enabled = enabledToSet;
    }
}