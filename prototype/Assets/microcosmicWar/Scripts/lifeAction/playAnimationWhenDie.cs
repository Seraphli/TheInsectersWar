
using UnityEngine;
using System.Collections;

public class playAnimationWhenDie : MonoBehaviour
{

    public Animation animationToPlay;
    public string animationName;
    public int animationStateLayer = 10;
    public Life life;


    void Start()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
        animationToPlay[animationName].layer = animationStateLayer;
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        animationToPlay.CrossFade(animationName,0.3f);
    }
}