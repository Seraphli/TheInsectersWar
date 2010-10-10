
using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{



    public string versionnumber = "0.00f";
    public zzInterfaceGUI GUIRoot;
    public GUISkin chSkin;

    void Awake()
    {
        //断开网络, 以免回到主菜单后, 影响后面的行为
        Network.Disconnect();
        bindGUI();
    }

    void sewer1ButtonCall(zzInterfaceGUI pGUI)
    {
        Application.LoadLevel("sewer1");
    }

    void ChooseRaceButtonCall(zzInterfaceGUI pGUI)
    {
        Application.LoadLevel("ChooseRace");
    }

    void NetworkMenuButtonCall(zzInterfaceGUI pGUI)
    {
        Application.LoadLevel("NetworkMenu");
    }

    void QuitButtonCall(zzInterfaceGUI pGUI)
    {
        Application.Quit();
    }

    ////杞崲鎴愪腑鏂?void  CnButtonCall ( zzInterfaceGUI pGUI  ){

    //    zzInterfaceGUI buttonParentGUI = GUIRoot.getSubElement("window");
    //    //璁剧疆涓枃
    //    zzLanguage.getSingleton().setChinese(buttonParentGUI);
    //}

    ////杞寲鎴愯嫳鏂?void  EnButtonCall ( zzInterfaceGUI pGUI  ){
    //    zzInterfaceGUI buttonParentGUI = GUIRoot.getSubElement("window");
    //    //璁剧疆鑻辨枃
    //    zzLanguage.getSingleton().setEnglish(buttonParentGUI);
    //}

    void bindButtonCall(zzInterfaceGUI pButtonContain, string pButtonName, zzInterfaceGUI.GUICallFunc pCall)
    {
        zzButton lButton = (zzButton)pButtonContain.getSubElement(pButtonName);
        lButton.setClickCall(pCall);
    }

    void bindGUI()
    {
        zzInterfaceGUI buttonParentGUI = GUIRoot.getSubElement("window");
        bindButtonCall(buttonParentGUI, "sewer1", sewer1ButtonCall);
        bindButtonCall(buttonParentGUI, "SinglePlayer", ChooseRaceButtonCall);
        bindButtonCall(buttonParentGUI, "NetworkPlayer", NetworkMenuButtonCall);
        bindButtonCall(buttonParentGUI, "Quit", QuitButtonCall);
        //bindButtonCall(buttonParentGUI,"Cn",CnButtonCall);
        //bindButtonCall(buttonParentGUI,"En",EnButtonCall);

        //显示版本号
        buttonParentGUI.getSubElement("versionLabel").setText("微观战争 " + versionnumber);
    }

    /*
    void  OnGUI (){
	
        GUILayout.BeginArea( new Rect(Screen.width/2-200,0,250,Screen.height));
	
        GUILayout.FlexibleSpace();	
	
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();	
        //GUILayout.Label("Microcosmic War  0.05fbeta-1");
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
}