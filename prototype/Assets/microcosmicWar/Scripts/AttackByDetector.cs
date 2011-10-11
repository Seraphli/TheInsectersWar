using UnityEngine;
using System.Collections.Generic;

public class AttackByDetector:MonoBehaviour
{
    public zzDetectorBase detector;

    public int attack = 150;

    HashSet<Life> lifeList = new HashSet<Life>();

    [SerializeField]
    WMPurse _purse;

    public WMPurse purse
    {
        get { return _purse; }
        set { _purse = value; }
    }

    public void doAttack()
    {
        if (Network.isClient)
            return;

        lifeList.Clear();
        var lDetecteds = detector.detect();
        foreach (var lDetected in lDetecteds)
        {
            lifeList.Add(Life.getLifeFromTransform(lDetected.transform));
        }
        foreach (var lLife in lifeList)
        {
            lLife.injure(attack, _purse);
        }
    }
}