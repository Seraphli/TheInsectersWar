
using UnityEngine;
using System.Collections;


class sendMessageWhenDie:MonoBehaviour
{
    public Life life;

    public GameObject messageReceiver;
    public string methodName;
    public bool sendSelf = false;

    void Start()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        if (messageReceiver)
        {
            if (sendSelf)
                messageReceiver.SendMessage(methodName, this, SendMessageOptions.RequireReceiver);
            else
                messageReceiver.SendMessage(methodName, SendMessageOptions.RequireReceiver);

        }

    }

}