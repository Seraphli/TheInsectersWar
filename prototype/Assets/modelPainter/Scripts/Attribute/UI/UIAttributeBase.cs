using System;
using System.Reflection;
using UnityEngine;


//public class UiAttribute
//{
//    public class UiAttributeContent
//    {
//        public int _verticalDepth = 1;
//        public string _label;

//        public UiAttributeContent verticalDepth(int depth)
//        {
//            _verticalDepth = depth;
//            return this;
//        }

//        public UiAttributeContent label(string pLabel)
//        {
//            _label = pLabel;
//            return this;
//        }
//    }

//    public static UiAttributeContent content
//    {
//        get { return new UiAttributeContent(); }
//    }

//}

public abstract class UiAttributeBase:Attribute
{
    public GUISkin skin;

    public string label;

    protected GUIContent content = new GUIContent();

    public string tooltip
    {
        set { content.tooltip = value; }
        get { return content.tooltip; }
    }

    public Rect windowRect;

    public int verticalDepth = 1;

    public int horizontalDepth = 1;

    //public UiAttribute.UiAttributeContent content
    //{
    //    set
    //    {
    //        label = value._label;
    //        verticalDepth = value._verticalDepth;
    //    }
    //}

    public abstract void impUI(object pObject, MemberInfo pMemberInfo);

    public virtual void clearBuffer()
    {

    }
}