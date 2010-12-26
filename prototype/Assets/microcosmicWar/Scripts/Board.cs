
using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{


    //用于2D游戏中,穿透可跳上去跳下来的板

    static ArrayList sBoardList = new ArrayList();

    public static void turnOffCollisionWithAllBaord(Collider pCollider)
    {
        //print(sBoardList);
        foreach (GameObject i in sBoardList)
        {
            Physics.IgnoreCollision(i.collider, pCollider);
        }
    }

    public static void turnOffCollisionWithAllBaord(GameObject pGameObject)
    {
        turnOffCollisionWithAllBaord(pGameObject.collider);
    }

    //记得在场景结束前调用
    public static void clearList()
    {
        sBoardList.Clear();
    }

    void Awake()
    {
        sBoardList.Add(gameObject);
    }

    public void turnOffCollision(GameObject pGameObject)
    {
        Physics.IgnoreCollision(gameObject.collider, pGameObject.collider);
    }

    public void turnOnCollision(GameObject pGameObject)
    {
        Physics.IgnoreCollision(gameObject.collider, pGameObject.collider, false);
    }

    public static void addBoardInAllChild(GameObject pGameObject)
    {
        if (pGameObject.GetComponent<Collider>())
            pGameObject.AddComponent<Board>();
        foreach (Transform lChild in pGameObject.transform)
        {
            addBoardInAllChild(lChild.gameObject);
        }
    }

    public static void removeBoardInAllChild(GameObject pGameObject)
    {
        Board lBoard = pGameObject.GetComponent<Board>();
        if (lBoard)
            Object.DestroyImmediate(lBoard);
        foreach (Transform lChild in pGameObject.transform)
        {
            removeBoardInAllChild(lChild.gameObject);
        }
    }


    //void Start()
    //{
    //    //用于子弹的穿透
    //    collisionLayer.addCollider(gameObject);
    //}
}