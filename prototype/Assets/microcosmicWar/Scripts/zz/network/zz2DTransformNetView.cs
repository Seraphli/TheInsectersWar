using UnityEngine;

public class zz2DTransformNetView:MonoBehaviour
{
    public float zPosition = 0f;
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 lData = Vector3.zero;
        if(stream.isWriting)
        {
            lData = transform.localPosition;
            lData.z = transform.localRotation.eulerAngles.z;
        }
        stream.Serialize(ref lData);
        if (stream.isReading)
        {
            transform.localPosition = new Vector3(lData.x, lData.y, zPosition);
            var lRotation = Quaternion.Euler(0f, 0f, lData.z);
            transform.localRotation = lRotation;
        }
    }
}