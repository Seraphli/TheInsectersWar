using UnityEngine;

public class zz2DTransformObject:MonoBehaviour
{
    [LabelUI(verticalDepth = 1, horizontalDepth = 0)]
    public const string freezePositionLabel = "位置";

    [FieldUI("X", verticalDepth = 2, horizontalDepth = 1)]
    public float freezeXPosition
    {
        get { return transform.position.x; }
        set
        {
            var lPosition = transform.position;
            lPosition.x = value;
            transform.position = lPosition;
        }
    }


    [FieldUI("Y", verticalDepth = 2, horizontalDepth = 2)]
    public float freezeYPosition
    {
        get { return transform.position.y; }
        set
        {
            var lPosition = transform.position;
            lPosition.y = value;
            transform.position = lPosition;
        }
    }


    [FieldUI("角度", verticalDepth = 3)]
    [SliderUI(0f, 359.99f, verticalDepth = 4)]
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