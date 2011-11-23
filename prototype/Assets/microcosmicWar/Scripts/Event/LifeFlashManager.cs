using UnityEngine;
using System.Collections;

public class LifeFlashManager : MonoBehaviour
{

    #region SingletonInstance
    static LifeFlashManager singletonInstance;

    public static LifeFlashManager Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance != null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
    }
    #endregion

    public Color selfAttackedColor;
    public Color attackedBySelfColor;
    public Color attackedByTeammateColor;
}
