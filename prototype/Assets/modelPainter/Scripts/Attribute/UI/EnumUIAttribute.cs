using UnityEngine;
using System.Collections;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class EnumUIAttribute: UiAttributeBase
{
    //public EnumUIAttribute(System.Enum[] pEnums,string[] pTexts)
    public EnumUIAttribute(string[] pTexts, int[] pEnums)
    {
        texts = pTexts;
        enumValues = new int[pEnums.Length];
        for (int i = 0; i < pEnums.Length; ++i)
        {
            enumValues[i] = pEnums[i];
        }
    }

    //struct EnumValueToText
    //{
    //    public int enumValue;
    //    public string text; 
    //}

    //EnumValueToText[] enumValueToText = new EnumValueToText[0]{};
    public int[] enumValues = new int[0] { };
    public string[] texts;

    //System.Type enumType;
    //int selected = 0;

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {

        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        int lValue = System.Convert.ToInt32(pPropertyInfo.GetValue(pObject, null));
        int lSelected = 0;
        foreach (var lEnumValue in enumValues)
        {
            if (lEnumValue == lValue)
                break;
            ++lSelected;
        }
        int lNewSelected = GUILayout.Toolbar(lSelected, texts);

        if (lNewSelected != lSelected)
        {
            if (pPropertyInfo.PropertyType.IsSubclassOf(typeof(System.Enum)))
                pPropertyInfo.SetValue(pObject,
                    System.Enum.ToObject(pPropertyInfo.PropertyType, enumValues[lNewSelected]),
                    null);
            else
                pPropertyInfo.SetValue(pObject, enumValues[lNewSelected], null);
        }
    }

}