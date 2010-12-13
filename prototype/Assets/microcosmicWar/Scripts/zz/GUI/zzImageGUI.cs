
using UnityEngine;
using System.Collections;


public class zzImageGUI : zzInterfaceGUI
{
    //Rect position;
    public Texture image;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;

    public override void impGUI(Rect rect)
    {
        //print("zzImageGUI renderGUI");
        if (image)
            GUI.DrawTexture(rect, image, scaleMode, alphaBlend, imageAspect);
    }

    public override void setImage(Texture pImage)
    {
        image = pImage;
    }
}