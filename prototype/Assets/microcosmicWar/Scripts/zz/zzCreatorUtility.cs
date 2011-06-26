using UnityEngine;
using System.Collections;

public class zzCreatorUtility : MonoBehaviour
{

    //统一单机与联网的创建与销毁接口


    abstract class IzzGenericCreator
    {
        public abstract GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, int group);
        //{
        //    Debug.LogError("IzzGenericCreator.Instantiate");
        //}

        public abstract void Destroy(GameObject gameObject);
        //{
        //    Debug.LogError("IzzGenericCreator.Destroy");
        //}

        public abstract bool isMine(NetworkView networkView);
        //{
        //    Debug.LogError("IzzGenericCreator.isMine");
        //    return true;
        //}

        public abstract void sendMessage(GameObject gameObject, string methodName);
        //{
        //    Debug.LogError("IzzGenericCreator.sendMessage");
        //}

        public abstract void sendMessage(GameObject gameObject, string methodName, object value);
        //{
        //    Debug.LogError("IzzGenericCreator.sendMessage");
        //}

    }

    class ZzNetCreator : IzzGenericCreator
    {
        public override GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, int group)
        {
            //Debug.Log("Network.Instantiate"+prefab.name);
            return Network.Instantiate(prefab, position, rotation, group) as GameObject;
        }

        public override void Destroy(GameObject gameObject)
        {
            Network.Destroy(gameObject);
        }
        public override bool isMine(NetworkView networkView)
        {
            return networkView.isMine;
        }

        public override void sendMessage(GameObject gameObject, string methodName)
        {
            gameObject.networkView.RPC(methodName, RPCMode.All);
        }

        public override void sendMessage(GameObject gameObject, string methodName, object value)
        {
            gameObject.networkView.RPC(methodName,
                RPCMode.All,
                value);
        }
    }

    class ZzSingleCreator : IzzGenericCreator
    {
        public override GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, int group)
        {
            //Debug.Log("ZzSingleCreator.Instantiate");
            return GameObject.Instantiate(prefab, position, rotation) as GameObject;
        }

        public override void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }

        public override bool isMine(NetworkView networkView)
        {
            return true;
        }

        public override void sendMessage(GameObject gameObject, string methodName)
        {
            gameObject.SendMessage(methodName);
        }

        public override void sendMessage(GameObject gameObject, string methodName, object value)
        {
            gameObject.SendMessage(methodName, value);
        }
    }

    //默认为单机,省去初始化操作
    static IzzGenericCreator zzGenericCreator = new ZzSingleCreator();

    static bool host = true;

    public static void resetCreator(zzCreatorUtility pCreator)
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            //print("Network.peerType ==NetworkPeerType.Disconnected");
            zzGenericCreator = new ZzSingleCreator();
            host = true;
        }
        else
        {
            //print("Network.peerType !=NetworkPeerType.Disconnected");
            pCreator.inNetwork = true;
            zzGenericCreator = new ZzNetCreator();
            if (Network.isServer)
            {
                pCreator.isServer = true;
                host = true;
            }
            else
                host = false;
        }
        //Debug.Log(zzGenericCreator.isMine(null));
        //zzGenericCreator
        //print(host);
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, int group)
    {
        //Debug.Log("Instantiate");
        //Debug.Log(zzGenericCreator);
        return zzGenericCreator.Instantiate(prefab, position, rotation, group);
    }
    public static void Destroy(GameObject gameObject)
    {
        zzGenericCreator.Destroy(gameObject);
    }

    //单机时与作为服务器时返回真
    public static bool isHost()
    {
        return host;
    }

    public static bool isMine(NetworkView networkView)
    {
        return zzGenericCreator.isMine(networkView);
    }

    //在单机中避免RPC
    public static void sendMessage(GameObject gameObject, string methodName, object value)
    {
        zzGenericCreator.sendMessage(gameObject, methodName, value);
    }

    public static void sendMessage(GameObject gameObject, string methodName)
    {
        zzGenericCreator.sendMessage(gameObject, methodName);
    }

    //联网时使用netMethodName,单机时 使用 singleMethodName
    public static void sendMessag2Two(GameObject gameObject, string singleMethodName, string netMethodName, Object value)
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
            gameObject.SendMessage(singleMethodName, value);
        else
            gameObject.networkView.RPC(netMethodName,
                RPCMode.All,
                value);
    }

    void Awake()
    {
        //Debug.LogError("zzCreatorUtility.Awake");
        resetCreator(this);
    }

    public bool inNetwork = false;
    public bool isServer = false;
    //void  Update (){
    //}
}