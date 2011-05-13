using UnityEngine;

public class zz2DRotator:MonoBehaviour
{
    [FieldUI("角度", verticalDepth = -3)]
    [SliderUI(0f, 359.99f, verticalDepth = -2)]
    public float objectRotation
    {
        get
        {
            return transform.localRotation.eulerAngles.z;
        }
        set
        {
            var lLocalRotation = new Quaternion();
            lLocalRotation.eulerAngles = new Vector3(0f, 0f, value);
            transform.localRotation = lLocalRotation;
        }
    }
}