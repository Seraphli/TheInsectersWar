using UnityEngine;

public class RaceChooseSetting:MonoBehaviour
{
    Texture2D[] _raceImages;

    public Texture2D[] raceImages
    {
        get { return _raceImages; }
    }

    public PlayerInfo playerInfo;

    [SerializeField]
    Race _race = Race.ePismire;

    public Race race
    {
        get { return playerInfo.race; }
        set { playerInfo.race = value; }
    }

    public int imageSize = 50;

    public string prismireText;
    public Texture2D prismireImage;

    public string beeText;
    public Texture2D beeImage;

    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;

    void Start()
    {
        _raceImages = new Texture2D[] { prismireImage, beeImage };
    }

    static RaceChooseSettingGUI raceChooseGUI = new RaceChooseSettingGUI();

    public static IPropertyGUI PropertyGUI
    {
        get
        {
            return raceChooseGUI;
        }
    }
}

public class RaceChooseSettingGUI : IPropertyGUI
{

    public override void OnPropertyGUI(MonoBehaviour pObject)
    {
        var lRaceChooseSetting = (RaceChooseSetting)pObject;
        var lChoose = lRaceChooseSetting.race;
        int lSelected = lChoose == Race.ePismire ? 0 : 1;
        Race lNewSelected = Race.eNone;
        //GUILayout.BeginVertical();
        //lSelected = GUILayout.SelectionGrid(lSelected, lRaceChooseSetting.raceImages, 2,
        //    GUILayout.Width(lRaceChooseSetting.imageSize*2),
        //    GUILayout.Height(lRaceChooseSetting.imageSize));

        //GUILayout.BeginHorizontal();
        //GUILayout.Label(lRaceChooseSetting.prismireText);
        //GUILayout.Label(lRaceChooseSetting.beeText);
        //GUILayout.EndHorizontal();
        //GUILayout.EndVertical();
        //var lNewChoose = lSelected == 0 ? Race.ePismire : Race.eBee;
        //if (lNewChoose != lChoose)
        //    lRaceChooseSetting.race = lNewChoose;
        int lImageSize = lRaceChooseSetting.imageSize;
        var lSelectedStyle = lRaceChooseSetting.selectedStyle;
        var lNotSelectedStyle = lRaceChooseSetting.notSelectedStyle;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (drawChoose(lRaceChooseSetting.prismireText, lRaceChooseSetting.prismireImage, lImageSize,
            lChoose == Race.ePismire ? lSelectedStyle : lNotSelectedStyle))
            lNewSelected = Race.ePismire;
        GUILayout.FlexibleSpace();
        if(drawChoose(lRaceChooseSetting.beeText, lRaceChooseSetting.beeImage,lImageSize,
            lChoose == Race.eBee ? lSelectedStyle : lNotSelectedStyle))
            lNewSelected = Race.eBee;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (lNewSelected != Race.eNone && lNewSelected != lChoose)
            lRaceChooseSetting.race = lNewSelected;
    }

    public bool drawChoose(string pText, Texture2D pImage, int pImageSize,GUIStyle pStyle)
    {
        GUILayout.BeginVertical();
        float lWidth = pImageSize;
        float lHeight = pImageSize;
        bool lOut = GUILayout.Button(pImage, pStyle, GUILayout.Width(pImageSize), GUILayout.Height(pImageSize));
        GUILayout.Label(pText);
        GUILayout.EndVertical();
        return lOut;
    }
}