
using UnityEngine;
using System.Collections;


public class layers
{
    public static readonly int pismire = LayerMask.NameToLayer("pismire");
    public static readonly int pismireValue = 1 << pismire;
    public static readonly int bee = LayerMask.NameToLayer("bee");
    public static readonly int beeValue = 1 << bee;
    public static readonly int pismireBullet = LayerMask.NameToLayer("pismireBullet");
    public static readonly int pismireBulletValue = 1 << pismireBullet;
    public static readonly int beeBullet = LayerMask.NameToLayer("beeBullet");
    public static readonly int beeBulletValue = 1 << beeBullet;

    public static readonly int pismireBuilding = LayerMask.NameToLayer("pismireBuilding");
    public static readonly int pismireBuildingValue = 1 << pismireBuilding;

    public static readonly int beeBuilding = LayerMask.NameToLayer("beeBuilding");
    public static readonly int beeBuildingValue = 1 << beeBuilding;

    public static readonly int deadObject = LayerMask.NameToLayer("deadObject");

    public static readonly int board = LayerMask.NameToLayer("board");
    public static readonly int boardValue = 1 << board;

    public static readonly int pismireMissile = LayerMask.NameToLayer("pismireMissile");
    public static readonly int beeMissile = LayerMask.NameToLayer("beeMissile");
}

public class collisionLayer : MonoBehaviour
{

    //static FIXME_VAR_TYPE mIgnoreList= new Hashtable();

    static ArrayList[] mLayersIgnoreList = new ArrayList[32];
    static ArrayList[] mLayersObjectList = new ArrayList[32];

    //离清理还差的步数
    protected static int nextStepToClear = 5;

    //清理 间隔步数
    protected static int intervalStepToClear = 5;

    public static void initialize()
    {
        for (int i = 0; i < 32; i += 1)
        {
            mLayersIgnoreList[i] = new ArrayList();
            mLayersObjectList[i] = new ArrayList();
        }
    }

    //检测某层中的物体是否可用,否则消除
    protected static void checkAndClear(ArrayList objectList)
    {
        //print("checkAndClear before"+objectList.length);
        for (int a = objectList.Count - 1; a >= 0; a -= 1)
        {
            //遍历此层物体
            //print("( int a = objectList.length - 1; a >= 0; a-=1 )");
            GameObject lCheckedObject = (GameObject)objectList[a];
            if (lCheckedObject == null || !lCheckedObject.active)
            {
                //objectList[a]=objectList.Pop();
                zzUtilities.quickRemoveArrayElement(objectList, a);
                //print(objectList[a].name);
                //Debug.LogWarning("checkAndClear(a)@@@@@@@@@@@@@@@@"+objectList.length+"  a:"+a);
            }
        }
        //print("checkAndClear end"+objectList.length);
    }

    protected static void checkAndClearStep(ArrayList objectList)
    {
        //	print("checkAndClearStep");
        nextStepToClear -= 1;
        if (nextStepToClear < 0)
        {
            //print("nextStepToClear<0");
            checkAndClear(objectList);
            nextStepToClear = intervalStepToClear;
        }
    }

    static void IgnoreCollisionBetween(int layer1Num, int layer2Num)
    {
        ////mIgnoreList[1 << layer1Num & 1 << layer2Num] = true;
        //if (layer1Num == layer2Num)
        //{
        //    mLayersIgnoreList[layer1Num].Add(layer1Num);
        //    return;
        //}
        //mLayersIgnoreList[layer1Num].Add(layer2Num);
        //mLayersIgnoreList[layer2Num].Add(layer1Num);
        Physics.IgnoreLayerCollision(layer1Num, layer2Num);
    }

    //目前只可用于deadObject
    public static void updateCollider(GameObject gameObject)
    {
        //print("@@@@@@@@@@@@@@@@  updateCollider");
        addCollider(gameObject);
    }
    public static void addCollider(GameObject gameObject)
    {

    }

