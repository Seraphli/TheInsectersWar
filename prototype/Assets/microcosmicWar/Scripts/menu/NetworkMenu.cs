
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NetworkView))]
public class NetworkMenu : MonoBehaviour
{

    //玩家信息
    public int raceSelect = 0;
    public string playerName = "player";

    public string savedDataName = "_info";

    //网络联接

    public string remoteIP = "127.0f.0.1f";
    public int remotePort = 25000;
    //FIXME_VAR_TYPE listenPort= 25000;
    //FIXME_VAR_TYPE useNAT= false;

    public NetworkConnectionError networkConnectionError;

    public string gameTypeName = "CZLUniqueGameType";
    public string gameName = "czl game";

    public float hostListRefreshTimeout = 10.0f;
    public float refreshTimePos = 0.0f;

    //FIXME_VAR_TYPE serverPort= 25002;
    //private FIXME_VAR_TYPE doneTesting= false;
    //private FIXME_VAR_TYPE probingPublicIP= false;
    //private FIXME_VAR_TYPE testMessage= "Undetermined NAT capabilities";
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find(savedDataName));

        MasterServer.ClearHostList();
        MasterServer.RequestHostList(gameTypeName);
        //Network.useNat = true;
        Network.useNat = false;
    }
    void Update()
    {
        refreshTimePos += Time.deltaTime;
        if (refreshTimePos > hostListRefreshTimeout)
        {
            //MasterServer.ClearHostList();
            MasterServer.RequestHostList(gameTypeName);
            refreshTimePos = 0;
        }
    }


    protected Race raceSelectToEnum(int ID)
    {
        if (ID == 0)
            return Race.ePismire;
        if (ID == 1)
            return Race.eBee;
        Debug.LogError("raceSelectToEnum(int ID)");
        return Race.eNone;
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        //Debug.Log("Could not connect to server: "+ error);
        networkConnectionError = error;
        Network.useNat = false;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        print("OnPlayerConnected");
        Race lServerRace = raceSelectToEnum(raceSelect);
        //networkView.RPC( "LoadMyLevel", RPCMode.AllBuffered, "", 0);

        int lIntRace = (int)Race.ePismire;
        //让联接的客户端选择相反的种族
        if (lServerRace == Race.ePismire)
        {
            lIntRace = (int)Race.eBee;
        }
        else
        {
            lIntRace = (int)Race.ePismire;
        }
        networkView.RPC("LoadMyLevel", RPCMode.Others, lIntRace);
        LoadMyLevel((int)lServerRace);
    }

    //@RPC
    //function setRace

    [RPC]
    void LoadMyLevel(int race)
    {
        //print("LoadMyLevel");
        Network.isMessageQueueRunning = false;

        PlayerInfo playerInfo = GameObject.Find(savedDataName).GetComponentInChildren<PlayerInfo>();
        playerInfo.setRace((Race)race);
        playerInfo.setPlayerName(playerName);

        //Application.LoadLevel("testScene");
        Application.LoadLevel("netSewer1");
    }

    void OnGUI(){
	GUILayout.Space(10);
	GUILayout.BeginHorizontal();
	GUILayout.Space(10);
	
	if (Network.peerType == NetworkPeerType.Disconnected)
	{
		GUILayout.BeginVertical();
		GUILayout.Label("your name");
		playerName= GUILayout.TextField(playerName, GUILayout.MinWidth(100));
		GUILayout.Space(5);
		
		GUILayout.Label("As Server");
		GUILayout.Label("choose team");
		GUILayout.Space(5);
		raceSelect = GUILayout.SelectionGrid (raceSelect,new string[]{"pismire","bee"},2);

		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Start Server:"))
		{
			//Network.useNat = useNAT;
			// Use NAT punchthrough if no public IP present
			//Network.useNat = !Network.HavePublicAddress();
			//Network.useNat = false;
			Network.InitializeServer(32, remotePort);
			
			MasterServer.RegisterHost(gameTypeName,
				gameName, playerName); 
			
			// Notify our objects that the level and the network is ready
			//for (var go in FindObjectsOfType(typeof(GameObject)))
			//	go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);		
		}
		remotePort = int.Parse(GUILayout.TextField(remotePort.ToString()));
		GUILayout.EndHorizontal();
		
		GUILayout.Space(15);
		
		GUILayout.Label("As Client:");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Manual Connect"))
		{

			//HostData[] data = MasterServer.PollHostList();
			//Network.useNat = useNAT;
			// Use NAT punchthrough if no public IP present
			//Network.useNat = !Network.HavePublicAddress();
			//Network.useNat = false;
			networkConnectionError = Network.Connect(remoteIP, remotePort);
			//print(networkConnectionError);
		}
		remoteIP = GUILayout.TextField(remoteIP, GUILayout.MinWidth(100));
		GUILayout.EndHorizontal();
		// Refresh hosts
		GUILayout.Label(networkConnectionError.ToString());
		GUILayout.Label(Network.peerType.ToString());
		
		if (GUILayout.Button ("Refresh available Servers manually") )
		{
			MasterServer.RequestHostList (gameTypeName);
		}
		GUILayout.Space(10);
	
		HostData[] data = MasterServer.PollHostList();
		GUILayout.Label(data.Length.ToString());
        foreach (HostData element in data)
		{
			//GUILayout.Label(element.gameType);
			//GUILayout.Label(element.gameName);
			GUILayout.Label("player name: "+element.comment);
			GUILayout.Label("IP:");
			foreach(string ipElement in element.ip)
				GUILayout.Label(ipElement);
			//print(element.ip);
			//print(element.ip[0]);
			GUILayout.Label("port: "+element.port.ToString());
			
				if (GUILayout.Button("Connect"))
				{
					// Enable NAT functionality based on what the hosts if configured to do
					Network.useNat = element.useNat;
					//if (Network.useNat)
					//	print("Using Nat punchthrough to connect");
					//else
					//	print("Connecting directly to host");
					Network.Connect(element.ip, element.port);			
				}
			GUILayout.Space(2);
		}
		
		GUILayout.EndVertical();
	}
	else
	{
		if (GUILayout.Button ("Disconnect"))
		{
			Network.Disconnect(200);
		}
		GUILayout.Label("state: "+Network.peerType.ToString());
	}
	
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
}

    
    //void  TestConnection (){
    //    // Start/Poll the connection test, report the results in a label and react to the results accordingly
    //    natCapable = Network.TestConnection();
    //    switch (natCapable) {
    //        case ConnectionTesterStatus.Error: 
    //            testMessage = "Problem determining NAT capabilities";
    //            doneTesting = true;
    //            break;
			
    //        case ConnectionTesterStatus.Undetermined: 
    //            testMessage = "Undetermined NAT capabilities";
    //            doneTesting = false;
    //            break;
			
    //        case ConnectionTesterStatus.PrivateIPNoNATPunchthrough: 
    //            testMessage = "Cannot do NAT punchthrough, filtering NAT enabled hosts for client connections, local LAN games only.";
    //            filterNATHosts = true;
    //            Network.useNat = true;
    //            doneTesting = true;
    //            break;
			
    //        case ConnectionTesterStatus.PrivateIPHasNATPunchThrough:
    //            if (probingPublicIP)
    //                testMessage = "Non-connectable public IP address (port "+ remotePort +" blocked), NAT punchthrough can circumvent the firewall.";
    //            else
    //                testMessage = "NAT punchthrough capable. Enabling NAT punchthrough functionality.";
    //            // NAT functionality is enabled in case a server is started,
    //            // clients should enable this based on if the host requires it
    //            Network.useNat = true;
    //            doneTesting = true;
    //            break;
			
    //        case ConnectionTesterStatus.PublicIPIsConnectable:
    //            testMessage = "Directly connectable public IP address.";
    //            Network.useNat = false;
    //            doneTesting = true;
    //            break;
			
    //        // This case is a bit special as we now need to check if we can 
    //        // cicrumvent the blocking by using NAT punchthrough
    //        case ConnectionTesterStatus.PublicIPPortBlocked:
    //            testMessage = "Non-connectble public IP address (port " + remotePort +" blocked), running a server is impossible.";
    //            Network.useNat = false;
    //            // If no NAT punchthrough test has been performed on this public IP, force a test
    //            if (!probingPublicIP)
    //            {
    //                Debug.Log("Testing if firewall can be circumnvented");
    //                natCapable = Network.TestConnectionNAT();
    //                probingPublicIP = true;
    //                pathTimer = Time.time + 10;
    //            }
    //            // NAT punchthrough test was performed but we still get blocked
    //            else if (Time.time > pathTimer)
    //            {
    //                probingPublicIP = false; 		// reset
    //                Network.useNat = true;
    //                doneTesting = true;
    //            }
    //            break;
    //        case ConnectionTesterStatus.PublicIPNoServerStarted:
    //            testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
    //            break;
    //        default: 
    //            testMessage = "Error in test routine, got " + natCapable;
    //    }
    //    //Debug.Log(natCapable + " " + probingPublicIP + " " + doneTesting);
    //}
    

}