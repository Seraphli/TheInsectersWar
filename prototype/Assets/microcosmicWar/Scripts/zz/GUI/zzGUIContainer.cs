﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class zzGUIContainer : zzInterfaceGUI
{

    public override void impGUI(Rect rect)
    {
        impSubs();
    }

    /// <summary>
    /// 鼠标指针是否在此UI上的判断
    /// </summary>
    public virtual bool isCursorOver
    {
        get
        {
            foreach (Transform lTransform in transform)
            {
                zzGUIContainer lContainer = lTransform.GetComponent<zzGUIContainer>();
                if (lContainer && lContainer.visible && lContainer.isCursorOver)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 鼠标指针是否不在在此UI上的判断
    /// </summary>
    public virtual bool isCursorOff
    {
        get { return !isCursorOver; }
    }

    public List<zzInterfaceGUI> getSubsByDepth()
    {
        var lGUIlist = new List<zzInterfaceGUI>();
        foreach (Transform lTransform in transform)
        {
            zzInterfaceGUI impGUI = lTransform.GetComponent<zzInterfaceGUI>();
            if (impGUI)
            {
                //按深度排序,小的排在前面,先被渲染,会被深度大的遮住
                for (int i = 0; i < lGUIlist.Count; ++i)
                {
                    zzInterfaceGUI lGUIlistTemp = (zzInterfaceGUI)lGUIlist[i];
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
        return lGUIlist;
    }


    public virtual void impSubs()
    {

        //print("********************");
        foreach (zzInterfaceGUI imp in getSubsByDepth())
        {
            //print(imp.gameObject.name+" "+imp.getDepth());
            imp.renderGUI();
        }
    }
}