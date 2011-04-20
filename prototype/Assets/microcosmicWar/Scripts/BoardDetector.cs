
using UnityEngine;
using System.Collections;

public class BoardDetector : MonoBehaviour
{


    public GameObject boardPlayer;

    [SerializeField]
    Vector3 originalPosition;

    [SerializeField]
    Collider playerCollider;

    //用于探测板是否在脚下
    protected bool inited = false;

    void Start()
    {
        gameObject.layer = layers.boardDetector;
        playerCollider = boardPlayer.collider;
        Board.turnOffCollisionWithAllBaord(boardPlayer);
        originalPosition = transform.localPosition;
        inited = true;
        //print(originalPosition);
    }

    public void down()
    {
        if (inited)
        {
            Vector3 lPos = transform.localPosition;
            lPos.y = originalPosition.y - 2;
            transform.localPosition = lPos;
            //transform.localPosition.y = originalPosition.y - 1;
        }
        //print("down:"+transform.localPosition.y);
    }

    public void recover()
    {

        if (inited)
        {
            Vector3 lPos = transform.localPosition;
            lPos.y = originalPosition.y;
            transform.localPosition = lPos;
            //transform.localPosition.y = originalPosition.y;
        }
        //print("recover:"+transform.localPosition.y);
    }

    void OnTriggerEnter(Collider other)
    {
        //print("OnTriggerEnter:" + other.name);
        //Board lBoard = other.GetComponent<Board>();
        //if (lBoard)
        //{
        //    print(lBoard.boardColliders.Length);
        //    lBoard.turnOnCollision(other,boardPlayer);
        //}
        Physics.IgnoreCollision(other, playerCollider,false);
    }


    void OnTriggerExit(Collider other)
    {
        //print("OnTriggerExit:" + other.name);
        //Board lBoard = other.GetComponent<Board>();
        //if (lBoard)
        //{
        //    print(lBoard.boardColliders.Length);
        //    lBoard.turnOffCollision(other,boardPlayer);
        //}
        Physics.IgnoreCollision(other, playerCollider);
    }

}