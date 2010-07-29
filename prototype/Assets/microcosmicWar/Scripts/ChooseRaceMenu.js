
var raceSelect=0;

var savedDataName="_info";

function Start()
{
	DontDestroyOnLoad( GameObject.Find(savedDataName) );
}

function Update () {
}

protected function raceSelectToEnum(ID:int)
{
	if(ID==0)
		return Race.ePismire;
	if(ID==1)
		return Race.eBee;
}

function OnGUI () {
	
	GUILayout.BeginArea(Rect(Screen.width/2-200,0,200,Screen.height));
	
	GUILayout.FlexibleSpace();	
	
	GUILayout.BeginHorizontal();
	GUILayout.FlexibleSpace();	
	//GUILayout.Label("Microcosmic War  0.00000000..1");
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	
	GUILayout.Space(30);
	
	GUILayout.BeginHorizontal();
	
	
	
	GUILayout.BeginVertical();
	/*
	if(GUILayout.Button("testScene")){
		Application.LoadLevel("testScene");
	}
	GUILayout.Space(20);
	
	//GUILayout.Space(10);
	
	if(GUILayout.Button("Network Player")){
		Application.LoadLevel("NetworkMenu");
	}
	GUILayout.Space(20);
	if(GUILayout.Button("Quit")){
		Application.Quit();
	}*/
	GUILayout.Space(30);
	GUILayout.Label("choose team");
	GUILayout.Space(5);
	raceSelect = GUILayout.SelectionGrid (raceSelect,["pismire","bee"],2);
	
	
	GUILayout.EndVertical();
	
	GUILayout.Space(30);
	
	
	GUILayout.EndHorizontal();
	GUILayout.Space(30);
	GUILayout.BeginHorizontal();
	
	if(GUILayout.Button("OK")){
		GameObject.Find(savedDataName)
			.GetComponentInChildren(PlayerInfo)
				.setRace(raceSelectToEnum(raceSelect));
		Application.LoadLevel("testScene");
		//print("Application.LoadLevel");
	}
	GUILayout.Space(20);
	if(GUILayout.Button("Back")){
		Application.LoadLevel("LoaderMenu");
	}
	
	GUILayout.EndHorizontal();
	GUILayout.FlexibleSpace();
	GUILayout.EndArea();
	
}