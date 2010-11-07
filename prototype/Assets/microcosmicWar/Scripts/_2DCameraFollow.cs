using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机平滑跟随目标
/// </summary>
public class _2DCameraFollow : MonoBehaviour
{
    // The target we are following
    public Transform target;

    // Smooth switcher
    public bool useSmooth = true;

    // 位移量;一帧内的逻辑, 可以看成要花positionDamping的时间,到达目标
    public float positionDamping = 0.74f;

    //void Start()
    //{

    //}

    /// <summary>
    /// 设置跟踪目标
    /// </summary>
    /// <param name="pTarget">要跟踪目标</param>
    public void setTaget(Transform pTarget)
    {
        target = pTarget;
    }

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
        return tranF(OV, target.position.x, transform.position.x);
    }

    /// <summary>
    /// 每帧调用,获得y轴上的平滑过渡值
    /// </summary>
    /// <param name="OV">向目标值的y偏移量</param>
    /// <returns></returns>
    protected float tranFY(float OV)
    {
        return tranF(OV, target.position.y, transform.position.y);
    }

    /// <summary>
    /// 是否要移动的判断
    /// </summary>
    /// <returns></returns>
    protected bool TarTranNotEqual()
    {
        if (transform.position.x != target.position.x || transform.position.y != target.position.y)
            return true;
        else
            return false;
    }

    void Update()
    {
        if (target)
        {
            if (useSmooth)
            {
                float translationx;
                float translationy;

                if (Input.GetButton("left"))
                {
                    translationx = tranFX(-10.0f);
                    if (Input.GetButton("up"))
                    {
                        translationy = tranFY(4.0f);
                    }
                    else if (Input.GetButton("down"))
                    {
                        translationy = tranFY(-4.0f);
                    }
                    else
                        translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("right"))
                {
                    translationx = tranFX(10.0f);
                    if (Input.GetButton("up"))
                    {
                        translationy = tranFY(4.0f);
                    }
                    else if (Input.GetButton("down"))
                    {
                        translationy = tranFY(-4.0f);
                    }
                    else
                        translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("up"))
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(4.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("down"))
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(-4.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                //transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, timeDamping);
                //transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 1.74f);
                //transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 1.74f); 
                //transform.position = target.position;
            }
            else
            {
                Vector3 lTargetPos = target.position;
                lTargetPos.z = 0.0f;
                transform.position = lTargetPos;
                //transform.position.x = target.position.x;
                //transform.position.y = target.position.y;
            }
        }
    }
}
