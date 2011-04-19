using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class zzCopyProperty:MonoBehaviour
{
    public MonoBehaviour[] toCopy = new MonoBehaviour[0]{};

    public string attributeName;

    PropertyCopier[] propertyCopiers;

    public void paste(GameObject pObject)
    {
        foreach (var lPropertyCopier in propertyCopiers)
        {
            lPropertyCopier.paste(pObject);
        }
    }

    class PropertyCopier
    {
        public PropertyCopier(MonoBehaviour pPrototype, System.Type pAttribute)
        {
            prototype = pPrototype;
            prototypeType = pPrototype.GetType();

            List<PropertyInfo> lOut = new List<PropertyInfo>();
            var lMembers =prototypeType.GetMembers();
            foreach (var lMember in lMembers)
            {
                var lAttributes =
                    lMember.GetCustomAttributes(pAttribute, false);
                if (lAttributes.Length > 0)
                {
                    lOut.Add((PropertyInfo)lMember);
                }
            }
            copyList = lOut.ToArray();
        }

        public void paste(GameObject pObject)
        {
            var lPasted = (MonoBehaviour)pObject.GetComponent(prototypeType);
            if (lPasted)
                paste(lPasted);
        }

        public void paste(MonoBehaviour pPasted)
        {
            foreach (var lPropertyInfo in copyList)
            {
                lPropertyInfo.SetValue(pPasted,
                    lPropertyInfo.GetValue(prototype, null), null);
            }
        }

        MonoBehaviour prototype;
        System.Type prototypeType;
        PropertyInfo[] copyList;
    }


    void Awake()
    {
        var lAttributeType = System.Type.GetType(attributeName);
        propertyCopiers = new PropertyCopier[toCopy.Length];
        int i = 0;
        foreach (var lPropertyScript in toCopy)
        {
            propertyCopiers[i]
                = new PropertyCopier(lPropertyScript, lAttributeType);
            ++i;
        }
    }
}