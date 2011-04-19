
using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{


    //用于2D游戏中,穿透可跳上去跳下来的板

    //static ArrayList sBoardList = new ArrayList();

    public static void turnOffCollisionWithAllBaord(Collider pCollider)
    {
        //print(sBoardList);
        //foreach (GameObject i in sBoardList)
        //{
        //    Physics.IgnoreCollision(i.collider, pCollider);
        //}
        foreach(Transform lBoard in GameSceneManager.Singleton
            .getManager(GameSceneManager.MapManagerType.board))
        {
            lBoard.GetComponent<Board>().turnOffCollision(pCollider);

        }
    }

    public static void turnOffCollisionWithAllBaord(GameObject pGameObject)
    {
        turnOffCollisionWithAllBaord(pGameObject.collider);
    }

    //记得在场景结束前调用
    //public static void clearList()
    //{
    //    //sBoardList.Clear();
    //}

    //void Awake()
    //{
    //    sBoardList.Add(gameObject);
    //}
    static void turnOffCollisionWithManager(zzSceneManager pSceneManager, Collider[] pColliders)
    {
        foreach (Transform lObject in pSceneManager)
        {
            var lObjectCollider = lObject.collider;
            foreach (var lCollider in pColliders)
            {
                Physics.IgnoreCollision(lObjectCollider, lCollider);
            }
        }
    }
    void turnOffCollisionWithManager(zzSceneManager pSceneManager)
    {
        turnOffCollisionWithManager(pSceneManager, boardColliders);
    }

    public Collider[] boardColliders = new Collider[0];

    void Start()
    {
        if (boardColliders.Length == 0)
            boardColliders = new Collider[1] { collider };

        var lGameSceneManager = GameSceneManager.Singleton;

        turnOffCollisionWithManager( 
            lGameSceneManager.getManager(Race.ePismire, 
            GameSceneManager.UnitManagerType.hero)
        );
        turnOffCollisionWithManager(
            lGameSceneManager.getManager(Race.ePismire,
            GameSceneManager.UnitManagerType.soldier)
        );

        turnOffCollisionWithManager(
            lGameSceneManager.getManager(Race.eBee,
            GameSceneManager.UnitManagerType.hero)
        );
        turnOffCollisionWithManager(
            lGameSceneManager.getManager(Race.eBee,
            GameSceneManager.UnitManagerType.soldier)
        );
    }

    public void turnOffCollision(GameObject pGameObject)
    {
        turnOffCollision(pGameObject.collider);
    }

    public void turnOffCollision(Collider pObjectCollider)
    {
        foreach (var lBoardCollider in boardColliders)
        {
            Physics.IgnoreCollision(lBoardCollider, pObjectCollider);
        }
    }

    public void turnOffCollision(Collider pBoardCollider, GameObject pGameObject)
    {
        Physics.IgnoreCollision(pBoardCollider, pGameObject.collider);
    }

    public void turnOnCollision(Collider pBoardCollider, GameObject pGameObject)
    {
        Physics.IgnoreCollision(pBoardCollider, pGameObject.collider, false);
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