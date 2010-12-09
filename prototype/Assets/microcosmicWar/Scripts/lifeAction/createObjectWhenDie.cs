
using UnityEngine;
using System.Collections;

public class createObjectWhenDie : MonoBehaviour
{


    public GameObject objectToCreate;
    public Life life;
    public bool onlyInHost = false;

    void Start()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        if ( (!onlyInHost) || zzCreatorUtility.isHost())
            Instantiate(objectToCreate, transform.position, transform.rotation);
    }
}