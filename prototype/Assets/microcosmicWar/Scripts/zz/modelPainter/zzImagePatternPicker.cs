using UnityEngine;

public class zzImagePatternPicker
{

    public static Texture2D pick(int[,] pPatternMark,int pPickPatternID,
        Texture2D pSource, zzPointBounds pBounds, zzPoint pOutSize)
    {
        Texture2D lOut = new Texture2D(pOutSize.x, pOutSize.y, TextureFormat.ARGB32, false);
        var lMin = pBounds.min;
        var lMax = pBounds.max;
        var lDrawOffset = zzPoint.zero - lMin;
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

    public static Texture2D pick(zzActiveChart pActiveChart, Texture2D pSource,
        zzPoint pDrawOffset, zzPoint pOutSize, zzPoint pMinPoint)
    {
        Texture2D lOut = new Texture2D(pOutSize.x, pOutSize.y, TextureFormat.ARGB32, false);
        for (int lY = 0; lY < lOut.height;++lY )
        {
            for (int lX = 0; lX < lOut.width;++lX )
            {
                lOut.SetPixel(lX, lY, Color.clear);
            }
        }
        zzActiveChart lPickedChart = new zzActiveChart(pActiveChart.width, pActiveChart.height);
        pick(pActiveChart, pSource, lOut, pMinPoint, lPickedChart, pDrawOffset);
        lOut.Apply();
        return lOut;
    }

    public static void pick(zzActiveChart pActiveChart, Texture2D pSource,
        Texture2D pToDraw, zzPoint pBeginPoint, zzActiveChart pPickedChart,
        zzPoint pDrawOffset)
    {
        if (
            pBeginPoint.x < pSource.width
            && pBeginPoint.y < pSource.height
            && pActiveChart.isActive(pBeginPoint)
            && !pPickedChart.isActive(pBeginPoint))
        {
            var lDrawedPoint = pBeginPoint + pDrawOffset;
            pToDraw.SetPixel(lDrawedPoint.x, lDrawedPoint.y,
                pSource.GetPixel(pBeginPoint.x, pBeginPoint.y)
                );
            pPickedChart.setActive(pBeginPoint, true);

            //pick(pActiveChart, pSource, pToDraw,
            //    pBeginPoint + new zzPoint(-1, 0), pPickedChart,
            //    pDrawOffset);

            pick(pActiveChart, pSource, pToDraw,
                pBeginPoint + new zzPoint(1, 0), pPickedChart,
                pDrawOffset);

            pick(pActiveChart, pSource, pToDraw,
                pBeginPoint + new zzPoint(0, 1), pPickedChart,
                pDrawOffset);
        }
    }
}