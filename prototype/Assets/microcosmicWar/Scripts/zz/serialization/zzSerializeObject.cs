using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

//提供序列化"zzSerializeAttribute"功能的单实例类
public class zzSerializeObject
{
    zzSerializeObject()
    {
        classSerialization = new ClassSerialization();
        classSerialization.serializeObject = this;
    }

    public interface SerializationMethod
    {
        zzSerializeObject serializeObject { set; }

        System.Type serializeType { get; }

        object serialize(object pObject);

        object deserialize(System.Type lPropertyType, object lTableValue);
    }
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

    public Dictionary<System.Type, PropertyInfo[]> typeToSerializeInMethod
        = new Dictionary<System.Type, PropertyInfo[]>();

    public Dictionary<System.Type, PropertyInfo[]> typeToSerializeOutMethod
        = new Dictionary<System.Type, PropertyInfo[]>();

    Dictionary<System.Type, SerializationMethod> customMethods
        = new Dictionary<System.Type,SerializationMethod>();

    public void setCustomMethod(SerializationMethod pCustomMethod)
    {
        customMethods[pCustomMethod.serializeType] = pCustomMethod;
    }

    public SerializationMethod getMethod(System.Type pType)
    {
        SerializationMethod lCustomMethod;
        if (customMethods.TryGetValue(pType, out lCustomMethod))
            return lCustomMethod;
        return classSerialization;
    }

    ClassSerialization classSerialization;

    public class ClassSerialization: SerializationMethod
    {
        public zzSerializeObject serializeObject 
        {
            set { _serializeObject = value; } 
        }
        zzSerializeObject _serializeObject;
        public System.Type serializeType { get{return null;} }

        public object serialize(object lValue)
        {
            object lOut;
            if (lValue is System.Array)
            {
                var lArrayList = new ArrayList();
                foreach (var lElement in lValue as System.Array)
                {
                    lArrayList.Add(_serializeObject.serializeToTable(lElement));
                }
                lOut = lArrayList;
            }
            else
                lOut = _serializeObject.serializeToTable(lValue);
            return lOut;
        }

        public object deserialize(System.Type lPropertyType, object lTableValue)
        {
            object lValue = null;
            if (
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
                    _serializeObject.serializeFromTable(lElement, (Hashtable)lTableValueElement);
                    lArray.SetValue(lElement, i);
                    ++i;
                }
            }
            else if (lTableValue.GetType() == typeof(Hashtable))
            {
                lValue = System.Activator.CreateInstance(lPropertyType);
                _serializeObject.serializeFromTable(lValue, (Hashtable)lTableValue);
            }
            return lValue;
        }
    }

    public Hashtable serializeToTable(object pObject)
    {
        var lList = getSerializeOutMethod(pObject.GetType());
        var lOut = new Hashtable();
        lOut["#ClassType"] = pObject.GetType().Name;
        var lSerializeString = zzSerializeString.Singleton;
        foreach (var lPropertyInfo in lList)
        {
            var lValue = lPropertyInfo.GetValue(pObject, null);

            //非支持类型,则转为table
            if (!lSerializeString.isSupportedType(lValue.GetType()))
            {
                lValue = getMethod(lValue.GetType()).serialize(lValue);
            }

            //Debug.Log("Name:" + lPropertyInfo.Name + " " + lValue);
            lOut[lPropertyInfo.Name] = lValue;
        }
        return lOut;
    }

    public void serializeFromTable(object pObject, Hashtable pTable)
    {
        var lList = getSerializeInMethod(pObject.GetType());
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
                    lValue = getMethod(lPropertyType).deserialize(lPropertyType, lTableValue);
                }
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

    PropertyInfo[] getSerializeOutMethod(System.Type lType)
    {
        PropertyInfo[] lOut;
        if (typeToSerializeOutMethod.TryGetValue(lType, out lOut))
            return lOut;
        lOut = createSerializeMethod(lType, false, true);
        typeToSerializeOutMethod[lType] = lOut;
        return lOut;
    }

    PropertyInfo[] getSerializeInMethod(System.Type lType)
    {
        PropertyInfo[] lOut;
        if (typeToSerializeInMethod.TryGetValue(lType, out lOut))
            return typeToSerializeInMethod[lType];
        lOut = createSerializeMethod(lType,true,false);
        typeToSerializeInMethod[lType] = lOut;
        return lOut;
    }

    PropertyInfo[] createSerializeMethod(System.Type lType, bool pNeedSerializeIn, bool pNeedSerializeOut)
    {
        List<PropertyInfo> lOut = new List<PropertyInfo>();
        var lMembers = lType.GetMembers();
        foreach (var lMember in lMembers)
        {
            zzSerializeAttribute[] lAttributes =
                (zzSerializeAttribute[])lMember.GetCustomAttributes(typeof(zzSerializeAttribute), true);
            if (lAttributes.Length > 0
                && ( pNeedSerializeIn?lAttributes[0].serializeIn:true)
                && ( pNeedSerializeOut?lAttributes[0].serializeOut:true))
            {
                lOut.Add((PropertyInfo)lMember);
            }
        }

        return lOut.ToArray();
    }

}