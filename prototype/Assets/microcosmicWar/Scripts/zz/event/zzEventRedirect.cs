using UnityEngine;

public class zzEventRedirect:MonoBehaviour
{
    System.Action<Collision> onCollisionEnterReceiver;
    System.Action<Collider> onTriggerEnterReceiver;
    System.Action<Collider> onTriggerExitReceiver;

    static void nullColliderReceiver(Collider p) { }
    static void nullCollisionReceiver(Collision p) { }

    public void addOnCollisionEnterReceiver(System.Action<Collision> pReceiver)
    {
        onCollisionEnterReceiver += pReceiver;
    }

    public void addOnTriggerEnterReceiver(System.Action<Collider> pReceiver)
    {
        onTriggerEnterReceiver += pReceiver;
    }

    public void addOnTriggerExitReceiver(System.Action<Collider> pReceiver)
    {
        onTriggerExitReceiver += pReceiver;
    }

    void Start()
    {
        if (onCollisionEnterReceiver == null)
            onCollisionEnterReceiver = nullCollisionReceiver;
        if (onTriggerEnterReceiver == null)
            onTriggerEnterReceiver = nullColliderReceiver;
        if (onTriggerExitReceiver == null)
            onTriggerExitReceiver = nullColliderReceiver;
    }

    //public void messageRedirectReceiver(string methodName)
    //{
    //    //print("@@@@@@@methodName: "+methodName);
    //    messageReceiver.gameObject.SendMessage(methodName);
    //}

    void OnCollisionEnter(Collision pCollision)
    {
        onCollisionEnterReceiver( pCollision);
    }

    void OnTriggerEnter(Collider pCollider)
    {
        onTriggerEnterReceiver(pCollider);
    }

    void OnTriggerExit(Collider pCollider)
    {
        onTriggerExitReceiver(pCollider);
    }
}