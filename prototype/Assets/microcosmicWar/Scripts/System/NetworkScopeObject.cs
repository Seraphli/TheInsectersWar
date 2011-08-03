using UnityEngine;

public class NetworkScopeObject:MonoBehaviour
{
    //public bool setScopeWhenStart = false;

    void Start()
    {
        if (!Network.isServer)
            return;
        var lSceneManager = GameSceneManager.Singleton;

        //将自己增加到英雄的BoundNetworkScope中
        addTo(lSceneManager.getManager(Race.ePismire,
            GameSceneManager.UnitManagerType.heroSpawn).managerRoot);
        addTo(lSceneManager.getManager(Race.eBee,
            GameSceneManager.UnitManagerType.heroSpawn).managerRoot);
    }

    void addTo(Transform pTransform)
    {
        var lNetworkViews = GetComponents<NetworkView>();
        //if (setScopeWhenStart)
        //{
        //    var lPosition = transform.position;
        //    foreach (Transform lSub in pTransform)
        //    {
        //        //因为一个玩家重生点上,有多个英雄重生点
        //        foreach (var lBoundNetworkScope in
        //            lSub.GetComponentsInChildren<BoundNetworkScope>())
        //        {
        //            lBoundNetworkScope.addAndSetScopeNetView(lPosition, lNetworkViews);
        //        }
        //    }
        //}
        //else
        //{
            foreach (Transform lSub in pTransform)
            {
                //因为一个玩家重生点上,有多个英雄重生点
                foreach (var lBoundNetworkScope in
                    lSub.GetComponentsInChildren<BoundNetworkScope>())
                {
                    lBoundNetworkScope.addScopeNetView(lNetworkViews);
                }
            }

        //}
    }
}