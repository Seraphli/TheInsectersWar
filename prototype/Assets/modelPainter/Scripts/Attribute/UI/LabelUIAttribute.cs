using UnityEngine;
using System.Collections;
using System.Reflection;


[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class LabelUIAttribute : UiAttributeBase
{
    public LabelUIAttribute(){}

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        if(pMemberInfo is PropertyInfo)
        {
            GUILayout.Label(
                ((PropertyInfo)pMemberInfo).GetValue(pObject,null).ToString());
        }
        else if (pMemberInfo is FieldInfo)
        {
            GUILayout.Label(
                ((FieldInfo)pMemberInfo).GetValue(pObject).ToString());
        }
        else
            Debug.LogError("no ui in the type ");
    }

}