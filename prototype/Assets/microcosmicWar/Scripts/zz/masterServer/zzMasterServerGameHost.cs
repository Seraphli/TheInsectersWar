using UnityEngine;
using System.Collections;

public class zzMasterServerGameHost : zzNetworkHost
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

    public int maxNatRetry = 5;

    [SerializeField]
    bool failedRequest = false;

    public bool inHosting = false;

    public float autoUpdateSelf = 7f;


    //------------------------host begin------------------------
    zzTimerCoroutine updateSelfHostTimer;

    public override void RegisterHost(zzHostInfo pHostInfo)
    {
        gameType = pHostInfo.gameType;
        gameName = pHostInfo.gameName;
        comment = pHostInfo.comment;
        RegisterHost();
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
        url += "?GUID=" + Network.player.guid;

        url += "&gameType=" + WWW.EscapeURL(gameType);
        url += "&gameName=" + WWW.EscapeURL(gameName);

        yield return StartCoroutine(sentUrlInfo(url));


    }

    IEnumerator updateSelfHost()
    {
        var url = masterServerURL + "updatehost";
        url += "?GUID=" + Network.player.guid;

        url += "&gameType=" + WWW.EscapeURL(gameType);
        url += "&gameName=" + WWW.EscapeURL(gameName);

        yield return StartCoroutine(sentUrlInfo(url));

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
            && lNatRetries < maxNatRetry)
        {
            yield return new WaitForSeconds(1f);
            ++lNatRetries;
        }

        if (Network.player.externalIP == "UNASSIGNED_SYSTEM_ADDRESS")
        {
            Debug.LogError("externalIP == UNASSIGNED_SYSTEM_ADDRESS");
            failedRequest = true;
        }
        else
        {
            externalIP = Network.player.externalIP;
            externalPort = Network.player.externalPort;
            url = masterServerURL + "registerhost";
            url += "?IP=" + Network.player.externalIP;
            url += "&port=" + Network.player.externalPort;
            url += "&GUID=" + Network.player.guid;

            url += "&gameType=" + WWW.EscapeURL(gameType);
            url += "&gameName=" + WWW.EscapeURL(gameName);
            url += "&comment=" + WWW.EscapeURL(comment);

            yield return StartCoroutine(sentUrlInfo(url));

        }

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