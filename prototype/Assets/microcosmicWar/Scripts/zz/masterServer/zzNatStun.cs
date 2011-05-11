using UnityEngine;
using System.Net;
using System.Net.Sockets;
using LumiSoft.Net.STUN.Client;

public class zzNatStun:MonoBehaviour
{
    public string stunServerAddress;
    public int stunServerPort;

    public zzHostData hostData;

    public string publicIP;

    public int publicPort;

    [SerializeField]
    bool _isSuccess = false;

    public bool isSuccess
    {
        get
        {
            return false;
        }
    }
    
    [SerializeField]
    bool _isFail = false;

    public bool isFail
    {
        get
        {
            return false;
        }
    }

    public void query(zzHostInfo pHostInfo)
    {

        STUN_Result result ;
        using(Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            //mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            socket.Bind(new IPEndPoint(IPAddress.Any, pHostInfo.port));
            result= STUN_Client.Query(stunServerAddress, stunServerPort, socket);
            socket.Close();
        }

        if (result.NetType != STUN_NetType.UdpBlocked)
        {
            publicIP = result.PublicEndPoint.Address.ToString();
            publicPort = result.PublicEndPoint.Port;
            _isSuccess = true;
        }
        else
        {
            _isFail = true ;
        }      
    }
}