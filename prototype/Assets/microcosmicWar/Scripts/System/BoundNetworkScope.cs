using UnityEngine;

public class BoundNetworkScope : MonoBehaviour
{
    public Transform[] networkPlayerRoots;

    System.Func<Bounds> getBoundsFunc;

    public NetworkPlayer networkPlayer;

    public void setGetBoundsFunc(System.Func<Bounds> pFunc)
    {
        getBoundsFunc = pFunc;
    }

    public void updateScope() 
    { 
        updateScope(getBoundsFunc()); 
    }

    public void updateScope(Bounds pBounds)
    {
        foreach (Transform lNetworkPlayerRoot in networkPlayerRoots)
        {
            setScope(pBounds, lNetworkPlayerRoot, networkPlayer);
        }
    }

    public static void setScope(Bounds pBounds, Transform pParent,NetworkPlayer pPlayer)
    {
        foreach (Transform lTransform in pParent)
        {
            lTransform.networkView.SetScope(pPlayer,pBounds.Contains(lTransform.position));
        }
    }
}