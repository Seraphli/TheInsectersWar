
using UnityEngine;
using System.Collections;

public class createObjectWhenDie : MonoBehaviour
{


    public GameObject objectToCreate;
    public Life life;

    void Start()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        Instantiate(objectToCreate, transform.position, transform.rotation);
    }
}