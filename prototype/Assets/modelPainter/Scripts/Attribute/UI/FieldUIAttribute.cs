using UnityEngine;
using System.Collections;
using System.Reflection;


[System.AttributeUsage(System.AttributeTargets.Property)]
public class FieldUIAttribute:UiAttributeBase
{

    public string stringBuffer = null;

    public override void clearBuffer()
    {
        stringBuffer = null;
    }

    public FieldUIAttribute(string labelName)
    {
        label = labelName;
    }

    string TextField(string text,int maxLength)
    {
        var lStyle = skin.FindStyle("FieldUI");
        if (lStyle!=null)
            return GUILayout.TextField(text, maxLength, lStyle);
        return GUILayout.TextField(text, maxLength);
    }

    //public bool isFloat(string pText)
    //{
    //    bool lOut = false;
    //    try
    //    {
    //        float.Parse(pText);
    //        lOut = true;
    //    }
    //    catch{}
    //    return lOut;
    //}

    public void intField(object pObject, PropertyInfo pPropertyInfo)
    {
        int lPreValue = (int)pPropertyInfo.GetValue(pObject, null);

        //string lPreText;
        //if (stringBuffer != null)
        //{
        //    lPreText = stringBuffer;
        //}
        //else
        //{
        //    lPreText = lPreValue.ToString();
        //}
        int lNewValue;
        string lNewText = TextField(lPreValue.ToString(), 8);
        if (lNewText.Length == 0)
            lNewValue = 0;
        else
            int.TryParse(lNewText, out lNewValue);


        if (lNewValue != lPreValue)
            pPropertyInfo.SetValue(pObject, lNewValue, null);
        //if (lPreText != lNewText)
        //{
        //    try
        //    {
        //        pPropertyInfo.SetValue(pObject, System.Int32.Parse(lNewText), null);
        //        stringBuffer = null;
        //    }
        //    catch (System.Exception e)
        //    {
        //        stringBuffer = lNewText;
        //    }
        //}
    }

    public void floatField(object pObject, PropertyInfo pPropertyInfo)
    {
        float lPreValue = (float)pPropertyInfo.GetValue(pObject, null);

        string lPreText = zzGUIUtilities.toString(lPreValue);
        //if(stringBuffer!=null)
        //{
        //    lPreText = stringBuffer;
        //}
        //else
        //{
        //    lPreText = lPreValue.ToString();
        //}
        float lNewValue;
        string lNewText = TextField(lPreText, 8);
        //if (lNewText.Length == 0)
        //    lNewValue = 0f;
        //else
        //{
        //    var lNums = lNewText.Split('.');
        //    if (lNums.Length>2)
        //    {
        //        lNewText = lNums[0] + "." + lNums[1];
        //        for (int i = 2; i<lNums.Length;++i )
        //        {
        //            lNewText += lNums[i];
        //        }
        //    }
        //    float.TryParse(lNewText, out lNewValue);
        //}


        if (zzGUIUtilities.stringToFloat(lNewText, out lNewValue)
            &&lNewValue != lPreValue)
            pPropertyInfo.SetValue(pObject, lNewValue, null);
        //if(lPreText!=lNewText)
        //{
        //    try
        //    {
        //        pPropertyInfo.SetValue(pObject, float.Parse(lNewText), null);
        //        stringBuffer = null;
        //    }
        //    catch (System.Exception e)
        //    {
        //        stringBuffer = lNewText;
        //    }
        //}

    }

    public void boolField(object pObject, PropertyInfo pPropertyInfo)
    {
        bool lValue = (bool)pPropertyInfo.GetValue(pObject, null);
        content.text="";
        bool lNewValue = GUILayout.Toggle(lValue, content);
        if(lValue!=lNewValue)
            pPropertyInfo.SetValue(pObject, lNewValue, null);
    }

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        //float lValue = (float)pPropertyInfo.GetValue(pObject, null);
        GUILayout.Label(label);
        var lPropertyType = pPropertyInfo.PropertyType;
        if (lPropertyType == typeof(float))
            floatField(pObject, pPropertyInfo);
        else if (lPropertyType == typeof(bool))
            boolField(pObject, pPropertyInfo);
        else if (lPropertyType == typeof(int))
            intField(pObject, pPropertyInfo);
        else
            Debug.LogError("no ui in the type ");
    }
}