
using UnityEngine;
using System.Collections;

class CreateObjectWhenTouch : MonoBehaviour
{
    public GameObject objectToCreate;

    void OnTriggerEnter(Collider other)
    {
        Instantiate(objectToCreate, other.transform.position, Quaternion.identity);
    }
}