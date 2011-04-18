using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

//提供序列化"zzSerializeAttribute"功能的单实例类
public class zzSerializeObject
{
    //class PropertyControl
    //{
    //    public PropertyControl(MemberInfo pMemberInfo)
    //    {
    //        memberInfo = pMemberInfo;
    //    }

    //    MemberInfo memberInfo;

    //    public void setValue(object obj,object value)
    //    {
    //        switch (memberInfo.MemberType)
    //        {
    //            case MemberTypes.Field:
    //                ((FieldInfo)memberInfo).SetValue(obj, value);
    //                break;

    //            case MemberTypes.Property:
    //                ((PropertyInfo)memberInfo).SetValue(obj,value, null);
    //                break;

    //            default:
    //                break;
    //        }
    //    }

    //    public object getValue()
    //    {
    //        return null;
    //    }
    //}
    static protected zzSerializeObject singletonInstance = new zzSerializeObject();

    public static zzSerializeObject Singleton
    {
        get { return singletonInstance; }
    }

    public Dictionary<System.Type, PropertyInfo[]> typeToSerializeMethod
        = new Dictionary<System.Type,PropertyInfo[]>();

    public Hashtable serializeToTable(object pObject)
    {
        var lList = getSerializeMethod(pObject.GetType());
        var lOut = new Hashtable();
        lOut["#ClassType"] = pObject.GetType().Name;
        var lSerializeString = zzSerializeString.Singleton;
        foreach (var lPropertyInfo in lList)
        {
            var lValue = lPropertyInfo.GetValue(pObject, null);

            //非支持类型,则转为table
            if (!lSerializeString.isSupportedType(lValue.GetType()))
            {
                if (lValue is System.Array)
                {
                    var lArrayList = new ArrayList();
                    foreach (var lElement in lValue as System.Array)
                    {
                        lArrayList.Add(serializeToTable(lElement));
                    }
                    lValue = lArrayList;
                }
                else
                    lValue = serializeToTable(lValue);

            }

            //Debug.Log("Name:" + lPropertyInfo.Name + " " + lValue);
            lOut[lPropertyInfo.Name] = lValue;
        }
        return lOut;
    }

    public void serializeFromTable(object pObject, Hashtable pTable)
    {
        var lList = getSerializeMethod(pObject.GetType());
        var lSerializeString = zzSerializeString.Singleton;
        foreach (var lPropertyInfo in lList)
        {
            var lPropertyType = lPropertyInfo.PropertyType;

            if (pTable.Contains(lPropertyInfo.Name))
            {
                object lTableValue = pTable[lPropertyInfo.Name];
                object lValue=null;
                if(lSerializeString.isSupportedType(lPropertyType))
                    lValue = lTableValue;
                else
                {
                    //非支持的类型: 类或 类的数组
                    if(
                        lPropertyType.IsSubclassOf(typeof(System.Array))
                        && lTableValue is ArrayList)
                    {
                        var lTableArrayList = (ArrayList)lTableValue;
                        var lElementType = lPropertyType.GetElementType();
                        lValue = System.Array.CreateInstance(lElementType, lTableArrayList.Count);
                        var lArray = (System.Array)lValue;
                        int i = 0;
                        foreach (var lTableValueElement in lTableArrayList)
                        {
                            var lElement = System.Activator.CreateInstance(lElementType);
                            serializeFromTable(lElement, (Hashtable)lTableValueElement);
                            lArray.SetValue(lElement, i);
                            ++i;
                        }
                    }
                    else if (lTableValue.GetType() == typeof(Hashtable))
                    {
                        lValue = System.Activator.CreateInstance(lPropertyType);
                        serializeFromTable(lValue, (Hashtable)lTableValue);
                    }
                }
            //    var lTable = lValue as Hashtable;
            //    var lArray = lValue as System.Array;
            //    if (lTable!=null
            //        &&lTable.Contains("#ClassName"))
            //    {

            //    }
                //    else
                if (lValue == null)
                    Debug.LogError("error in " + pObject.GetType().ToString() + "." + lPropertyInfo.Name);
                else
                {
                    //Debug.Log(lPropertyInfo.Name + " " + lValue + ":" + lPropertyInfo.PropertyType);
                    lPropertyInfo.SetValue(pObject, lValue, null);
                }
            }
        }
        
    }

    PropertyInfo[] getSerializeMethod(System.Type lType)
    {
        if(typeToSerializeMethod.ContainsKey(lType))
            return typeToSerializeMethod[lType];
        var lOut = createSerializeMethod(lType);
        typeToSerializeMethod[lType] = lOut;
        return lOut;
    }

    PropertyInfo[] createSerializeMethod(System.Type lType)
    {
        List<PropertyInfo> lOut = new List<PropertyInfo>();
        var lMembers = lType.GetMembers();
        foreach (var lMember in lMembers)
        {
            zzSerializeAttribute[] lAttributes =
                (zzSerializeAttribute[])lMember.GetCustomAttributes(typeof(zzSerializeAttribute), false);
            if (lAttributes.Length > 0)
            {
                lOut.Add((PropertyInfo)lMember);
            }
        }

        return lOut.ToArray();
    }

}