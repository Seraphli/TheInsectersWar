
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

    IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        //initCheck();
        //print(originalPosition);
        if (!inited)
        {
            gameObject.layer = layers.boardDetector;
            playerCollider = boardPlayer.collider;

            Board.turnOffCollisionWithAllBaord(boardPlayer);
            originalPosition = transform.localPosition;
            inited = true;

        }
    }
    
    //延迟到碰撞体active了,再关闭与板的碰撞
    //void initCheck()
    //{
    //    if(!inited)
    //    {
    //        gameObject.layer = layers.boardDetector;
    //        playerCollider = boardPlayer.collider;

    //        Board.turnOffCollisionWithAllBaord(boardPlayer);
    //        originalPosition = transform.localPosition;
    //        inited = true;

    //    }
    //}

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
        print("OnTriggerEnter:" + other.name);
        //Board lBoard = other.GetComponent<Board>();
        //if (lBoard)
        //{
        //    print(lBoard.boardColliders.Length);
        //    lBoard.turnOnCollision(other,boardPlayer);
        //}
        if (!inited)
        {
            gameObject.layer = layers.boardDetector;
            playerCollider = boardPlayer.collider;

            Board.turnOffCollisionWithAllBaord(boardPlayer);
            originalPosition = transform.localPosition;
            inited = true;
            if (other.gameObject.layer == layers.board)
                Physics.IgnoreCollision(other, playerCollider, false);

        }
        else
            Physics.IgnoreCollision(other, playerCollider,false);
    }


    void OnTriggerExit(Collider other)
    {
        print("OnTriggerExit:" + other.name);
        //Board lBoard = other.GetComponent<Board>();
        //if (lBoard)
        //{
        //    print(lBoard.boardColliders.Length);
        //    lBoard.turnOffCollision(other,boardPlayer);
        //}
        Physics.IgnoreCollision(other, playerCollider);
    }

}