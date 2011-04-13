using UnityEngine;
using System.Reflection;
using System.Linq.Expressions;

public class zzGUIModifierToolbar : zzGUIModifierBase
{
    [System.Serializable]
    public class EnumToToolbarIndex
    {
        public string enumName;
        public int toolbarIndex;
    }

    public zzGUIToolbar toolbar;
    public Component component;
    public string memberName;

    public EnumToToolbarIndex[] enumToToolbarIndex;

    System.Type memberEnumType;

    delegate void EnumSetMethod(int value);

    delegate int EnumGetMethod();

    EnumGetMethod getSelected;
    EnumSetMethod setSelected;

    struct EnumValueToToolbarIndex
    {
        public EnumValueToToolbarIndex(int pEnumValue, int pToolbarIndex)
        {
            enumValue = pEnumValue;
            toolbarIndex = pToolbarIndex;
        }
        public int enumValue;
        public int toolbarIndex;
    }

    EnumValueToToolbarIndex[] enumToolbarList;

    delegate object toEnumObject(System.Type enumType, int value);

    void Start()
    {

        var lMemberProperty = component.GetType().GetProperty(memberName);
        memberEnumType = lMemberProperty.PropertyType;
        enumToolbarList = new EnumValueToToolbarIndex[enumToToolbarIndex.Length];
        for (int i = 0; i < enumToToolbarIndex.Length;++i )
        {
            int lEnumValue = (int)System.Enum.Parse(memberEnumType,
                enumToToolbarIndex[i].enumName);

            enumToolbarList[i] = new EnumValueToToolbarIndex(lEnumValue,
                enumToToolbarIndex[i].toolbarIndex);
        }

        var lInstance = Expression.Constant(component);

        {
            var lGetMethod = lMemberProperty.GetGetMethod();
            var lGetExpression = Expression.Call(lInstance, lGetMethod);
            var lGetMethodLambda = Expression.Lambda(typeof(EnumGetMethod), Expression.Convert(lGetExpression, typeof(int)));
            getSelected = (EnumGetMethod)lGetMethodLambda.Compile();
        }

        {
            var lSetMethod = lMemberProperty.GetSetMethod();
            var lParameter = Expression.Parameter(typeof(int), "value");
            var lTypeConstant = Expression.Constant(lMemberProperty.PropertyType);
            var lConvertedFunc = Expression.Call(((toEnumObject)System.Enum.ToObject).Method,
                lTypeConstant, lParameter);
            var lConverted = Expression.Convert(lConvertedFunc, lMemberProperty.PropertyType);
            var lSetExpression = Expression.Call(lInstance, lSetMethod, lConverted);
            var lSetMethodLambda = Expression.Lambda(typeof(EnumSetMethod), lSetExpression, lParameter);
            setSelected = (EnumSetMethod)lSetMethodLambda.Compile();
        }

        toolbar.addSelectedChangeReceiver(selectionChanged);

    }

    void outDelegateInfo(System.Delegate pDelegate)
    {
        var pMethodInfo = pDelegate.Method;
        string lFunction = pMethodInfo.Name+"(";
        foreach (var lParam in pMethodInfo.GetParameters())
        {
            lFunction = lFunction + ", " + lParam.Name + ":" + lParam.ParameterType;
        }
        lFunction = lFunction + "):" + pMethodInfo.ReturnType.Name;
        print(lFunction);
    }

    bool changed = false;
    int newSelected = 0;
    void selectionChanged(int pSelected)
    {
        changed = true;
        newSelected = pSelected;
    }

    public override void modifierBegin()
    {
        int lSelected = getSelected();
        foreach (var lEnumIndex in enumToolbarList)
        {
            if (lSelected == lEnumIndex.enumValue)
            {
                toolbar.selected = lEnumIndex.toolbarIndex;
                break;
            }
        }
    }

    public override void modifierEnd()
    {
        if (changed)
        {
            changed = false;
            foreach (var lEnumIndex in enumToolbarList)
            {
                if (newSelected == lEnumIndex.toolbarIndex)
                {
                    setSelected(lEnumIndex.enumValue);
                    break;
                }
            }
        }
    }
}