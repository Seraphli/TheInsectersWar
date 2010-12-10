
using UnityEngine;
using System.Collections;

public class MessageRedirect : MonoBehaviour
{


    public Transform messageReceiver;

    //void Start()
    //{
        //if(!messageReceiver)
        //	messageReceiver=transform.parent;
    //}

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