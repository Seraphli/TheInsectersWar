using UnityEngine;
using System.Collections;

public class zzBroadcastServerRequester:MonoBehaviour
{
    public zzBroadcast receiver;
    public delegate void RecieverFunc(zzHostData data);
    RecieverFunc recieverFunc;

    public void addReciever(RecieverFunc pRecieverFunc)
    {
        recieverFunc += pRecieverFunc;
    }

    System.Action beginRecieverFunc;

    public void addBeginRecieverFunc(System.Action pFunc)
    {
        receiver.addBeginRecieverFunc( pFunc);
    }

    System.Action endRecieverFunc;

    public void addEndRecieverFunc(System.Action pFunc)
    {
        receiver.addEndRecieverFunc( pFunc );
    }

    void Awake()
    {
        receiver.addReciever(recieverBroadcast);
    }

    public void recieverBroadcast(string data, string IP)
    {
        Hashtable lHost = (Hashtable)zzSerializeString.Singleton.unpackToData(data);
        zzHostData lHostData = new zzHostData();
        lHostData.gameName = lHost["gameName"] as string;
        lHostData.gameType = lHost["gameType"] as string;
        lHostData.comment = lHost["comment"] as string;
        lHostData.guid = lHost["GUID"] as string;
        lHostData.IP = IP;
        lHostData.port = (int)lHost["port"];
        recieverFunc(lHostData);
    }
}