using UnityEngine;
using System.Collections;

class AwardItemWhenTouch : MonoBehaviour 
{
    //public string itemName;
    public int itemID;
    public GameObject touchedShape;

    void Awake()
    {
        if (!zzCreatorUtility.isHost())
        {
            if (touchedShape)
                Destroy(touchedShape);
            Destroy(this);
        }
    }

    void OnCollisionEnter (Collision pCollisionInfo)
    {
        impAward(pCollisionInfo.gameObject);
    }

    void impAward(GameObject pOther)
    {
        zzItemBagControl lItemBagControl
            = pOther.GetComponent<zzItemBagControl>();
        if (lItemBagControl)
        {

            if (!lItemBagControl.isFull)
                lItemBagControl.addItemOne(itemID);
            Life.getLifeFromTransform(gameObject.transform).makeDead();
        }

    }

    void OnTriggerEnter (Collider pOther)
    {
        impAward(pOther.gameObject);

    }
}