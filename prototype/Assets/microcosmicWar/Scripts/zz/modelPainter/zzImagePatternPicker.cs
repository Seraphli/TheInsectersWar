using UnityEngine;

public class zzImagePatternPicker
{

    public static Texture2D pick(int[,] pPatternMark,int pPickPatternID,
        Texture2D pSource, zzPointBounds pBounds, zzPoint pOutSize)
    {
        Texture2D lOut = new Texture2D(pOutSize.x, pOutSize.y, TextureFormat.ARGB32, false);
        var lMin = pBounds.min;
        var lMax = pBounds.max;
        var lDrawOffset = -lMin;
        for (int lY = lMin.y; lY < lMax.y; ++lY)
        {
            var lDrawedPointY = lY + lDrawOffset.y;
            for (int lX = lMin.x; lX < lMax.x; ++lX)
            {
                var lColor = pPatternMark[lX, lY] == pPickPatternID
                    ? pSource.GetPixel(lX, lY) : Color.clear;

                lOut.SetPixel(lX + lDrawOffset.x, lDrawedPointY, lColor);
            }
            for (int i = lMax.x+lDrawOffset.x; i < lOut.width; ++i)
            {
                lOut.SetPixel(i, lDrawedPointY, Color.clear);
            }
        }
        for (int lY = lMax.y + lDrawOffset.y; lY < lOut.height; ++lY)
        {
            for (int lX = 0; lX < lOut.width; ++lX)
                lOut.SetPixel(lX, lY, Color.clear);
        }
        lOut.Apply();
        return lOut;
    }

}