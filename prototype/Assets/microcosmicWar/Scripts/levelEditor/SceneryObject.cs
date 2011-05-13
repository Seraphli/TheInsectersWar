using UnityEngine;

public class SceneryObject:MonoBehaviour
{
    public Transform sceneryTransform;
    public GameObject renderObject;

    public int orthographicLayer;
    public int perspectiveLayer;

    public float minBackgroundDepth = 1f;
    public float maxBackgroundDepth = 2f;

    public float minForegroundDepth = -1f;
    public float maxForegroundDepth = -2f;

    void Start()
    {
        updateSceneryTransform();
    }

    void updateRenderLayer()
    {
        if (isPerspective)
            renderObject.layer = perspectiveLayer;
        else
            renderObject.layer = orthographicLayer;
    }

    void updateSceneryTransform()
    {
        var lPosition = sceneryTransform.position;
        lPosition.z = getSceneryTransformDepth();
        sceneryTransform.position = lPosition;
    }

    float getSceneryTransformDepth()
    {
        switch(_sceneryPosition)
        {
            case SceneryPosition.background:
                return Mathf.Lerp(minBackgroundDepth, maxBackgroundDepth, depth);

            case SceneryPosition.foreground:
                return Mathf.Lerp(minForegroundDepth, maxForegroundDepth, depth);
        }
        Debug.LogError("getSceneryTransformDepth");
        return 0f;
    }

    [SerializeField]
    float _depth;

    /// <summary>
    /// 0f-1f
    /// </summary>
    [zzSerialize]
    [FieldUI("深度", verticalDepth=0)]
    [SliderUI(0f, 1f, verticalDepth = 1)]
    public float depth
    {
        get
        {
            return _depth;
        }

        set
        {
            _depth = Mathf.Clamp01(value);
            updateSceneryTransform();
        }
    }

    public enum SceneryPosition
    {
        background,
        foreground,
    }

    [SerializeField]
    SceneryPosition _sceneryPosition = SceneryPosition.background;

    [zzSerialize]
    [EnumUI(new string[]{"背景","前景"},
        new int[] { (int)SceneryPosition.background, (int)SceneryPosition.foreground },
        verticalDepth = 3)]
    public SceneryPosition sceneryPosition
    {
        get
        {
            return _sceneryPosition;
        }

        set
        {
            if(_sceneryPosition!=value)
            {
                _sceneryPosition = value;
                updateSceneryTransform();
            }
        }
    }

    [SerializeField]
    bool _isPerspective = false;

    [zzSerialize]
    [FieldUI("透视", verticalDepth = 2, tooltip = "3D下的视角")]
    public bool isPerspective
    {
        get 
        { 
            return _isPerspective; 
        }

        set 
        { 
            _isPerspective = value;
            updateRenderLayer();
        }
    }
}