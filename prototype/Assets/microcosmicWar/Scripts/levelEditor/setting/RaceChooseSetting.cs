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


}
