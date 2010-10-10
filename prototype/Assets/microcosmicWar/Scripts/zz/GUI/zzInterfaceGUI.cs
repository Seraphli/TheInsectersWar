
using UnityEngine;
using System.Collections;


//@RequireComponent(zzGUI)

public enum zzGUIDockPos
{
    min,
    center,
    max,
    custom
};

//class zzGUIRelativeLength
//{
//	FIXME_VAR_TYPE useRelative=false;
//	float relativeLength;//[0,1]
//}

[System.Serializable]
public class zzGUIRelativeUsedInfo
{
    public bool x = false;
    public bool y = false;
    public bool width = false;
    public bool height = false;
}

public abstract class zzInterfaceGUI : MonoBehaviour
{
    //int data;
    public Rect position;
    public Rect relativePosition;

    public zzGUIRelativeUsedInfo useRelativePosition = new zzGUIRelativeUsedInfo();

    public int depth;
    public GUISkin skin;

    //位置信息;custom 则使用position的
    public zzGUIDockPos horizontalDockPosition = zzGUIDockPos.custom;
    public zzGUIDockPos verticalDockPosition = zzGUIDockPos.custom;

    //相对尺寸信息,useRelative=false,则使用position的
    //FIXME_VAR_TYPE relativeWidth=zzGUIRelativeLength();
    //FIXME_VAR_TYPE relativeHeight=zzGUIRelativeLength();


    //FIXME_VAR_TYPE relativePosX=zzGUIRelativeLength();
    //FIXME_VAR_TYPE relativePosY=zzGUIRelativeLength();

    public bool visible = true;

    //只是为了付类型
    //protected void nullGUICallback ( zzInterfaceGUI pGUI  ){
    //}
    protected static void nullGUICallback ( zzInterfaceGUI pGUI  ){}

    public delegate void GUICallFunc(zzInterfaceGUI pGUI);

    public virtual GUISkin getSkin()
    {
        return skin;
    }

    public virtual void setVisible(bool pVisible)
    {
        visible = pVisible;
    }

    public virtual bool getVisible()
    {
        return visible;
    }

    public abstract void impGUI();

    public virtual void setImage(Texture pImage)
    {
    }

    public virtual void setText(string pText)
    {
    }

    public zzInterfaceGUI getSubElement(string pName)
    {
        Transform lTransform = transform.Find(pName);
        if (lTransform)
        {
            zzGUI impGUI = lTransform.GetComponent<zzGUI>();
            if (impGUI)
                return impGUI.getGUI();
        }
        return null;
    }

    public zzInterfaceGUI getParent()
    {
        zzGUI impGUI = transform.parent.GetComponent<zzGUI>();
        if (impGUI)
            return impGUI.getGUI();
        //print('"null"');
        return null;
    }

    public virtual float getWidth()
    {
        if (useRelativePosition.width)
            return relativePosition.width * getParent().getWidth();
        return position.width;
    }

    public virtual float getHeight()
    {
        if (useRelativePosition.height)
            return relativePosition.height * getParent().getHeight();
        return position.height;
    }

    public virtual float getPosX()
    {
        if (useRelativePosition.x)
            return relativePosition.x * getParent().getWidth();
        return position.x;
    }

    public virtual float getPosY()
    {
        if (useRelativePosition.y)
            return relativePosition.y * getParent().getHeight();
        return position.y;
    }

    public virtual Rect getPosition()
    {
        //FIXME_VAR_TYPE lOut= position;
        Rect lOut = new Rect();

        //horizontal
        switch (horizontalDockPosition)
        {
            case zzGUIDockPos.min:
                lOut.x = 0;
                break;
            case zzGUIDockPos.center:
                lOut.x = Screen.width / 2 - lOut.width / 2;
                break;
            case zzGUIDockPos.max:
                lOut.x = Screen.width - lOut.width;
                break;
            case zzGUIDockPos.custom:
                lOut.x = getPosX();
                break;
        }

        //vertical
        switch (verticalDockPosition)
        {
            case zzGUIDockPos.min:
                lOut.y = 0;
                break;
            case zzGUIDockPos.center:
                lOut.y = Screen.height / 2 - lOut.height / 2;
                break;
            case zzGUIDockPos.max:
                lOut.y = Screen.height - lOut.height;
                break;
            case zzGUIDockPos.custom:
                lOut.y = getPosY();
                break;
        }

        lOut.width = getWidth();
        //print(gameObject.name);
        //print(relativeWidth.useRelative);
        //print(lOut.width);
        lOut.height = getHeight();

        position = lOut;
        return lOut;
    }

    void Reset()
    {
        //添加
        /*
        if(! gameObject.GetComponent<zzGUI>() )
        {
            zzGUI lzzGUI = gameObject.AddComponent<zzGUI>();
            lzzGUI.setGUI(this);
        }
        */
        zzGUI lzzGUI = (zzGUI)zzUtilities.needComponent(gameObject, typeof(zzGUI) );
        lzzGUI.setGUI(this);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(Color.white);
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        DrawGizmos(Color.blue);
    }

    void DrawGizmos(Color pColor)
    {
        Gizmos.color = pColor;

        Gizmos.matrix = transform.localToWorldMatrix;

        Rect lPosition = getPosition();
        Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.y, 0), new Vector3(lPosition.xMax, -lPosition.y, 0));
        Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.y, 0), new Vector3(lPosition.x, -lPosition.yMax, 0));


        Gizmos.DrawLine(new Vector3(lPosition.xMax, -lPosition.y, 0), new Vector3(lPosition.xMax, -lPosition.yMax, 0));
        Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.yMax, 0), new Vector3(lPosition.xMax, -lPosition.yMax, 0));
        /*
			
        Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax-position.width/2,-position.y,0));
        Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax+position.height/2,0));
		
		
        Gizmos.DrawLine (Vector3(position.xMax-position.width/2,-position.y,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
        Gizmos.DrawLine (Vector3(position.x,-position.yMax+position.height/2,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
        */
    }
}
