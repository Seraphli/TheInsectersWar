
using UnityEngine;
using System.Collections;

class shieldItem : IitemObject
{
    public GameObject useObject;

    public GameObject shieldObject;

    //public GameObject networkObjectPrefab;

    //效果持续时间
    public float duration = 30.0f;

    public override bool canUse(GameObject pGameObject)
    {
        useObject = pGameObject;
        return true;
    }

    [RPC]
    void ShieldRPCUse(NetworkViewID pViewID,NetworkMessageInfo pInfo)
    {
        ShieldItemUse(NetworkView.Find(pViewID).gameObject,
            duration - (float)(Network.time - pInfo.timestamp));
    }

    void ShieldItemUse(GameObject pOwner,float pDuration)
    {
        GameObject lShieldObject 
            = (GameObject)Instantiate(shieldObject);
        var lAdversaryWeaponLayer = PlayerInfo.getAdversaryRaceBulletLayer(pOwner.layer);
        Shield lShield = lShieldObject.GetComponent<Shield>();
        lShield.adversaryWeaponLayer = lAdversaryWeaponLayer;

        lShield.setOwner(pOwner);

        lShieldObject.GetComponent<EffectOfShield>().filterLayer = lAdversaryWeaponLayer;
        //在一段时间后删除
        zzCoroutineTimer lTimer = lShieldObject.AddComponent<zzCoroutineTimer>();
        lTimer.setInterval(pDuration);
        lTimer.setImpFunction(
            delegate()
            {
                Object.Destroy(lTimer);
                Destroy(lShieldObject);
            }
        );

    }

    public override void use()
    {
        ShieldItemUse(useObject, duration);
        if (Network.isServer)
            networkView.RPC("ShieldRPCUse", RPCMode.Others, useObject.networkView.viewID);
        //lLifeRecover.setLife(lLife);
    }
};