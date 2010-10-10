
using UnityEngine;
using System.Collections;

public class BoardDetector : MonoBehaviour
{


    public GameObject boardPlayer;
    protected Vector3 originalPosition;
    //用于探测板是否在脚下
    protected bool inited = false;

    void Start()
    {
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
            lPos.y = originalPosition.y - 1;
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
        //print("OnTriggerEnter");
        Board lBoard = other.GetComponent<Board>();
        if (lBoard)
        {
            lBoard.turnOnCollision(boardPlayer);
        }
    }


    void OnTriggerExit(Collider other)
    {
        //print("OnTriggerExit");
        Board lBoard = other.GetComponent<Board>();
        if (lBoard)
        {
            lBoard.turnOffCollision(boardPlayer);
        }
    }

}