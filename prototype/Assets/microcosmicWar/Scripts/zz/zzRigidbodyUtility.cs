using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class zzRigidbodyUtility:MonoBehaviour
{
    public Transform centerOfMass;
    void Start()
    {
        var lRigidbody = gameObject.GetComponent<Rigidbody>();
        lRigidbody.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
    }
}