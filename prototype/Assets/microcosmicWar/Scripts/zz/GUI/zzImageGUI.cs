
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

    public override void impGUI(Rect rect)
    {
        Color lPreColor = GUI.color;
        GUI.color = imageColor;
        if (image)
            GUI.DrawTexture(rect, image, scaleMode, alphaBlend, imageAspect);
        GUI.color = lPreColor;
    }

    public override void setImage(Texture pImage)
    {
        image = pImage;
    }
}