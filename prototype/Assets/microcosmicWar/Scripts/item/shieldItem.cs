
using UnityEngine;
using System.Collections;

class shieldItem : IitemObject
{
    public GameObject useObject;

    public GameObject shieldObject;

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
        GameObject lShield = (GameObject)GameObject.Instantiate(shieldObject);
        lShield.GetComponent<Shield>().adversaryWeaponLayer
            = PlayerInfo.getAdversaryRaceBulletLayer(useObject.layer);
        lShield.transform.parent = useObject.transform;
        lShield.transform.localPosition = Vector3.zero;

        //在一段时间后删除
        zzCoroutineTimer lTimer = lShield.AddComponent<zzCoroutineTimer>();
        lTimer.setInterval(duration);
        lTimer.setImpFunction(
            delegate()
            {
                Object.Destroy(lTimer);
                Object.Destroy(lShield);
            }
        );
        //lLifeRecover.setLife(lLife);
    }
};