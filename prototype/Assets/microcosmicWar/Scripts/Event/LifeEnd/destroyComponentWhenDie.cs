
using UnityEngine;
using System.Collections;

class destroyComponentWhenDie:MonoBehaviour
{
    public float delayTime = 0.0f;
    public Life life;
    public Component componentToDestroy;

    void Start()
    {
        if(!life)
            life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        Destroy(componentToDestroy, delayTime);
    }
}