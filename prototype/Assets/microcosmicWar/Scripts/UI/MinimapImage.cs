
using UnityEngine;
using System.Collections;


public class MinimapImage : zzInterfaceGUI
{
    //Rect position;
    public Texture image;
    public float textureAlpha = 0.6f;
    public ScaleMode scaleMode = ScaleMode.StretchToFill;
    public bool alphaBlend = true;
    public float imageAspect = 0;

    public override void impGUI(Rect rect)
    {
        //print("zzImageGUI renderGUI");
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, textureAlpha);
        if (image)
            GUI.DrawTexture(rect, image, scaleMode, alphaBlend, imageAspect);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
    }

    public override void setImage(Texture pImage)
    {
        image = pImage;
    }
}