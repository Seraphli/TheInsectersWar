
var remoteIP = "127.0.0.1";
var remotePort = 25000;
//var listenPort = 25000;
//var useNAT = false;

var networkConnectionError :NetworkConnectionError ;

var gameTypeName ="CZLUniqueGameType";
var gameName ="czl game";

//var serverPort = 25002;
//private var doneTesting = false;
//private var probingPublicIP = false;
//private var testMessage = "Undetermined NAT capabilities";
function Start()
{
	//MasterServer.ClearHostList();
	//MasterServer.RequestHostList(gameTypeName);
//Network.useNat = true;
}
function Update () 
{
}

function OnFailedToConnect(error: NetworkConnectionError)
{
//Debug.Log("Could not connect to server: "+ error);
	networkConnectionError = error;
}

function OnPlayerConnected (player : NetworkPlayer)
{
	print("OnPlayerConnected");
	networkView.RPC( "LoadMyLevel", RPCMode.AllBuffered, "", 0);
}

@script RequireComponent(NetworkView)

@RPC
function LoadMyLevel (level : String, levelPrefix : int)
{
	Network.isMessageQueueRunning = false;
	Application.LoadLevel("testScene");
}

function OnGUI ()
{
	GUILayout.Space(10);

	GUILayout.BeginHorizontal();
	GUILayout.Space(10);
	if (Network.peerType == NetworkPeerType.Disconnected)
	{
		GUILayout.BeginVertical();
		if (GUILayout.Button ("Connect"))
		{

			//var data : HostData[] = MasterServer.PollHostList();
			//Network.useNat = useNAT;
			// Use NAT punchthrough if no public IP present
			//Network.useNat = !Network.HavePublicAddress();
			//Network.useNat = false;
			networkConnectionError = Network.Connect(remoteIP, remotePort);
			//print(networkConnectionError);
		}
		if (GUILayout.Button ("Start Server"))
		{
			//Network.useNat = useNAT;
			// Use NAT punchthrough if no public IP present
			//Network.useNat = !Network.HavePublicAddress();
			//Network.useNat = false;
			Network.InitializeServer(32, remotePort);
			
			//MasterServer.RegisterHost(gameTypeName,
			//	gameName, ""); 
			
			// Notify our objects that the level and the network is ready
			//for (var go in FindObjectsOfType(GameObject))
			//	go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);		
		}
		// Refresh hosts
		GUILayout.Label(networkConnectionError.ToString());
		GUILayout.Label(Network.peerType.ToString());
		/*
		if (GUILayout.Button ("Refresh available Servers") )
		{
			MasterServer.RequestHostList (gameTypeName);
		}
		GUILayout.Space(10);
	
		var data : HostData[] = MasterServer.PollHostList();
		GUILayout.Label(data.length.ToString());
		for (var element in data)
		{
			GUILayout.Label(element.gameType);
			GUILayout.Label(element.gameName);
			for (var ipElement in element.ip)
				GUILayout.Label(ipElement);
			//print(element.ip);
			//print(element.ip[0]);
			GUILayout.Label(element.port.ToString());
			GUILayout.Space(2);
		}
		*/
		GUILayout.EndVertical();
		remoteIP = GUILayout.TextField(remoteIP, GUILayout.MinWidth(100));
		remotePort = parseInt(GUILayout.TextField(remotePort.ToString()));
	}
	else
	{
		if (GUILayout.Button ("Disconnect"))
		{
			Network.Disconnect(200);
		}
		GUILayout.Label(Network.peerType.ToString());
	}
	
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
}

/*
function TestConnection() {
	// Start/Poll the connection test, report the results in a label and react to the results accordingly
	natCapable = Network.TestConnection();
	switch (natCapable) {
		case ConnectionTesterStatus.Error: 
			testMessage = "Problem determining NAT capabilities";
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.Undetermined: 
			testMessage = "Undetermined NAT capabilities";
			doneTesting = false;
			break;
			
		case ConnectionTesterStatus.PrivateIPNoNATPunchthrough: 
			testMessage = "Cannot do NAT punchthrough, filtering NAT enabled hosts for client connections, local LAN games only.";
			filterNATHosts = true;
			Network.useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.PrivateIPHasNATPunchThrough:
			if (probingPublicIP)
				testMessage = "Non-connectable public IP address (port "+ remotePort +" blocked), NAT punchthrough can circumvent the firewall.";
			else
				testMessage = "NAT punchthrough capable. Enabling NAT punchthrough functionality.";
			// NAT functionality is enabled in case a server is started,
			// clients should enable this based on if the host requires it
			Network.useNat = true;
			doneTesting = true;
			break;
			
		case ConnectionTesterStatus.PublicIPIsConnectable:
			testMessage = "Directly connectable public IP address.";
			Network.useNat = false;
			doneTesting = true;
			break;
			
		// This case is a bit special as we now need to check if we can 
		// cicrumvent the blocking by using NAT punchthrough
		case ConnectionTesterStatus.PublicIPPortBlocked:
			testMessage = "Non-connectble public IP address (port " + remotePort +" blocked), running a server is impossible.";
			Network.useNat = false;
			// If no NAT punchthrough test has been performed on this public IP, force a test
			if (!probingPublicIP)
			{
				Debug.Log("Testing if firewall can be circumnvented");
				natCapable = Network.TestConnectionNAT();
				probingPublicIP = true;
				timer = Time.time + 10;
			}
			// NAT punchthrough test was performed but we still get blocked
			else if (Time.time > timer)
			{
				probingPublicIP = false; 		// reset
				Network.useNat = true;
				doneTesting = true;
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
			break;
		default: 
			testMessage = "Error in test routine, got " + natCapable;
	}
	//Debug.Log(natCapable + " " + probingPublicIP + " " + doneTesting);
}
*/
