var versionnumber="0.00";

function Awake()
{
	//断开网络, 以免回到主菜单后, 影响后面的行为
	Network.Disconnect();
}

function OnGUI () {
	
	GUILayout.BeginArea(Rect(Screen.width/2-200,0,250,Screen.height));
	
	GUILayout.FlexibleSpace();	
	
	GUILayout.BeginHorizontal();
	GUILayout.FlexibleSpace();	
	//GUILayout.Label("Microcosmic War  0.05beta-1");
	GUILayout.Label("Microcosmic War "+versionnumber);
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	
	GUILayout.Space(30);
	
	GUILayout.BeginHorizontal();
	
	
	
	GUILayout.BeginVertical();
	if(GUILayout.Button("New Map (please click me)")){
		Application.LoadLevel("sewer1");
	}
	GUILayout.Space(20);
	if(GUILayout.Button("Single Player")){
		Application.LoadLevel("ChooseRace");
	}
	GUILayout.Space(20);
	
	//GUILayout.Space(10);
	
	if(GUILayout.Button("Network Player (don't try me)")){
		Application.LoadLevel("NetworkMenu");
	}
	GUILayout.Space(20);
	if(GUILayout.Button("Quit")){
		Application.Quit();
	}
	
	
	GUILayout.EndVertical();
	
	GUILayout.Space(30);
	
	
	GUILayout.EndHorizontal();
	GUILayout.FlexibleSpace();
	GUILayout.EndArea();
	
}