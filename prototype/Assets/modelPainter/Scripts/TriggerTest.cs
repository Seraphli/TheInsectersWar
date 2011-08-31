using UnityEngine;
using System.Collections;

class TriggerTest:MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        print("OnTriggerStay");
    }
    void OnTriggerExit(Collider other)
    {
        print("OnTriggerExit");
    }
    void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter");
    }

}