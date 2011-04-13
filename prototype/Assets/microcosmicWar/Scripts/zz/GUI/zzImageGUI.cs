
using UnityEngine;
using System.Collections;


public class zzImageGUI : zzInterfaceGUI
{
    //Rect position;
    public Texture image;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;
    public Color imageColor = Color.white;
    public bool changeColor = true;

    public override void impGUI(Rect rect)
    {
        if (image)
        {
            if (changeColor)
            {
                Color lPreColor = GUI.color;
                GUI.color = imageColor;
                GUI.DrawTexture(rect, image, scaleMode, alphaBlend, imageAspect);
                GUI.color = lPreColor;
            }
            else
                GUI.DrawTexture(rect, image, scaleMode, alphaBlend, imageAspect);
        }
    }

    public override void setImage(Texture pImage)
    {
        image = pImage;
    }
}