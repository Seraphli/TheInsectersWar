using UnityEngine;
using System.Collections;

public enum GameObjectType
{
    virtualObject,
    generalObject,
}


public class GameConfig
{
    int virtualObjectLayer;
    int generalObjectLayer;

    GameConfig()
    {
        virtualObjectLayer = LayerMask.NameToLayer("virtualObject");
        generalObjectLayer = LayerMask.NameToLayer("generalObject");
    }

    public int getLayer(GameObjectType pType)
    {
        switch (pType)
        {
            case GameObjectType.virtualObject:return virtualObjectLayer;
                    break;
            case GameObjectType.generalObject: return generalObjectLayer;
                    break;
        }
        throw new System.InvalidOperationException("Invalid GameObjectType");
        return -1;
    }

    public string ModelDir = Application.dataPath + "/../Models";

    static protected GameConfig singletonInstance = new GameConfig();

    public static GameConfig Singleton
    {
        get { return singletonInstance; }
    }

    public static GameConfig getSingleton()
    {
        return singletonInstance;
    }

    //void Awake()
    //{
    //    if (singletonInstance!=null)
    //        Debug.LogError("have singletonInstance");
    //    singletonInstance = this;
    //}

}