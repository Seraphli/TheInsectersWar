using UnityEngine;
using System.Collections.Generic;

public class MessageRedirect : MonoBehaviour
{


    public Transform messageReceiver;

    [System.Serializable]
    public class ActionInfo
    {
        public string name;
        public zzOnAction action;
    }

    public ActionInfo[] actionList = new ActionInfo[0]{};

    Dictionary<string, zzOnAction> actionMap;

    void Awake()
    {
        if (actionList.Length == 0)
            return;

        actionMap = new Dictionary<string, zzOnAction>();
        foreach (var lActionInfo in actionList)
        {
            actionMap[lActionInfo.name] = lActionInfo.action;
        }
    }

    public void actionRedirectReceiver(string actionName)
    {
        actionMap[actionName].impAction();
    }

    public void messageRedirectReceiver(string methodName)
    {
        //print("@@@@@@@methodName: "+methodName);
        messageReceiver.gameObject.SendMessage(methodName);
    }

    void OnCollisionEnter(Collision pCollision)
    {
        messageReceiver.gameObject.SendMessage("OnCollisionEnter", pCollision);
    }

    void OnTriggerEnter(Collider pCollider)
    {
        messageReceiver.gameObject.SendMessage("OnTriggerEnter", pCollider);
    }

    void OnTriggerExit(Collider pCollider)
    {
        messageReceiver.gameObject.SendMessage("OnTriggerExit",
            pCollider, SendMessageOptions.DontRequireReceiver);
    }
}