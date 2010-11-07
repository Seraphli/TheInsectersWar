
using UnityEngine;
using System.Collections;

public class DeadZone : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        Life lLife = other.gameObject.GetComponent<Life>();
        if (lLife)
            lLife.makeDead();
    }
}