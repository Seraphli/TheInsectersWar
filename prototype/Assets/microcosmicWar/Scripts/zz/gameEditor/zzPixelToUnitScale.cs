using UnityEngine;

public class zzPixelToUnitScale:MonoBehaviour
{
    public Transform objectToScale;
    public int width;
    public int height;
    public float _lengthPerPixel;

    public void setImageSize(int pWidth,int pHeight)
    {
        width = pWidth;
        height = pHeight;
    }

    [FieldUI("像素长度")]
    public float lengthPerPixel
    {
        get { return _lengthPerPixel; }
        set { _lengthPerPixel = value; }
    }

    [FieldUI("单位长度像素量")]
    public float PixelPerlength
    {
        get { return 1f / _lengthPerPixel; }
        set { _lengthPerPixel = 1f / value; }
    }

    [ButtonUI("应用尺寸",verticalDepth = 3)]
    public void doScale()
    {
        var lSize = objectToScale.localScale;
        lSize.x = _lengthPerPixel * width;
        lSize.y = _lengthPerPixel * height;
        objectToScale.localScale = lSize;
    }
}