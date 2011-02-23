using UnityEngine;
using System.Collections;

public class zzGUICombineImage : zzInterfaceGUI
{
    public Texture[] images = new Texture[0]{};
    public int xCount = 2;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;
    public Color imageColor = Color.white;

    public override void impGUI(Rect rect)
    {
        Color lPreColor = GUI.color;
        GUI.color = imageColor;
        float lWidth = rect.width;
        float lHeigth = rect.height;
        float lXPos = rect.x;
        float lYPos = rect.y;
        int lXCount = 0;
        foreach (var lImage in images)
        {
            var lRect = new Rect(lXPos, lYPos, lWidth, lHeigth);
            GUI.DrawTexture(lRect, lImage, scaleMode, alphaBlend, imageAspect);
            lXPos += lWidth;
            ++lXCount;
            if(lXCount>=xCount)
            {
                lXCount = 0;
                lXPos = 0f;
                lYPos += lHeigth;
            }
        }
        GUI.color = lPreColor;
    }

}