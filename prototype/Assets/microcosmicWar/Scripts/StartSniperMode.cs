using UnityEngine;
using System.Collections;


/// <summary>
/// 将此代码文件放置在英雄身上 进入狙击台或使用狙击枪的时候启动狙击模式
/// </summary>
public class StartSniperMode : MonoBehaviour
{


    //地图的一个重要对象 
    public GameObject SystemObject;
    //场景中的摄像机
    public GameObject mainCamera;

    //摄像机组件
    private Camera cameraT;
    //保存先前的ActionCommandControl
    private ActionCommandControl lActionCommandControlTemp;
    //mainInput
    private mainInput IMainInput;
    //SniperMode
    private SniperMode lSniperMode;
    //狙击台
    private GameObject sniperObjectT;

    // Use this for initialization
    void Start()
    {
        //找到System对象
        if (!SystemObject)
        {
            SystemObject = GameObject.Find("System");
        }

        //找到摄像机
        if (!(mainCamera && cameraT))
        {
            mainCamera = GameObject.Find("Main Camera");
            cameraT = mainCamera.GetComponent<Camera>();
        }
        //找到SniperMode组件
        if (!lSniperMode)
        {
            lSniperMode = mainCamera.GetComponent<SniperMode>();
        }

        //找到mainInput组件
        if (!IMainInput)
        {
            IMainInput = SystemObject.GetComponent<mainInput>();
        }

        
    }

    // Update is called once per frame
    //在狙击台前跳跃的时候启动狙击
    void Update()
    {

        //检测看是否按下跳跃   
        if (Input.GetButtonDown("jump"))
        {
            
            if (!lSniperMode.getSniperBool())
            {
                
                //狙击镜未开启

                if (surveyBool())
                {
                    startSniper();
                }
            }
            else
            {
                //狙击镜已开启

                if (raycastGameObject()&&lSniperMode.checkSniperObject(sniperObjectT))
                {
                    closeSniper();
                }
            }
        }



    }

    //开启狙击
    private void startSniper()
    {

        lActionCommandControlTemp = IMainInput.actionCommandControl;
        IMainInput.actionCommandControl = null;
        reset();
        lSniperMode.changeSniper(sniperObjectT,gameObject);

    }


    //关闭狙击
    private void closeSniper()
    {

        IMainInput.actionCommandControl = lActionCommandControlTemp;
        reset();
        lSniperMode.changeSniper();
        
        //print()

    }

    //摄像机复位到英雄当前坐标
    private void reset()
    {
        Vector3 v3 = gameObject.transform.position;
        v3.z = -10f;
        mainCamera.transform.position = v3;
    }

    //检查是否处于狙击台 狙击台是否开启
    private bool surveyBool()
    {

        if (raycastGameObject())
        {
            sniperObjectT = raycastGameObject();
            if (sniperObjectT.GetComponent<SniperEntrance>().getStatus())
            {
                return true;
            }
        }

        return false;
    }

    //线投检测狙击塔
    //返回狙击台
    private GameObject raycastGameObject()
    {
        //线投检测英雄当前位置的物体
        RaycastHit[] hits;
        Vector3 vc3 = transform.position;
        vc3.z -= 50;
        hits = Physics.RaycastAll(vc3, transform.forward, 100.0F);
        if (hits.Length > 0)
        {
            for (int i = 0; i <=(hits.Length - 1); i++)
            {
                //一直搜寻父物体 看是否狙击台 知道最父层物体
                Transform hit = hits[i].transform;
                do
                {
                    if (hit.tag == "sniper")
                    {
                        return hit.gameObject;
                    }
                    hit = hit.parent;
                }
                while (hit);
            }
        }
        return null;
    }
}
