var versionnumber="0.00";
var GUIRoot:zzInterfaceGUI;
var chSkin : GUISkin;
enum Language
{
	En = 0,
	Cn = 1,
};

function Awake()
{
	//断开网络, 以免回到主菜单后, 影响后面的行为
	Network.Disconnect();
	bindGUI();
}

function sewer1ButtonCall(pGUI:zzInterfaceGUI)
{
	Application.LoadLevel("sewer1");
}

function ChooseRaceButtonCall(pGUI:zzInterfaceGUI)
{
	Application.LoadLevel("ChooseRace");
}

function NetworkMenuButtonCall(pGUI:zzInterfaceGUI)
{
	Application.LoadLevel("NetworkMenu");
}

function QuitButtonCall(pGUI:zzInterfaceGUI)
{
	Application.Quit();
}

//杞崲鎴愪腑鏂�
function CnButtonCall(pGUI:zzInterfaceGUI)
{
	//var result:String;
	//Debug.Log("Got");
	var buttonParentGUI : zzInterfaceGUI = GUIRoot.getSubElement("window");
	//璁剧疆涓枃
	zzLanguage.getSingleton().setChinese(buttonParentGUI);
	//result=zzLanguage.getSingleton().SwitchLanguage("Quit",Language.En);
	//Debug.Log(result);
}

//杞寲鎴愯嫳鏂�
function EnButtonCall(pGUI:zzInterfaceGUI)
{
	var buttonParentGUI : zzInterfaceGUI = GUIRoot.getSubElement("window");
	//璁剧疆鑻辨枃
	zzLanguage.getSingleton().setEnglish(buttonParentGUI);
}

function bindButtonCall(pButtonContain:zzInterfaceGUI,pButtonName:String,pCall)
{
	var lButton:zzButton= pButtonContain.getSubElement(pButtonName);
	lButton.setClickCall(pCall);
}

function bindGUI()
{
	var buttonParentGUI:zzInterfaceGUI = GUIRoot.getSubElement("window");
	bindButtonCall(buttonParentGUI,"sewer1",sewer1ButtonCall);
	bindButtonCall(buttonParentGUI,"SinglePlayer",ChooseRaceButtonCall);
	bindButtonCall(buttonParentGUI,"NetworkPlayer",NetworkMenuButtonCall);
	bindButtonCall(buttonParentGUI,"Quit",QuitButtonCall);
	bindButtonCall(buttonParentGUI,"Cn",CnButtonCall);
	bindButtonCall(buttonParentGUI,"En",EnButtonCall);
	
	//显示版本号
	buttonParentGUI.getSubElement("versionLabel").setText("寰鎴樹簤 "+versionnumber);
}

/*
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
	if(GUILayout.Button("sewer 1")){
		Application.LoadLevel("sewer1");
	}
	GUILayout.Space(20);
	if(GUILayout.Button("Single Player:sewer (new)")){
		Application.LoadLevel("ChooseRace");
	}
	GUILayout.Space(20);
	
	//GUILayout.Space(10);
	
	if(GUILayout.Button("Network Player :sewer (new)")){
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
*/