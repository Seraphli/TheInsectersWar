using UnityEngine;

public class zzActiveChart
{
    public zzActiveChart(int pWidth, int pHeight)
    {
        _width = pWidth;
        _height = pHeight;
        mImage = new bool[pWidth, pHeight];
    }

    bool[,] mImage;

    int _width;
    public int width
    {
        get { return _width; }
    }

    int _height;
    public int height
    {
        get { return _height; }
    }

    public void setActive(zzPoint pPoint, bool pActive)
    {
        setActive(pPoint.x, pPoint.y, pActive);
    }

    public void setActive(int x, int y, bool pActive)
    {
        mImage[x, y] = pActive;
    }

    public bool isActive(zzPoint pPoint)
    {
        return isActive(pPoint.x, pPoint.y);
    }

    public bool isActive(int x, int y)
    {
        return mImage[x, y];
    }

    public bool isInside(zzPoint pPoint)
    {
        return isInside(pPoint.x, pPoint.y);
    }

    public bool isInside(int x, int y)
    {
        return x >= 0
                && x < width
                && y >= 0
                && y < height;
    }

    public Texture2D asTexture()
    {
        Texture2D lOut = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
        for (int x = 0; x < _width; ++x)
        {
            for (int y = 0; y < _height; ++y)
            {
                if (isActive(x, y))
                {
                    lOut.SetPixel(x, y, Color.black);
                }
                else
                {
                    lOut.SetPixel(x, y, Color.clear);
                }
            }
        }
        lOut.Apply();
        return lOut;
    }
}