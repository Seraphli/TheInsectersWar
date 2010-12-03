
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

    void OnCollisionEnter(Collision collision)
    {
        messageReceiver.gameObject.SendMessage("OnCollisionEnter", collision);
    }

    void OnTriggerEnter(Collider collider)
    {
        messageReceiver.gameObject.SendMessage("OnTriggerEnter", collider);
    }
}