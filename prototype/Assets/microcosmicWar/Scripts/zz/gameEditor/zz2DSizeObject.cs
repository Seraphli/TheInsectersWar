using UnityEngine;

public class zz2DSizeObject:MonoBehaviour
{
    public zz2DEditorBounds bounds;
    [LabelUI(verticalDepth = 1, horizontalDepth = 0)]
    public const string freezePositionLabel = "尺寸";

    [FieldUI("宽", verticalDepth = 2, horizontalDepth = 1)]
    public float freezeXPosition
    {
        get { return transform.localScale.x * bounds.width; }
        set
        {
            var lScale = transform.localScale;
            lScale.x = value / bounds.width;
            transform.localScale = lScale;
        }
    }


    [FieldUI("高", verticalDepth = 2, horizontalDepth = 2)]
    public float freezeYPosition
    {
        get { return transform.localScale.y * bounds.height; }
        set
        {
            var lScale = transform.localScale;
            lScale.y = value / bounds.height;
            transform.localScale = lScale;
        }
    }

}