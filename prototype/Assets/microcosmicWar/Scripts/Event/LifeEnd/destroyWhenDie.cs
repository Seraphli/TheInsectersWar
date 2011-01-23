
using UnityEngine;
using System.Collections;

public class destroyWhenDie : MonoBehaviour
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
        Destroy(gameObject, delayTime);
    }
}