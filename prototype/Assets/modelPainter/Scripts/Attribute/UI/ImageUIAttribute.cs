using UnityEngine;
using System.Collections;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class ImageUIAttribute : UiAttributeBase
{
    //public ImageUIAttribute(){}

    //public ImageUIAttribute(UiAttribute.UiAttributeContent pContent)
    //{
    //    content = pContent;
    //}

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        var lImage = pPropertyInfo.GetValue(pObject, null) as Texture;
        if (lImage)
        {
            var lStyle = skin.FindStyle("ImageUI");
            float lWidth = windowRect.width - 20f;
            float lHeight = (float)lImage.height / (float)lImage.width * lWidth;
            if (lStyle == null)
                GUILayout.Box(lImage, GUILayout.Width(lWidth), GUILayout.Height(lHeight));
            else
                GUILayout.Box(lImage, lStyle, GUILayout.Width(lWidth), GUILayout.Height(lHeight));
            //Debug.Log("windowRect:" + windowRect);
            //Debug.Log("lImage.width:" + lImage.width);
            //Debug.Log("lImage.height:" + lImage.height);
            //Debug.Log("lWidth:" + lWidth);
            //Debug.Log("lHeight:" + lHeight);
        }
    }
}