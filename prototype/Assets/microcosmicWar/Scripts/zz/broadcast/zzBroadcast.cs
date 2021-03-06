﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;


public class zzBroadcast:MonoBehaviour
{
    public enum ServerType
    {
        receive,
        send,
    }

    public ServerType servertype = ServerType.receive;
    ServerType mServertype;

    public int port;

    public bool autoSent = true;
    public string sentedData;

    public float autoInterval = 0.5f;
    //public Component recieverComponent;
    //void (string data,string IP)
    //public string beginRecieverFunctionName;
    //public string recieverFunctionName;

    public delegate void RecieverFunc(string data, string IP);
    RecieverFunc recieverFunc;

    public void addReciever(RecieverFunc pRecieverFunc)
    {
        recieverFunc += pRecieverFunc;
    }

    System.Action beginRecieverFunc;

    public void addBeginRecieverFunc(System.Action pFunc)
    {
        beginRecieverFunc += pFunc;
    }

    System.Action endRecieverFunc;

    public void addEndRecieverFunc(System.Action pFunc)
    {
        endRecieverFunc += pFunc;
    }


    zzBroadcastSender   broadcastSender;
    zzBroadcastReciever broadcastReciever;

    zzTimerClass timer;

    void Start()
    {
        if (beginRecieverFunc==null)
            beginRecieverFunc = zzUtilities.nullFunction;
        if (endRecieverFunc==null)
            endRecieverFunc = zzUtilities.nullFunction;
        mServertype = servertype;
        //if (mServertype == ServerType.receive || autoSent)
        //{
            timer = new zzTimerClass();
            timer.interval = autoInterval;
        //}

        switch (mServertype)
        {
            case ServerType.receive:
                initReciever();
                timer.addImpFunction(receive);
                break;

            case ServerType.send:
                initSender();
                if (autoSent)
                    timer.addImpFunction(sent);
                else
                    timer.enable = false;
                break;

            default:
                Debug.LogError("ServerType");
                break;
        }
    }

    void Update()
    {
        //if (timer != null)
        timer.Update();
    }

    public void close()
    {

        switch (mServertype)
        {
            case ServerType.receive:
                broadcastReciever.close();
                break;

            case ServerType.send:
                broadcastSender.close();
                break;

            default:
                Debug.LogError("ServerType");
                break;
        }
        timer.enable = false;
    }

    public void sent()
    {
        broadcastSender.send(sentedData);
    }

    void initSender()
    {
        broadcastSender = new zzBroadcastSender();

        broadcastSender.port = port;
        var lNetInfo = zzNetworkInfo.getNetworkInfo();
        IPAddress[] lIPEndPoints = new IPAddress[lNetInfo.Length];
        for (int i = 0; i < lNetInfo.Length; ++i)
        {
            //初始化一个发送广播和指定端口的网络端口实例
            lIPEndPoints[i] = lNetInfo[i].broadcastIP;
        }
        broadcastSender.setBroadcastIPs(lIPEndPoints);
    }

    void initReciever()
    {
        //MethodInfo lRecieverMethodInfo = recieverComponent.GetType()
        //    .GetMethod(recieverFunctionName);
        //recieverFunc = (RecieverFunc)System.Delegate.CreateDelegate(
        //    typeof(RecieverFunc),recieverComponent, lRecieverMethodInfo);

        //if (beginRecieverFunctionName.Length!=0)
        //{
        //    MethodInfo lBeginMethodInfo = recieverComponent.GetType()
        //        .GetMethod(beginRecieverFunctionName);
        //    beginRecieverFunc = (zzUtilities.voidFunction)System.Delegate.CreateDelegate(
        //        typeof(zzUtilities.voidFunction),recieverComponent, lBeginMethodInfo);

        //}

        broadcastReciever = new zzBroadcastReciever();
        broadcastReciever.port = port;
    }

    void receive()
    {
        beginRecieverFunc();
        string lReceivedDate;
        var lEndPoint = broadcastReciever.receive(out lReceivedDate);
        while (lEndPoint!=null)
        {
            recieverFunc(lReceivedDate, lEndPoint.Address.ToString());

            //下一条数据
            lEndPoint = broadcastReciever.receive(out lReceivedDate);
        }
        endRecieverFunc();
    }
}