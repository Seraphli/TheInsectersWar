
using UnityEngine;
using System.Collections;

class dieWhenDie : MonoBehaviour
{
    public float delayTime = 0.0f;
    void Start()
    {
        Life life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        gameObject.layer = layers.deadObject;
        Destroy(gameObject, delayTime);
    }
}