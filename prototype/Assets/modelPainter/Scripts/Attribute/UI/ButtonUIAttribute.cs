using UnityEngine;
using System.Collections;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class ButtonUIAttribute : UiAttributeBase
{
    public ButtonUIAttribute(string labelName)
    {
        label = labelName;
    }

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        var lStyle = skin.FindStyle("ButtonUI");
        bool lIsClick = false;
        if (lStyle == null)
            lIsClick = GUILayout.Button(label);
        else
            lIsClick = GUILayout.Button(label, lStyle);
        if(lIsClick)
        {
            MethodInfo pMethodInfo = (MethodInfo)pMemberInfo;
            pMethodInfo.Invoke(pObject, null);

        }
    }
}