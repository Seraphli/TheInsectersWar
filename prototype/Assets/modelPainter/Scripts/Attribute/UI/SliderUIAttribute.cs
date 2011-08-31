using UnityEngine;
using System.Collections;
using System.Reflection;


[System.AttributeUsage(System.AttributeTargets.Property)]
public class SliderUIAttribute : UiAttributeBase
{
    float leftValue;
    float rightValue;

    public SliderUIAttribute(float pLeftValue,float pRightValue)
    {
        leftValue = pLeftValue;
        rightValue = pRightValue;
    }

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        float lValue = (float) pPropertyInfo.GetValue(pObject, null);
        var lNewValue = GUILayout.HorizontalSlider(lValue, leftValue, rightValue);
        if (lNewValue != lValue)
            pPropertyInfo.SetValue(pObject, lNewValue, null);
    }
}