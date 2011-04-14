using UnityEngine;

public class zzGUILink:zzInterfaceGUI
{
    public zzInterfaceGUI link;

    public override void impGUI(Rect rect)
    {
        if (link)
        {
            link.position = rect;
            link.impGUI(rect);
        }
    }
}