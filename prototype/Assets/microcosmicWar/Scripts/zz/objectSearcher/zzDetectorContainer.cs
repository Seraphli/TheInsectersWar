
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class zzDetectorContainer : zzDetectorBase
{
    public zzDetectorBase[] subDetectorList;

    void Start()
    {
        //创建子探测器列表, subDetectorList; 按优先级排序
        ArrayList lDetectorList = new ArrayList();
        int i = 0;
        foreach (Transform lTransform in transform)
        {
            zzDetectorBase lDetector = lTransform.GetComponent<zzDetectorBase>();
            if (lDetector)
            {
                //按优先级排序,大的排在前面.先检测
                for (i = 0; i < lDetectorList.Count; ++i)
                {
                    zzDetectorBase lDetectorTemp = (zzDetectorBase)lDetectorList[i];
                    if (lDetector.getPriority() > lDetectorTemp.getPriority())
                    {
                        lDetectorList.Insert(i, lDetector);
                        lDetector = null;//来说明已添加
                        break;
                    }
                }
                if (lDetector)
                    lDetectorList.Add(lDetector);
            }
        }

        subDetectorList = new zzDetectorBase[lDetectorList.Count];
        for (i = 0; i < lDetectorList.Count; ++i)
        {
            subDetectorList[i] = (zzDetectorBase)lDetectorList[i];
        }
    }

    public override Collider[] detect()
    {
        var pMaxRequired = maxRequired;
        List<Collider> lOut = new List<Collider>();
        foreach (zzDetectorBase subDetector in subDetectorList)
        {
            Collider[] lSubResult = subDetector.detect();
            pMaxRequired -= lSubResult.Length;
            lOut.AddRange(lSubResult);
            if (pMaxRequired <= 0)
                break;
        }
        return lOut.ToArray();
    }

    public override Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        //Collider[] lOut = new Collider[0];
        List<Collider> lOut = new List<Collider>();
        foreach (zzDetectorBase subDetector in subDetectorList)
        {
            Collider[] lSubResult = subDetector.detect(pMaxRequired, pLayerMask,pNeedDetectedFunc);
            pMaxRequired -= lSubResult.Length;
            //lOut += lSubResult;
            lOut.AddRange(lSubResult);
            if (pMaxRequired <= 0)
                break;
        }
        return lOut.ToArray();
    }

    [ContextMenu("MakeSubSameData")]
    void makeSubSameData()
    {
        foreach (Transform lTransform in transform)
        {
            zzDetectorBase lDetector = lTransform.GetComponent<zzDetectorBase>();
            if (lDetector)
            {
                lDetector.layerMask = _layerMask;
                lDetector.maxRequired = maxRequired;
            }
            if (lDetector is zzDetectorContainer)
                ((zzDetectorContainer)lDetector).makeSubSameData();
        }
    }

}