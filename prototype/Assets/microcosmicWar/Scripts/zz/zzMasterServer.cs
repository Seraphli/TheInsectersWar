using UnityEngine;
using System.Collections;

public class zzMasterServer : MonoBehaviour
{
    public string masterServerURL;
    public string gameType;
    public string gameName;
    public string comment;

    public int remotePort = 2088;

    public int maxRetries = 5;

    public string externalIP;
    public int externalPort;

    public string IP;
    public int port;
    // Use this for initialization

    ArrayList hostList = new ArrayList();

    public int maxNatRetries = 5;

    [SerializeField]
    bool failedRequest = false;

    private bool _autoRequestHostList = true;

    public bool autoRequestHostList
    {
        get { return _autoRequestHostList; }
        set 
        {
            if (_autoRequestHostList == value)
                return;
            _autoRequestHostList = value;

            if (_autoRequestHostList)
                StartCoroutine(_RequestHostList());
        }
    }

    public bool inHosting = false;

    public float autoRequestInterval = 7f;
    public float autoUpdateSelf = 7f;


    public delegate void RecieverFunc(string data, string IP);
    RecieverFunc recieverFunc;

    public void addReciever(RecieverFunc pRecieverFunc)
    {
        recieverFunc += pRecieverFunc;
    }

    zzUtilities.voidFunction beginRecieverFunc = zzUtilities.nullFunction;

    public void addBeginRecieverFunc(zzUtilities.voidFunction pFunc)
    {
        beginRecieverFunc += pFunc;
    }

    void Start()
    {
        if (beginRecieverFunc == null)
            beginRecieverFunc = zzUtilities.nullFunction;
    }

    //------------------------host begin------------------------
    zzTimerCoroutine updateSelfHostTimer;

    public void RegisterHost()
    {
        if (!Network.isServer || requestHostTimer)
            return;
        inHosting = true;
        StartCoroutine(_RegisterHost());
    }

    IEnumerator _ImpHost()
    {
        if (inHosting && Network.isServer)
        {
            yield return updateSelfHost();
            if (failedRequest)
                yield return _SentRegisterInfo();
        }
        else
        {
            updateSelfHostTimer.endTimer();
            updateSelfHostTimer = null;
        }
    }

    IEnumerator _SentRegisterInfo()
    {
        do 
        {
            //tudo:yield return 
            yield return beginHost();
            //执行到成功为止
        } while (inHosting && Network.isServer && failedRequest);

    }

    IEnumerator _RegisterHost()
    {
        yield return _SentRegisterInfo();
        if (inHosting)
        {
            yield return new WaitForSeconds(autoUpdateSelf);
            updateSelfHostTimer = gameObject.AddComponent<zzTimerCoroutine>();
            updateSelfHostTimer.setInterval(autoUpdateSelf);
            updateSelfHostTimer.setImpFunction(_ImpHost);

        }
    }

    void UnregisterHost()
    {
        inHosting = false;
    }

    IEnumerator OnApplicationQuit()
    {
        if (inHosting)
            yield return _UnregisterHost();
    }

    IEnumerator _UnregisterHost()
    {
        var url = masterServerURL + "unregisterhost";
        url += "?GUID=" + Network.player.guid;

        url += "&gameType=" + WWW.EscapeURL(gameType);
        url += "&gameName=" + WWW.EscapeURL(gameName);

        yield return sentUrlInfo(url);


    }

    IEnumerator updateSelfHost()
    {
        var url = masterServerURL + "updatehost";
        url += "?GUID=" + Network.player.guid;

        url += "&gameType=" + WWW.EscapeURL(gameType);
        url += "&gameName=" + WWW.EscapeURL(gameName);

        yield return sentUrlInfo(url);

    }

    IEnumerator beginHost()
    {
        //Network.InitializeServer(32, remotePort, true);
        failedRequest = false;
        yield return new WaitForSeconds(1f);
        int lNatRetries = 0;
        while (
            inHosting
            && Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS"
            && lNatRetries < maxNatRetries)
        {
            yield return new WaitForSeconds(1f);
        }

        if (Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS")
        {
            Debug.Log("externalIP == UNASSIGNED_SYSTEM_ADDRESS");
            failedRequest = true;
        }
        else
        {
            var url = masterServerURL + "registerhost";

            url += "?IP=" + Network.player.externalIP;
            url += "&port=" + Network.player.externalPort;
            url += "&GUID=" + Network.player.guid;

            url += "&gameType=" + WWW.EscapeURL(gameType);
            url += "&gameName=" + WWW.EscapeURL(gameName);

            yield return sentUrlInfo(url);

        }

    }

    //------------------------host end--------------------------

    zzTimerCoroutine requestHostTimer;

    IEnumerator getHostList()
    {
        var url = masterServerURL + "queryhost";

        url += "&gameType=" + WWW.EscapeURL(gameType);
        url += "&gameName=" + WWW.EscapeURL(gameName);

        failedRequest = false;
        var www = new WWW(url);
        yield return www;

        ArrayList lHostList; 
        int retries = 0;
        while (
            (www.error != null || (lHostList = unpackHostList(www.text)) == null) //当有错误时
            && retries < maxRetries)
        {
            retries++;
            www = new WWW(url);
            yield return www;
        }

        if (www.error != null
            || lHostList==null
            ||(lHostList = unpackHostList(www.text)) == null)
        {
            failedRequest = true;
            hostList = new ArrayList();
        }
        else
            hostList = lHostList;
    }

    IEnumerator _RequestHostList()
    {
        //while (_autoRequestHostList)
        //{
            yield return getHostList();
            foreach (Hashtable lHost in hostList)
            {
                //Todo:
            }
        //}
    }

    ArrayList unpackHostList(string pData)
    {
        try
        {
            return zzSerializeString.Singleton.unpackToData(pData) as ArrayList;
        }
        catch (System.Exception e)
        {
            return null;
        }
    }

    IEnumerator sentUrlInfo(string pUrl)
    {
        failedRequest = false;
        var www = new WWW(pUrl);
        yield return www;

        //int retries = 0;
        //while (
        //    (www.error != null || www.text != "succeeded")//当有错误时
        //    && retries < maxRetries)
        //{
        //    retries++;
        //    www = new WWW(url);
        //    yield return www;
        //}
        if (www.error != null || www.text != "succeeded")
            failedRequest = true;

    }


}