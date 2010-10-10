
using UnityEngine;
using System.Collections;

public class DefenseTowerAim : MonoBehaviour
{

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
        if (target)
        {
            defenseTower.takeAim(target.position, fireDeviation);
        }
    }
}