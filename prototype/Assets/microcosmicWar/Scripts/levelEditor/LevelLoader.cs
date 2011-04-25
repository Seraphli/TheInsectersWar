using UnityEngine;

public class LevelLoader:MonoBehaviour
{
    public string settingObject;
    public string levelName;

    public SceneLoader sceneLoader;

    public void setPath()
    {
        var lSettingObject = GameObject.Find(settingObject);
        if (lSettingObject&&lSettingObject.GetComponent<LevelLoader>())
        {
            levelName = lSettingObject.GetComponent<LevelLoader>().levelName;
            Destroy(lSettingObject);
        }
        sceneLoader.loadPath = levelName;
    }

}