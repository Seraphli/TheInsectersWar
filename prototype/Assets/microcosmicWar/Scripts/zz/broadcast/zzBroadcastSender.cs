using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

class zzBroadcastSender
{
    Socket mSocket;
    int mPort;

    IPEndPoint[]    mIPEndPoints = new IPEndPoint[0];

    public  void setBroadcastIPs(IPAddress[] pIPs)
    {
        mIPEndPoints = new IPEndPoint[pIPs.Length];
        for (int i = 0; i < pIPs.Length;++i )
        {
            mIPEndPoints[i] = new IPEndPoint(pIPs[i], mPort);
        }
    }

    public int port
    {
        get { return mPort; }

        set
        {
            foreach (var lEndPoint in mIPEndPoints)
            {
                lEndPoint.Port = value;
            }
            mPort = value;
        }
    }

    public zzBroadcastSender()
    {
        resetSocket();

        mPort = 0;
        //bindReceivePort();

    }

    public void send(string request)
    {

        byte[] buffer = Encoding.Unicode.GetBytes(request);

        foreach (var lIPEndPoint in mIPEndPoints)
        {
            mSocket.SendTo(buffer, lIPEndPoint);
        }

    }

    public void close()
    {
        mSocket.Close();
    }

    void resetSocket()
    {
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //设置该scoket实例的发送形式
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 3000);
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

    }


}