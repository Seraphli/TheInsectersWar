using UnityEngine;
using System.Collections;

public class WMGameObjectType:MonoBehaviour
{
    public Race race = Race.eNone;

    public GameSceneManager.UnitManagerType unitType
        = GameSceneManager.UnitManagerType.none;

    public GameSceneManager.MapManagerType mapType
        = GameSceneManager.MapManagerType.none;
}