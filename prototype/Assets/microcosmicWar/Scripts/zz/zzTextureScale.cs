using UnityEngine;
//using System.Threading;

//http://www.unifycommunity.com/wiki/index.php?title=TextureScale
public class TextureScale : MonoBehaviour
{
    class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }


    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;

    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / (float)newWidth / (tex.width - 1);
            ratioY = 1.0f / (float)newHeight / (tex.height - 1);
        }
        else
        {
            ratioX = (float)(tex.width) / newWidth;
            ratioY = (float)(tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        //var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        var cores = 1;
        var slice = newHeight / cores;
        finishCount = 0;

        //if (cores > 1)
        //{
        //    int i = 0;
        //    for ( i = 0; i < cores - 1; i++)
        //    {
        //        var lSubThreadData = new ThreadData(slice * i, slice * (i + 1));
        //        var thread = useBilinear ?
        //            new Thread(() => BilinearScale(lSubThreadData))
        //            : new Thread(() => PointScale(lSubThreadData));
        //    }
        //    var lThreadData = new ThreadData(slice * i, newHeight);
        //    if (useBilinear)
        //    {
        //        BilinearScale(lThreadData);
        //    }
        //    else
        //    {
        //        PointScale(lThreadData);
        //    }
        //    while (finishCount < cores) ;
        //}
        //else
        {
            var lThreadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(lThreadData);
            }
            else
            {
                PointScale(lThreadData);
            }
        }

        tex.Resize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();
    }

    private static void BilinearScale(ThreadData threadData)
    {
        for (int y = threadData.start; y < threadData.end; y++)
        {
            var yFloor = Mathf.Floor(y * ratioY);
            var y1 = yFloor * w;
            var y2 = (yFloor + 1) * w;
            var yw = y * w2;

            for (int x = 0; x < w2; x++)
            {
                var xFloor = Mathf.Floor(x * ratioX);
                var xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[(int)(y1 + xFloor)], texColors[(int)(y1 + xFloor + 1)], xLerp),
                                                       ColorLerpUnclamped(texColors[(int)(y2 + xFloor)], texColors[(int)(y2 + xFloor + 1)], xLerp),
                                                       y * ratioY - yFloor);
            }
        }

        finishCount++;
    }

    private static void PointScale(ThreadData threadData)
    {
        for (int y = threadData.start; y < threadData.end; y++)
        {
            var thisY = (int)(ratioY * y) * w;
            var yw = y * w2;
            for (int x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[thisY + (int)(ratioX * x)];
            }
        }

        finishCount++;
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }
}