using UnityEngine;
using System.Net;
using System.Net.Sockets;
using LumiSoft.Net.STUN.Message;
using LumiSoft.Net.STUN.Client;

public class zzNatStun:MonoBehaviour
{
    public string stunServerAddress;
    public int stunServerPort;

    public string publicIP;

    public int publicPort;
    
    System.Action stunSucceedEvent;

    public void addSucceedEventReceiver(System.Action pReceiver)
    {
        stunSucceedEvent += pReceiver;
    }

    System.Action stunFailEvent;

    public void addFailEventReceiver(System.Action pReceiver)
    {
        stunFailEvent += pReceiver;
    }

    public bool inProcessing
    {
        get
        {
            return timer.enabled;
        }
    }

    public STUN_Result result;

    void Start()
    {
        if (stunSucceedEvent==null)
        {
            stunSucceedEvent = zzUtilities.nullFunction;
        }
        if (stunFailEvent == null)
        {
            stunFailEvent = zzUtilities.nullFunction;
        }
        timer = gameObject.AddComponent<zzTimer>();
        timer.setInterval(0.05f);
        timer.addImpFunction(updateTransaction);
        timer.enabled = false;
    }

    public void query(zzHostInfo pHostInfo)
    {
        Socket lSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        lSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
        lSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        lSocket.Bind(new IPEndPoint(IPAddress.Any, pHostInfo.port));
        Query(stunServerAddress, stunServerPort, lSocket);
    }

    public void stunEnd()
    {
        timer.enabled = false;
        if (transaction.socket!=null)
            transaction.socket.Close();
    }

    void stunSucceed()
    {
        stunEnd();
        publicIP = result.PublicEndPoint.Address.ToString();
        publicPort = result.PublicEndPoint.Port;
        stunSucceedEvent();
    }

    void stunFail()
    {
        stunEnd();
        stunFailEvent();
    }



    #region static method Query

    /// <summary>
    /// Gets NAT info from STUN server.
    /// </summary>
    /// <param name="host">STUN server name or IP.</param>
    /// <param name="port">STUN server port. Default port is 3478.</param>
    /// <param name="socket">UDP socket to use.</param>
    /// <returns>Returns UDP netwrok info.</returns>
    /// <exception cref="Exception">Throws exception if unexpected error happens.</exception>
    public void Query(string host, int port, Socket socket)
    {
        if (host == null)
        {
            throw new System.ArgumentNullException("host");
        }
        if (socket == null)
        {
            throw new System.ArgumentNullException("socket");
        }
        if (port < 1)
        {
            throw new System.ArgumentNullException("Port value must be >= 1 !");
        }
        if (socket.ProtocolType != ProtocolType.Udp)
        {
            throw new System.ArgumentNullException("Socket must be UDP socket !");
        }

        IPEndPoint remoteEndPoint = new IPEndPoint(System.Net.Dns.GetHostAddresses(host)[0], port);

        socket.ReceiveTimeout = 3000;
        socket.SendTimeout = 3000;

        // Test I
        STUN_Message test1 = new STUN_Message();
        test1.Type = STUN_MessageType.BindingRequest;
        transaction.doTransaction(test1, socket, remoteEndPoint);
        transactionFunc = test1ResponeFunc;
        timer.enabled = true;

    }

    #endregion

    #region Respone

    STUN_Message test1ResponeMessage;
    TransactionFunc test1ResponeFunc(STUN_Message test1response)
    {
        // UDP blocked.
        if (test1response == null)
        {
            result = new STUN_Result(STUN_NetType.UdpBlocked, null);
            stunFail();
            return null;
        }
        else
        {
            test1ResponeMessage = test1response;
            // Test II
            STUN_Message test2 = new STUN_Message();
            test2.Type = STUN_MessageType.BindingRequest;
            test2.ChangeRequest = new STUN_t_ChangeRequest(true, true);

            // No NAT.
            if (transaction.socket.LocalEndPoint.Equals(test1response.MappedAddress))
            {
                return transactionRespone(test2, noNATResponeFunc);
            }
            // NAT
            else
            {
                return transactionRespone(test2, NATResponeFunc);
            }
        }
    }

