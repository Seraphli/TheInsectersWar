using UnityEngine;

public class NetworkScopeObject:MonoBehaviour
{
    void Start()
    {
        var lSceneManager = GameSceneManager.Singleton;
        addTo(lSceneManager.getManager(Race.ePismire,
            GameSceneManager.UnitManagerType.heroSpawn).managerRoot);
        addTo(lSceneManager.getManager(Race.eBee,
            GameSceneManager.UnitManagerType.heroSpawn).managerRoot);
    }

    void addTo(Transform pTransform)
    {
        foreach (Transform lSub in pTransform)
        {
            foreach (var lBoundNetworkScope in 
                lSub.GetComponentsInChildren<BoundNetworkScope>())
            {
                lBoundNetworkScope.addScopeNetView(networkView);
            }
        }
    }
}