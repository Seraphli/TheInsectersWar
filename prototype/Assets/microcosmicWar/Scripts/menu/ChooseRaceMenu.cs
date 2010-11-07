
using UnityEngine;
using System.Collections;

public class ChooseRaceMenu : MonoBehaviour
{


    public int raceSelect = 0;

    public string savedDataName = "_info";

    void Start()
    {
        DontDestroyOnLoad(GameObject.Find(savedDataName));
    }

    //void  Update (){
    //}

    protected Race raceSelectToEnum(int ID)
    {
        if (ID == 0)
            return Race.ePismire;
        if (ID == 1)
            return Race.eBee;
        Debug.LogError("raceSelectToEnum(int ID) error");
        return Race.eNone;
    }

    void OnGUI()
    {

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, 0, 200, Screen.height));

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //GUILayout.Label("Microcosmic War  0.00000000f..1");
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
        raceSelect = GUILayout.SelectionGrid(raceSelect, new string[] { "pismire", "bee" }, 2);


        GUILayout.EndVertical();

        GUILayout.Space(30);


        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("OK"))
        {
            PlayerInfo lPlayerInfo = GameObject.Find(savedDataName)
                                                        .GetComponentInChildren<PlayerInfo>();

            lPlayerInfo.setRace(raceSelectToEnum(raceSelect));
            Application.LoadLevel("netSewer2");
            //print("Application.LoadLevel");
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Back"))
        {
            Application.LoadLevel("LoaderMenu");
        }

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();

    }
}