    TransactionFunc noNATResponeFunc(STUN_Message pRequest)
    {
        STUN_Result lResult;
        // Open Internet.
        if (pRequest != null)
        {
            lResult = new STUN_Result(STUN_NetType.OpenInternet, pRequest.MappedAddress);
        }
        // Symmetric UDP firewall.
        else
        {
            lResult = new STUN_Result(STUN_NetType.SymmetricUdpFirewall, pRequest.MappedAddress);
        }
        return succeedRespone(lResult);
    }

    TransactionFunc NATResponeFunc(STUN_Message pRequest)
    {
        // Full cone NAT.
        if (pRequest != null)
        {
            return succeedRespone(
                new STUN_Result(STUN_NetType.FullCone, pRequest.MappedAddress));
        }
        else
        {
            STUN_Message test12 = new STUN_Message();
            test12.Type = STUN_MessageType.BindingRequest;

            return transactionRespone(test12, testI_IIResponeFunc);
        }
    }

    TransactionFunc testI_IIResponeFunc(STUN_Message pRequest)
    {
        if (pRequest == null)
        {
            Debug.LogError("STUN Test I(II) dind't get resonse !");
            stunFail();
            return null;
        }
        else
        {
            // Symmetric NAT
            if (!test1ResponeMessage.MappedAddress.Equals(pRequest.MappedAddress))
            {
                return succeedRespone(
                    new STUN_Result(STUN_NetType.Symmetric, pRequest.MappedAddress));
            }
            else
            {
                // Test III
                STUN_Message test3 = new STUN_Message();
                test3.Type = STUN_MessageType.BindingRequest;
                test3.ChangeRequest = new STUN_t_ChangeRequest(false, true);
                return transactionRespone(test3, test3ResponeFunc);
            }
        }
    }

    TransactionFunc test3ResponeFunc(STUN_Message pRequest)
    {
        // Restricted
        if (pRequest != null)
        {
            return succeedRespone(
                new STUN_Result(STUN_NetType.RestrictedCone, test1ResponeMessage.MappedAddress));
        }
        // Port restricted
        else
        {
            return succeedRespone(
                new STUN_Result(STUN_NetType.PortRestrictedCone, test1ResponeMessage.MappedAddress));
        }
    }
    #endregion

    TransactionFunc transactionRespone(STUN_Message pRequest,TransactionFunc pFunc )
    {
        transaction.doTransaction(pRequest);
        return pFunc;
    }

    TransactionFunc succeedRespone(STUN_Result pResult)
    {
        result = pResult;
        stunSucceed();
        return null;
    }

    #region method DoTransaction

    class Transaction
    {
        STUN_Message _request;
        public STUN_Message request
        {
            get
            {
                return _request;
            }

            set
            {
                _request = value;
                requestBytes = request.ToByteData();
            }
        }

        public Socket socket;
        public IPEndPoint remoteEndPoint;

        byte[] requestBytes;
        float startTime;
        public float timeout = 4f;

        public bool overTime
        {
            get
            {
                return startTime+timeout < Time.realtimeSinceStartup;
            }
        }

        public void doTransaction(STUN_Message pRequest)
        {
            doTransaction(pRequest, socket, remoteEndPoint);
        }

        public void doTransaction(STUN_Message pRequest, Socket pSocket,
            IPEndPoint pRemoteEndPoint)
        {
            request = pRequest;
            socket = pSocket;
            remoteEndPoint = pRemoteEndPoint;
            startTime = Time.realtimeSinceStartup;
            socket.SendTo(requestBytes, remoteEndPoint);
        }

        public STUN_Message continueTransaction()
        {
            socket.SendTo(requestBytes, remoteEndPoint);

            // We got response.
            if (socket.Poll(100, SelectMode.SelectRead))
            {
                byte[] receiveBuffer = new byte[512];
                socket.Receive(receiveBuffer);

                // Parse message
                STUN_Message response = new STUN_Message();
                response.Parse(receiveBuffer);

                // Check that transaction ID matches or not response what we want.
                if (request.TransactionID.Equals(response.TransactionID))
                {
                    return response;
                }
            }

            return null;
        }
    }

    Transaction transaction = new Transaction();
    zzTimer timer;

    delegate TransactionFunc TransactionFunc(STUN_Message message);

    TransactionFunc transactionFunc;

    void updateTransaction()
    {
        var lMessage = transaction.continueTransaction();
        if (lMessage != null || transaction.overTime)
        {
            transactionFunc = transactionFunc(lMessage);
        }
    }


    #endregion
}