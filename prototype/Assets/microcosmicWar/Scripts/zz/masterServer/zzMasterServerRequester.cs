using UnityEngine;
using System.Collections;

public class zzMasterServerRequester : MonoBehaviour
{
    public string masterServerURL;
    public string gameType;

    ArrayList hostList = new ArrayList();
    
    public int maxRetry = 5;

    [SerializeField]
    bool failedRequest = false;

    [SerializeField]
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
            {
                createAutoRequest();
            }
            else
            {
                requestHostTimer.endTimer();
                requestHostTimer = null;
            }
        }
    }

    void createAutoRequest()
    {
        requestHostTimer = gameObject.AddComponent<zzTimerCoroutine>();
        requestHostTimer.setInterval(autoRequestInterval);
        requestHostTimer.setImpFunction(_RequestHostList);

    }


    public float autoRequestInterval = 7f;


    public delegate void RecieverFunc(zzHostData data);
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

    System.Action endRecieverFunc;

    public void addEndRecieverFunc(System.Action pFunc)
    {
        endRecieverFunc += pFunc;
    }

    void Start()
    {
        if (beginRecieverFunc == null)
            beginRecieverFunc = zzUtilities.nullFunction;
        if (endRecieverFunc == null)
            endRecieverFunc = zzUtilities.nullFunction;
        if (_autoRequestHostList)
            createAutoRequest();
    }

    zzTimerCoroutine requestHostTimer;

    IEnumerator getHostList()
    {
        var url = masterServerURL + "queryhost";

        url += "?gameType=" + WWW.EscapeURL(gameType);
        //url += "&gameName=" + WWW.EscapeURL(gameName);

        failedRequest = false;
        var www = new WWW(url);
        yield return www;

        ArrayList lHostList;
        int retries = 0;
        while (
            (www.error != null || (lHostList = unpackHostList(www.text)) == null) //当有错误时
            && retries < maxRetry)
        {
            retries++;
            www = new WWW(url);
            yield return www;
        }

        if (www.error != null
            || lHostList == null
            || (lHostList = unpackHostList(www.text)) == null)
        {
            failedRequest = true;
            Debug.LogError(www.text);
            Debug.LogError(url);
            hostList = new ArrayList();
        }
        else
            hostList = lHostList;
    }

    IEnumerator _RequestHostList()
    {
        //while (_autoRequestHostList)
        //{
        yield return StartCoroutine( getHostList() );
        beginRecieverFunc();
        foreach (Hashtable lHost in hostList)
        {
            zzHostData lHostData = new zzHostData();
            lHostData.gameName = lHost["gameName"] as string;
            lHostData.gameType = lHost["gameType"] as string;
            lHostData.comment = lHost["comment"] as string;
            lHostData.guid = lHost["guid"] as string;
            lHostData.IP = lHost["IP"] as string;
            lHostData.port = (int)(lHost["port"]);
            recieverFunc(lHostData);
        }
        endRecieverFunc();
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

    //IEnumerator sentUrlInfo(string pUrl)
    //{
    //    failedRequest = false;
    //    var www = new WWW(pUrl);
    //    yield return www;

    //    int retries = 0;
    //    while (
    //        (www.error != null || www.text != "succeeded")//当有错误时
    //        && retries < maxRetry)
    //    {
    //        ++retries;
    //        www = new WWW(pUrl);
    //        yield return www;
    //    }
    //    if (www.error != null || www.text != "succeeded")
    //    {
    //        failedRequest = true;
    //        Debug.LogError(www.text);
    //        Debug.LogError(pUrl);
    //    }

    //}


}