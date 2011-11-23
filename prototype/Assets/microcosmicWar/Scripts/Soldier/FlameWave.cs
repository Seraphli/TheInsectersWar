using UnityEngine;

public class FlameWave:Bullet
{
    public GameObject flamePrefab;
    public string flameObjectName = "burn";

    protected override void _touch(Transform pOther)
    {
        if (pOther.gameObject.layer == layers.ground)
        {
            lifeEnd();
            return;
        }

        Life lLife = Life.getLifeFromTransform(pOther);

        if (lLife)
        {
            if (zzCreatorUtility.isHost())
                lLife.injure(harmVale);
            onFire(lLife.transform);
        }
    }

    void onFire(Transform pOther)
    {
        if (!pOther.GetComponent<Soldier>())
            return;
        var lCenter = pOther.GetComponent<zzSceneObjectMap>().getObject("center").transform;
        var lFlameTransform = lCenter.FindChild(flameObjectName);
        if (!lFlameTransform)
        {
            var lEffectObject = (GameObject)Object.Instantiate(flamePrefab,
                lCenter.position, Quaternion.identity);
            lEffectObject.name = flameObjectName;
            lFlameTransform = lEffectObject.transform;
            lFlameTransform.parent = lCenter;
            lEffectObject.GetComponent<LifeIntervalChange>().life = pOther.GetComponent<Life>();
        }
        lFlameTransform.FindChild("OnEffect").GetComponent<zzOnAction>().impAction();

    }

}