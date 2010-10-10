
using UnityEngine;
using System.Collections;

public class zzGUI : MonoBehaviour
{


    public zzInterfaceGUI myGUI;

    public static void impGUI(zzInterfaceGUI pMyGUI)
    {
        //GUI.depth = myGUI.depth;

        if (pMyGUI.getVisible())
        {
            GUISkin lSkin = pMyGUI.getSkin();
            if (lSkin)
            {
                //使用 Skin
                GUISkin lPreSkin = GUI.skin;
                GUI.skin = lSkin;
                pMyGUI.impGUI();
                GUI.skin = lPreSkin;
            }
            else
                pMyGUI.impGUI();
        }
    }

    public void impGUI()
    {
        impGUI(myGUI);
    }

    public int getDepth()
    {
        return myGUI.depth;
    }

    public void setGUI(zzInterfaceGUI pGUI)
    {
        myGUI = pGUI;
    }

    public zzInterfaceGUI getGUI()
    {
        return myGUI;
    }
}