var versionnumber="0.00";
var GUIRoot:zzInterfaceGUI;
var chSkin : GUISkin;

function Awake()
{
	//∂œø™Õ¯¬Á, “‘√‚ªÿµΩ÷˜≤Àµ•∫Û, ”∞œÏ∫Û√Êµƒ––Œ™
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

//ËΩ¨Êç¢Êàê‰∏≠Êñá
function CnButtonCall(pGUI:zzInterfaceGUI)
{
	
	var buttonParentGUI : zzInterfaceGUI = GUIRoot.getSubElement("window");
	//ËÆæÁΩÆ‰∏≠Êñá
	zzLanguage.getSingleton().setChinese(buttonParentGUI);
}

//ËΩ¨ÂåñÊàêËã±Êñá
function EnButtonCall(pGUI:zzInterfaceGUI)
{
	var buttonParentGUI : zzInterfaceGUI = GUIRoot.getSubElement("window");
	//ËÆæÁΩÆËã±Êñá
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
	
	//œ‘ æ∞Ê±æ∫≈
	buttonParentGUI.getSubElement("versionLabel").setText("ÂæÆËßÇÊàò‰∫â "+versionnumber);
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