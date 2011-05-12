using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

class zzBroadcastReciever
{
    Socket mSocket;
    int mPort;

    public int port
    {
        get { return mPort; }

        set
        {
            mPort = value;
            bindReceivePort();
        }
    }

    public zzBroadcastReciever()
    {
        resetSocket();

        mPort = 0;
        //bindReceivePort();

    }

    public void close()
    {
        mSocket.Close();
    }

    void bindReceivePort()
    {
        if(mSocket.LocalEndPoint != null)
        {
            mSocket.Close();
            resetSocket();
        }

        //初始化一个侦听局域网内部所有IP和指定端口
        IPEndPoint lIPEndPoint = new IPEndPoint(IPAddress.Any, mPort);

        EndPoint lEndPoint = (EndPoint)lIPEndPoint;

        //绑定这个实例
        mSocket.Bind(lEndPoint);
    }

    void resetSocket()
    {
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //设置该scoket实例的发送形式
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 3000);
        mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

    }

    public IPEndPoint  receive(out string pData)
    {

        if (mSocket.Available > 0)
        {
            //设置缓冲数据流
            byte[] buffer = new byte[1024];
            EndPoint lEndPoint = new IPEndPoint(IPAddress.Any,1);
            //接收数据,并确把数据设置到缓冲流里面
            mSocket.ReceiveFrom(buffer, ref lEndPoint);
            pData = Encoding.Unicode.GetString(buffer).TrimEnd('\u0000');
            return (IPEndPoint)lEndPoint;

        }
        pData = string.Empty;
        return null;
    }

}