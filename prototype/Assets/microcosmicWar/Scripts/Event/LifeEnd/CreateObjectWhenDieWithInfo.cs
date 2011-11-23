
using UnityEngine;
using System.Collections;

public class CreateObjectWhenDieWithInfo : MonoBehaviour
{
    public GameObject objectToCreate;
    public Life life;
    public bool onlyInHost = false;
    public CharacterInfoObject characterInfoObject;

    void Awake()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        if ((!onlyInHost) || (!Network.isClient))
        {
            var lObject = (GameObject)Instantiate(objectToCreate,
                transform.position, transform.rotation);
            lObject.GetComponent<CharacterInfoObject>().characterInfo
                = characterInfoObject.characterInfo;
        }
    }
}