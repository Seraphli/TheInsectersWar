﻿
using UnityEngine;
using System.Collections;



public class zzGUIContainer : zzInterfaceGUI
{
    public override void impGUI()
    {
        impSubs();
    }


    public virtual void impSubs()
    {
        ArrayList lGUIlist = new ArrayList();
        foreach (Transform lTransform in transform)
        {
            zzGUI impGUI = lTransform.GetComponent<zzGUI>();
            if (impGUI)
            {
                //按深度排序,小的排在前面,先被渲染,会被深度大的遮住
                for (int i = 0; i < lGUIlist.Count; ++i)
                {
                    zzGUI lGUIlistTemp = (zzGUI)lGUIlist[i];
                    if (impGUI.getDepth() < lGUIlistTemp.getDepth())
                    {
                        lGUIlist.Insert(i, impGUI);
                        impGUI = null;//来说明已添加
                        break;
                    }
                }
                if (impGUI)
                    lGUIlist.Add(impGUI);
            }
        }

        //print("********************");
        foreach (zzGUI imp in lGUIlist)
        {
            //print(imp.gameObject.name+" "+imp.getDepth());
            imp.impGUI();
        }
    }
}