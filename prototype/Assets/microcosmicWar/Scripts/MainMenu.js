

function Awake()
{
	//�Ͽ�����, ����ص����˵���, Ӱ��������Ϊ
	Network.Disconnect();
}

function OnGUI () {
	
	GUILayout.BeginArea(Rect(Screen.width/2-200,0,200,Screen.height));
	
	GUILayout.FlexibleSpace();	
	
	GUILayout.BeginHorizontal();
	GUILayout.FlexibleSpace();	
	GUILayout.Label("Microcosmic War  0.00000000..1");
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	
	GUILayout.Space(30);
	
	GUILayout.BeginHorizontal();
	
	
	
	GUILayout.BeginVertical();
	if(GUILayout.Button("Single Player")){
		Application.LoadLevel("ChooseRace");
	}
	GUILayout.Space(20);
	
	//GUILayout.Space(10);
	
	if(GUILayout.Button("Network Player")){
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