var versionnumber="0.00";
var GUIRoot:zzInterfaceGUI;
var chSkin : GUISkin;
var languageCtrl:zzLanguage;
var buttonParentGUI:zzInterfaceGUI;


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

function LanguageButtonCall(pGUI:zzInterfaceGUI)
{
	switch (languageCtrl.Language()) {
		case "English" :
			
			buttonParentGUI.getSubElement("Language").setText("English");
			languageCtrl.Language("Chinese");
			zzLanguage.getSingleton().setChinese(buttonParentGUI);
			break;
		
		case "Chinese" :
		
			buttonParentGUI.getSubElement("Language").setText("涓枃");
			languageCtrl.Language("English");
			zzLanguage.getSingleton().setEnglish(buttonParentGUI);
			break;
		
	}
}
function bindButtonCall(pButtonContain:zzInterfaceGUI,pButtonName:String,pCall)
{
	var lButton:zzButton= pButtonContain.getSubElement(pButtonName);
	lButton.setClickCall(pCall);
}

function bindGUI()
{
	bindButtonCall(buttonParentGUI,"sewer1",sewer1ButtonCall);
	bindButtonCall(buttonParentGUI,"SinglePlayer",ChooseRaceButtonCall);
	bindButtonCall(buttonParentGUI,"NetworkPlayer",NetworkMenuButtonCall);
	bindButtonCall(buttonParentGUI,"Quit",QuitButtonCall);
	bindButtonCall(buttonParentGUI,"Language",LanguageButtonCall);

	//显示版本号
	buttonParentGUI.getSubElement("versionLabel").setText("寰鎴樹簤 "+versionnumber);
	buttonParentGUI.getSubElement("Language").setText("涓枃");
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