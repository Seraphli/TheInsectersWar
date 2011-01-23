
using UnityEngine;
using System.Collections;

public class DefenseTowerAim : MonoBehaviour
{
    //判断目标是否可用
    public delegate bool aimIsActiveFunc(Transform pTarget);

    aimIsActiveFunc aimIsActive;

    /// <summary>
    /// 设置判断是否目标可跟踪的函数
    /// </summary>
    /// <param name="pFunc">若为null,则允许跟踪全部可用的Transform(默认)</param>
    public void setAimIsActiveFunc(aimIsActiveFunc pFunc)
    {
        if (pFunc!=null)
            aimIsActive = pFunc;
        else
            aimIsActive = allIsActive;
    }

    bool allIsActive(Transform pTarget)
    {
        return true;
    }

    public Transform target;
    public Bullet bullet;
    public float fireDeviation = 4.0f;
    public DefenseTower defenseTower;

    void Start()
    {
        if (!defenseTower)
            defenseTower = GetComponent<DefenseTower>();
    }

    public void setTarget(Transform pTarget)
    {
        target = pTarget;
    }

    void Update()
    {
        if (target && aimIsActive(target))
        {
            defenseTower.takeAim(target.position, fireDeviation);
        }
    }
}