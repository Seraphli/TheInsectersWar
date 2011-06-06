
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

    public override void use()
    {
        //Life lLife = useObject.GetComponent<Life>();
        //Shield lShield = useObject.AddComponent<Shield>();
        GameObject lShieldObject 
            = (GameObject)zzCreatorUtility.Instantiate(shieldObject,Vector3.zero,Quaternion.identity,0);
        var lAdversaryWeaponLayer = PlayerInfo.getAdversaryRaceBulletLayer(useObject.layer);
        Shield lShield = lShieldObject.GetComponent<Shield>();
        lShield.adversaryWeaponLayer = lAdversaryWeaponLayer;

        lShield.setOwner(useObject);

        lShieldObject.GetComponent<EffectOfShield>().filterLayer = lAdversaryWeaponLayer;
        //在一段时间后删除
        zzCoroutineTimer lTimer = lShieldObject.AddComponent<zzCoroutineTimer>();
        lTimer.setInterval(duration);
        lTimer.setImpFunction(
            delegate()
            {
                Object.Destroy(lTimer);
                zzCreatorUtility.Destroy(lShieldObject);
            }
        );
        //lLifeRecover.setLife(lLife);
    }
};