using UnityEngine;

public class zz2DEditorPosition:MonoBehaviour
{
    [LabelUI(verticalDepth = -1, horizontalDepth = 0)]
    public const string freezePositionLabel = "位置";

    [FieldUI("X", verticalDepth = 0, horizontalDepth = 1)]
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


    [FieldUI("Y", verticalDepth = 0, horizontalDepth = 2)]
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
}