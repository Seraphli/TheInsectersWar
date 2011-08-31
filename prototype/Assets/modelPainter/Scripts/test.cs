using UnityEngine;
using System.Collections;

class test : MonoBehaviour
{
    public delegate int testDelegate();
    public testDelegate testSignal;

    public int i = 0;

    public string sceneSave;

    public SerializeScene serializeScene;

    [ContextMenu("serialize Save Test")]
    public void serializeSaveTest()
    {
        sceneSave =zzSerializeString.Singleton.pack( serializeScene.serializeTo() );
    }

    [ContextMenu("serialize Read Test")]
    public void serializeReadTest()
    {
        serializeScene.serializeFrom(zzSerializeString.Singleton.unpackToData(sceneSave));
    }

    public int testSlot()
    {
        print((++i).ToString());
        return i;
    }

    public int testSlot2()
    {
        print((i+=2).ToString());
        return i;
    }

    public int testSlot3()
    {
        print((i += 3).ToString());
        return i;
    }
    public enum tEnum
    {
        e1=1,
        e2,
        e3,
    }

    public void testEnum(tEnum e)
    {
        print(e);
    }

    void Start()
    {
        //string typeName = tEnum.e2.GetType().FullName;
        //print(typeName);
        
        //this.GetType().GetMethod("testEnum").Invoke(
        //    this,
        //    new object[]{System.Enum.Parse( System.Type.GetType(typeName), tEnum.e2.ToString() )}
        //    );  
      
        //this.GetType().GetMethod("testEnum").Invoke(
        //    this,
        //    new object[] { tEnum.e2.ToString() }
        //    );
    }

    void OnTriggerStay(Collider other)
    {
        print("OnTriggerStay");
    }
}