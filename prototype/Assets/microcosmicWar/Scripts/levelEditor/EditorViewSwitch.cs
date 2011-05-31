using UnityEngine;

public class EditorViewSwitch:MonoBehaviour
{
    //public string editorUiTagName;
    //public string editorCameraTagName;
    public GameObject[] editorObjects = new GameObject[0]{};
    public MonoBehaviour[] editorComponents = new MonoBehaviour[0]{};

    public GameObject[] gameObjects = new GameObject[0] { };
    public MonoBehaviour[] gameComponents = new MonoBehaviour[0] { };
    //public zzInterfaceGUI editorUi;
    //public Camera editorCamera;
    //public AudioListener editorAudioListener;

    ////public string gameUiTagName;
    //public string gameCameraTagName;

    //public zzInterfaceGUI gameUi;
    //public Camera gameCamera;
    //public AudioListener gameAudioListener;

    public enum ViewMode
    {
        editor,
        game,
    }

    public ViewMode viewMode = ViewMode.editor;

    void Start()
    {
        //gameUi = GameScene.Singleton.playerInfo.UiRoot.GetComponent<zzInterfaceGUI>();
        //gameCamera = GameObject.FindGameObjectWithTag(gameCameraTagName).camera;
        switchMode();
    }

    void setActive(GameObject[] pObjects, MonoBehaviour[] pComponents, bool pActive)
    {
        foreach (var lObject in pObjects)
        {
            lObject.SetActiveRecursively(pActive);
        }
        foreach (var lComponent in pComponents)
        {
            lComponent.enabled = pActive;
        }
    }

    public void switchMode()
    {
        switch (viewMode)
        {
            case ViewMode.editor:
                setActive(editorObjects, editorComponents, false);
                setActive(gameObjects, gameComponents, true);
                viewMode = ViewMode.game;
                break;

            case ViewMode.game:
                setActive(editorObjects, editorComponents, true);
                setActive(gameObjects, gameComponents, false);
                viewMode = ViewMode.editor;
                break;

            default:
                Debug.LogError("ViewMode");
                break;
        }
    }

    //void updateMode()
    //{
    //}

    void OnDestroy()
    {
        //防止在编辑器中报错
        try
        {
            if (viewMode == ViewMode.game)
                setActive(editorObjects, editorComponents, true);
        }
        catch (System.Exception e)
        {
        }
    }
}