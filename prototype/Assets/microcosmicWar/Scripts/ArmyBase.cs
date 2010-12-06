
using UnityEngine;
using System.Collections;

public class ArmyBase : MonoBehaviour
{
    public string adversaryName = "";

    public Transform finalAim;

    public int adversaryLayerValue;

    public Transform produceTransform;

    public IobjectListener objectListener;


    void Start()
    {

        adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);

        if (!produceTransform)
            produceTransform = transform;

        Life lLife = gameObject.GetComponent<Life>();
        //lLife.setDieCallback(dieCall);
        lLife.addDieCallback(dieCall);
    }

    public void dieCall(Life p)
    {
        if (objectListener != null)
            objectListener.removedCall();
        Destroy(gameObject);
    }

}