    //public static void addCollider(GameObject gameObject)
    //{
    //    //if(gameObject.layer==layers.beeMissile)
    //    //	print("addCollider before:"+gameObject.name);
    //    if (gameObject.collider && gameObject.layer > 7)
    //    {
    //        //if(gameObject.layer==layers.beeMissile)
    //        //	print("addCollider :"+gameObject.name);
    //        ArrayList ignoreList = mLayersIgnoreList[gameObject.layer];
    //        //print(ignoreList);
    //        foreach (int i in ignoreList)
    //        {
    //            //遍历忽略的层
    //            //print("(int lOutIndex in ignoreList )"+gameObject.name);
    //            ArrayList objectList = mLayersObjectList[i];
    //            for (int a = objectList.Count - 1; a >= 0; a -= 1)
    //            {
    //                //遍历此层物体
    //                //print("( int a = objectList.length - 1; a >= 0; a-=1 )");
    //                GameObject aGameObject = (GameObject)objectList[a];
    //                if (aGameObject && aGameObject.active && aGameObject != gameObject)
    //                {
    //                    //print("Physics.IgnoreCollision "+gameObject.name+" "+objectList[a].name);
    //                    Physics.IgnoreCollision(gameObject.collider, aGameObject.collider);
    //                }
    //                else
    //                {
    //                    //Debug.LogWarning("objectList.RemoveAt(a)@@@@@@@@@@@@@@@@");
    //                    //objectList.RemoveAt(a);
    //                    //objectList[a]=objectList.Pop();
    //                    zzUtilities.quickRemoveArrayElement(objectList, a);
    //                }
    //            }
    //        }
    //        //print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
    //        checkAndClearStep(mLayersObjectList[gameObject.layer]);
    //        //print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
    //        mLayersObjectList[gameObject.layer].Add(gameObject);
    //        //print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);

    //    }

    //    //遍历子物体
    //    foreach (Transform child in gameObject.transform)
    //    {
    //        //print(child.name);
    //        addCollider(child.gameObject);
    //    }
    //}

    //从物体的所属层来判断是否存活
    public static bool isAlive(Transform pOwn)
    {
        if (pOwn.gameObject.layer == layers.deadObject)
            return false;
        return true;
    }

    public static bool isAliveFullCheck(Transform pOwn)
    {
        if (pOwn)
            return isAlive(pOwn);
        return false;
    }

    // Use this for initialization
    void Start()
    {
        //print("Start");

    }

    // Use this for initialization
    void Awake()
    {
        //print("Awake");
        //initialize();

        //print("layers.pismireMissile:"+layers.pismireMissile);
        //print("layers.beeMissile:"+layers.beeMissile);

        //子弹
        IgnoreCollisionBetween(layers.pismire, layers.pismireBullet);
        IgnoreCollisionBetween(layers.bee, layers.beeBullet);

        IgnoreCollisionBetween(layers.pismireBullet, layers.pismireBullet);
        IgnoreCollisionBetween(layers.beeBullet, layers.beeBullet);
        IgnoreCollisionBetween(layers.pismireBullet, layers.beeBullet);

        IgnoreCollisionBetween(layers.board, layers.pismireBullet);
        IgnoreCollisionBetween(layers.board, layers.beeBullet);

        //导弹
        IgnoreCollisionBetween(layers.pismire, layers.pismireMissile);
        IgnoreCollisionBetween(layers.pismireBullet, layers.pismireMissile);

        IgnoreCollisionBetween(layers.bee, layers.beeMissile);
        IgnoreCollisionBetween(layers.beeBullet, layers.beeMissile);

        IgnoreCollisionBetween(layers.board, layers.pismireMissile);
        IgnoreCollisionBetween(layers.board, layers.beeMissile);
        /*
        //防止可在子弹上跳跃
	
        IgnoreCollisionBetween(layers.characterShape,layers.characterShape);
        IgnoreCollisionBetween(layers.characterShape,layers.beeBullet);
        IgnoreCollisionBetween(layers.characterShape,layers.bee);
        IgnoreCollisionBetween(layers.characterShape,layers.pismireBullet);
        IgnoreCollisionBetween(layers.characterShape,layers.pismire);
        */
        if (!zzCreatorUtility.isHost())
        {
            //客户端子弹都可穿透  :判断都放在服务器端 , 客户端就不用管子弹层了
            IgnoreCollisionBetween(layers.pismire, layers.beeBullet);
            IgnoreCollisionBetween(layers.bee, layers.pismireBullet);
        }


        IgnoreCollisionBetween(layers.bee, layers.bee);
        IgnoreCollisionBetween(layers.pismire, layers.pismire);

        IgnoreCollisionBetween(layers.pismire, layers.bee);

        for (int i = 8; i < 32; i += 1)
        {
            IgnoreCollisionBetween(layers.deadObject, i);
        }
    }

}