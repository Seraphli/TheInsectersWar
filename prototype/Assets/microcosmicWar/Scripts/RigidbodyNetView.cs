
using UnityEngine;
using System.Collections;

class RigidbodyNetView : MonoBehaviour
{
    //Rigidbody 

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        var lIsKinematic = true;
        var lPosition = Vector3.zero;
        var lRot = Quaternion.identity;

        if (stream.isWriting)
        {
            lPosition = transform.position;
            lRot = transform.rotation;
            lIsKinematic = rigidbody.isKinematic;
            //lBoxPos = _supplyBox.transform.position;
        }

        stream.Serialize(ref lIsKinematic);
        stream.Serialize(ref lPosition);
        stream.Serialize(ref lRot);
        //stream.Serialize(ref lBoxPos);

        if (stream.isReading)
        {
            transform.position = lPosition;
            transform.rotation = lRot;
            rigidbody.isKinematic = lIsKinematic;
        }

        if(!lIsKinematic)
        {
            networkView.observed = rigidbody;
            Destroy(this);
        }
    }
}