
using UnityEngine;
using System.Collections;

public class SoldierFactory : MonoBehaviour
{


    public string adversaryName = "";

    public Transform finalAim;

    public float produceInterval = 1.0f;

    public GameObject soldierToProduce;

    protected float timePos = 0.0f;

    protected int adversaryLayerValue;

    public Transform produceTransform;

    //Component.SendMessage ("dieCallFunction")
    //Component dieCallFunction;
    public IobjectListener objectListener;

    void Start()
    {
        if (!produceTransform)
            produceTransform = transform;

        collisionLayer.addCollider(gameObject);

        adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);

        Life lLife = gameObject.GetComponent<Life>();
        //lLife.setDieCallback(dieCall);
        lLife.addDieCallback(dieCall);

        if (!zzCreatorUtility.isHost())
            Destroy(this);
    }

    void Update()
    {
        //if(zzCreatorUtility.isHost())
        //{
        timePos += Time.deltaTime;
        if (timePos > produceInterval)
        {
            //FIXME_VAR_TYPE lClone= Network.Instantiate(soldierToProduce, transform.position+Vector3(0,2.5f,0), Quaternion(), 0);
            GameObject lClone = zzCreatorUtility.Instantiate(soldierToProduce, 
                produceTransform.position,new Quaternion(), 0);
            timePos = 0.0f;
            SoldierAI soldierAI = lClone.GetComponent<SoldierAI>();
            soldierAI.SetFinalAim(finalAim);
            soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
            //lClone.GetComponent<SoldierAI>().SetSoldier(lClone.GetComponent<Soldier>());
            //lClone.GetComponent<SoldierAI>().SetAdversaryLayerValue(adversaryLayerValue);
        }
        //}
    }

    public void dieCall(Life p)
    {
        //if(dieCallFunction)
        //	dieCallFunction.SendMessage ("dieCallFunction");
        //else
        if (objectListener!=null)
            objectListener.removedCall();
        Destroy(gameObject);
        //GameScene.getSingleton().gameResult(adversaryName);
    }
}