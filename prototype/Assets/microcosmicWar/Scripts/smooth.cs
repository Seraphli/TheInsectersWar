using UnityEngine;
using System.Collections;

public class smooth : MonoBehaviour
{

    // The target we are following
    public Vector3 target;

    // Smooth switcher
    public Vector3 local;

    public GameObject mainCamera;
    public float range = 100f;

    // 位移量;一帧内的逻辑, 可以看成要花positionDamping的时间,到达目标
    public float positionDamping = 0.74f;

    //void Start()
    //{

    //}


    /// <summary>
    /// 每帧调用,获得平滑过渡值
    /// </summary>
    /// <param name="OffsetValue">向目标值的偏移量,用于摄像机偏移到转向的位置</param>
    /// <param name="lastValue">目标值</param>
    /// <param name="nowValue">目前的值</param>
    /// <returns></returns>
    protected float tranF(float OffsetValue, float lastValue, float nowValue)
    {
        //要花positionDamping的时间,到达目标位置
        return (lastValue + OffsetValue /* Time.deltaTime*/ - nowValue) / positionDamping * Time.deltaTime;
    }

    /// <summary>
    /// 每帧调用,获得x轴上的平滑过渡值
    /// </summary>
    /// <param name="OV">向目标值的x偏移量</param>
    /// <returns></returns>
    protected float tranFX(float OV)
    {
        return tranF(OV, target.x, local.x);
    }

    /// <summary>
    /// 每帧调用,获得y轴上的平滑过渡值
    /// </summary>
    /// <param name="OV">向目标值的y偏移量</param>
    /// <returns></returns>
    protected float tranFY(float OV)
    {
        return tranF(OV, target.y, local.y);
    }

    /// <summary>
    /// 是否要移动的判断
    /// </summary>
    /// <returns></returns>
    protected bool TarTranNotEqual()
    {
        if (local.x != target.x || local.y != target.y)
            return true;
        else
            return false;
    }

    void Update()
    {
        if (target != null && target != null)
        {
            float translationx=0f;
            float translationy=0f;

            if (Input.GetButton("left"))
            {
                mainCamera.transform.Translate(-10f * Time.deltaTime, 0, 0);

                translationx = tranFX(-range);
                if (Input.GetButton("up"))
                {
                    mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                    translationy = tranFY(-range);
                }
                else if (Input.GetButton("down"))
                {
                    mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                    translationy = tranFY(range);
                }
                else
                {
                    //translationy = tranFY(0.0f);
                }

                //local.x += translationx;
                //local.y += translationy;

            }
            else if (Input.GetButton("right"))
            {
                mainCamera.transform.Translate(10f * Time.deltaTime, 0, 0);

                translationx = tranFX(range);
                if (Input.GetButton("up"))
                {
                    mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                    translationy = tranFY(-range);
                }
                else if (Input.GetButton("down"))
                {
                    mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                    translationy = tranFY(range);
                }
                //else
                //{
                //    //translationy = tranFY(0.0f);
                //}

                //local.x += translationx;
                //local.y += translationy;
            }
                
            else if (Input.GetButton("up"))
            {
                //translationx = tranFX(0.0f);
                translationy = tranFY(-range);
                mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                //local.x += translationx;
                //local.y += translationy;
            }
            else if (Input.GetButton("down"))
            {
                //translationx = tranFX(0.0f);
                translationy = tranFY(range);
                mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                //local.x += translationx;
                //local.y += translationy;

            }
            else
            {


                //translationx = tranFX(0.0f);
                //translationy = tranFY(0.0f);

                //local.x += translationx;
                //local.y += translationy;

            }


            local.x += translationx;
            local.y += translationy;


            //print(translationx);
            //if (translationx >= 0)
            //{
            //    local.x += translationx;
            //}
            //else
            //{
            //    mainCamera.transform.Translate(translationx, 0, 0);
            //}

            //if (translationy >= 0)
            //{
            //    local.y += translationy;
            //}
            //else
            //{
            //    mainCamera.transform.Translate(0, translationy, 0);
            //}

        }
        //print(local.x);
    }
}
