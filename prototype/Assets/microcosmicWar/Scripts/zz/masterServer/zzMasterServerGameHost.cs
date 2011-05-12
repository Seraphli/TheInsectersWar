using UnityEngine;
using System.Collections;

public class zzMasterServerGameHost : zzNetworkHost
{
    public string masterServerURL;
    zzHostData hostData;
    [SerializeField]
    string _gameType;
    [SerializeField]
    string _gameName;
    [SerializeField]
    string _comment;

    public int maxRetries = 5;

    public string externalIP;
    public int externalPort;

    public int maxNatRetry = 5;

    [SerializeField]
    bool failedRequest = false;

    public bool inHosting = false;

    public float autoUpdateSelf = 7f;

    System.Action registerSucceedEvent;

    System.Action registerFailEvent;

    public zzNatStun stun;

    public override void addRegisterSucceedReceiver(System.Action pReceiver)
    {
        registerSucceedEvent += pReceiver;
    }

    public override void addRegisterFailReceiver(System.Action pReceiver) 
    {
        registerFailEvent += pReceiver;
    }

    void stunSucceedRespone()
    {
        hostData.IP = stun.publicIP;
        hostData.port = stun.publicPort;
        registerSucceedEvent();
        RegisterHost();
    }

    void stunFailRespone()
    {
        registerFailEvent();
    }

    void Start()
    {
        if (registerSucceedEvent == null)
            registerSucceedEvent = zzUtilities.nullFunction;
        if (registerFailEvent == null)
            registerFailEvent = zzUtilities.nullFunction;
        stun.addSucceedEventReceiver(stunSucceedRespone);
        stun.addFailEventReceiver(stunFailRespone);
    }

    //------------------------host begin------------------------
    zzTimerCoroutine updateSelfHostTimer;

    public override void RegisterHost(zzHostInfo pHostInfo)
    {
        if (stun.inProcessing)
            return;
        _gameType = pHostInfo.gameType;
        hostData.gameType = pHostInfo.gameType;

        _gameName = pHostInfo.gameName;
        hostData.gameName = pHostInfo.gameName;

        _comment = pHostInfo.comment;
        hostData.comment = pHostInfo.comment;

        hostData.guid = pHostInfo.guid;
        stun.query(pHostInfo);
    }

    public void RegisterHost()
    {
        inHosting = true;
        if (!Network.isServer || updateSelfHostTimer)
            return;
        StartCoroutine(_RegisterHost());
    }

    IEnumerator _ImpHost()
    {
        if (inHosting && Network.isServer)
        {
            yield return StartCoroutine(updateSelfHost());
            if (failedRequest)
                yield return StartCoroutine( _SentRegisterInfo());
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
            yield return StartCoroutine(beginHost());
            //执行到成功为止
        } while (inHosting && Network.isServer && failedRequest);

    }

    IEnumerator _RegisterHost()
    {
        yield return StartCoroutine( _SentRegisterInfo());
        if (inHosting)
        {
            yield return new WaitForSeconds(autoUpdateSelf);
            updateSelfHostTimer = gameObject.AddComponent<zzTimerCoroutine>();
            updateSelfHostTimer.setInterval(autoUpdateSelf);
            updateSelfHostTimer.setImpFunction(_ImpHost);

        }
    }

    public override void UnregisterHost()
    {
        inHosting = false;
        stun.stunEnd();
        StartCoroutine(_UnregisterHost());
    }

    IEnumerator OnApplicationQuit()
    {
        if (inHosting)
            yield return StartCoroutine(_UnregisterHost());
    }

    IEnumerator _UnregisterHost()
    {
        var url = masterServerURL + "unregisterhost";
        url += "?GUID=" + hostData.guid;

        url += "&gameType=" + WWW.EscapeURL(hostData.gameType);
        url += "&gameName=" + WWW.EscapeURL(hostData.gameName);

        yield return StartCoroutine(sentUrlInfo(url));


    }

    IEnumerator updateSelfHost()
    {
        var url = masterServerURL + "updatehost";
        url += "?GUID=" + hostData.guid;

        url += "&gameType=" + WWW.EscapeURL(hostData.gameType);
        url += "&gameName=" + WWW.EscapeURL(hostData.gameName);

        yield return StartCoroutine(sentUrlInfo(url));

    }

    IEnumerator beginHost()
    {
        //Network.InitializeServer(32, remotePort, true);
        failedRequest = false;
        //yield return new WaitForSeconds(1f);
        //int lNatRetries = 0;
        //while (
        //    inHosting
        //    && Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS"
        //    && lNatRetries < maxNatRetry)
        //{
        //    yield return new WaitForSeconds(1f);
        //    ++lNatRetries;
        //}

        //if (Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS")
        //{
        //    Debug.LogError("externalIP == UNASSIGNED_SYSTEM_ADDRESS");
        //    failedRequest = true;
        //}
        //else
        //{
            url = masterServerURL + "registerhost";
            url += "?IP=" + hostData.IP;
            url += "&port=" + hostData.port;
            url += "&GUID=" + hostData.guid;

            url += "&gameType=" + WWW.EscapeURL(hostData.gameType);
            url += "&gameName=" + WWW.EscapeURL(hostData.gameName);
            url += "&comment=" + WWW.EscapeURL(hostData.comment);

            yield return StartCoroutine(sentUrlInfo(url));

        //}

    }

    public string url;

    IEnumerator sentUrlInfo(string pUrl)
    {
        failedRequest = false;

        var www = new WWW(pUrl);
        yield return www;

        if (www.error != null || www.text != "succeeded")
        {
            failedRequest = true;
            Debug.LogError(www.text);
            Debug.LogError(pUrl);
        }

    }


}