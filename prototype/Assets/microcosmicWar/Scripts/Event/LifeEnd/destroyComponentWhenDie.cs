
using UnityEngine;
using System.Collections;

class destroyComponentWhenDie:MonoBehaviour
{
    public float delayTime = 0.0f;
    public Component componentToDestroy;

    void Start()
    {
        Life life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        Destroy(componentToDestroy, delayTime);
    }
}