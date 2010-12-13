
using UnityEngine;
using System.Collections;

public class zzGameObjectInit : MonoBehaviour
{
    //用于统一单机和联网的.物体或工具的 使用 和初始化接口 .使用hashtable初始化

    public Component componentToInit;

    public void init(Hashtable p)
    {
        //componentToInit.SendMessage("init",p);
        if (zzCreatorUtility.isHost())
        {
            //zzCreatorUtility.sendMessag2Two(gameObject,"impInit","initedFromRPC",gameObject.layer);
            if (Network.peerType == NetworkPeerType.Disconnected)
                impInit(p);
            else
            {
                impInit(p);
                networkView.RPC("initedFromRPC",
                    RPCMode.Others,
                    zzSerializeString.getSingleton().pack(p));
            }
        }
    }

    [RPC]
    public void initedFromRPC(string p)
    {
        impInit(zzSerializeString.getSingleton().unpackToData(p) as Hashtable);
    }

    public void impInit(Hashtable p)
    {
        //object objectInited ;//= componentToInit;
        componentToInit.GetType().GetMethod("init").Invoke(componentToInit,new object[]{ p} );
        //object.init(p);
    }
}