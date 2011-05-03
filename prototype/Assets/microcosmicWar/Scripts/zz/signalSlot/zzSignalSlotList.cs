using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

public class zzSignalSlotList:MonoBehaviour
{
    [System.Serializable]
    public class SlotInfo
    {
        public string description = "";
        public UnityEngine.Object slotComponent;
        public string slotMethodName = "slotMethodName";

    }
    //用于注释 便于观看
    public string description = "";

    public UnityEngine.Object signalComponent;

    //delegate or event
    public string signalMethodName = "signalMethodName";

    public bool destroyAfterAwake = true;

    public SlotInfo[] slotList = new SlotInfo[0]{};
    //just for show "enabled" in editor
    void Start() { }

    void Awake()
    {
        if (!enabled)
        {
            if (destroyAfterAwake)
                Destroy(this);
            return;
        }

        MemberInfo lSignalMemberInfo = zzSignalSlot.getSignalMember(signalComponent, signalMethodName);
        if (lSignalMemberInfo == null)
        {
            Debug.LogError(gameObject.name + ":There is not name in method,or it is not public." 
                + signalComponent + "." + signalMethodName);
            return;
        }
        Type lSignalDelegateType = zzSignalSlot.getSignalDelegate(lSignalMemberInfo);

        Type ReturnType;
        Type[] ParameterTypes;

        zzSignalSlot.getSignalMethod(lSignalDelegateType,
            out ReturnType, out ParameterTypes);
        foreach (var lSlot in slotList)
        {
            MethodInfo lSlotMethod = lSlot.slotComponent.GetType()
                .GetMethod(lSlot.slotMethodName, ParameterTypes);


            if (lSlotMethod == null ||
                    !(lSlotMethod.ReturnType == ReturnType
                    || lSlotMethod.ReturnType.IsSubclassOf(ReturnType))
                )
            {
                Debug.LogError(name+" "+lSlot.slotComponent+"."+lSlot.slotMethodName
                    +"Slot Method isn't fit Signal,or it is not public");
                return;
            }
            var lSlotDelegate = System.Delegate.CreateDelegate(
                 lSignalDelegateType, lSlot.slotComponent, lSlotMethod);

            zzSignalSlot.linkSignalToSlot(signalComponent, lSignalMemberInfo, lSlotDelegate);

        }

        if (destroyAfterAwake)
            Destroy(this);
    }
}