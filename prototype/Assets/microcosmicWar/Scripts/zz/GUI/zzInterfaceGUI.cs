
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

    //public GUIContent getContent()
    //{
    //    return _content;
    //}

    //public _style getStyle()
    //{
    //    return _style;
    //}


    //只是为了付类型
    //protected void nullGUICallback ( zzInterfaceGUI pGUI  ){
    //}
    protected static void nullGUICallback ( zzInterfaceGUI pGUI  ){}

    public delegate void GUICallFunc(zzInterfaceGUI pGUI);

    public void renderGUI()
    {

        if (getVisible())
        {
            GUISkin lSkin = getSkin();
            if (lSkin)
            {
                //使用 Skin
                GUISkin lPreSkin = GUI.skin;
                GUI.skin = lSkin;
                impGUI();
                GUI.skin = lPreSkin;
            }
            else
                impGUI();
        }

    }

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
            zzInterfaceGUI impGUI = lTransform.GetComponent<zzInterfaceGUI>();
            //if (impGUI)
                return impGUI;
        }
        return null;
    }

    public zzInterfaceGUI getParent()
    {
        zzInterfaceGUI impGUI = transform.parent.GetComponent<zzInterfaceGUI>();
        //if (impGUI)
        //    return impGUI.getGUI();
        //print('"null"');
        //return null;
        return impGUI;
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


        lOut.width = getWidth();
        //print(gameObject.name);
        //print(relativeWidth.useRelative);
        //print(lOut.width);
        lOut.height = getHeight();
        //horizontal
        switch (horizontalDockPosition)
        {
            case zzGUIDockPos.min:
                lOut.x = 0;
                break;
            case zzGUIDockPos.center:
                lOut.x = getParent().getWidth() / 2 - lOut.width / 2;
                break;
            case zzGUIDockPos.max:
                lOut.x = getParent().getWidth() - lOut.width;
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
                lOut.y = getParent().getHeight() / 2 - lOut.height / 2;
                break;
            case zzGUIDockPos.max:
                lOut.y = getParent().getHeight() - lOut.height;
                break;
            case zzGUIDockPos.custom:
                lOut.y = getPosY();
                break;
        }

        position = lOut;
        return lOut;
    }

    public virtual int getDepth()
    {
        return depth;
    }

    //void Reset()
    //{
    //    //添加
    //    /*
    //    if(! gameObject.GetComponent<zzGUI>() )
    //    {
    //        zzGUI lzzGUI = gameObject.AddComponent<zzGUI>();
    //        lzzGUI.setGUI(this);
    //    }
    //    */
    //    zzGUI lzzGUI = (zzGUI)zzUtilities.needComponent(gameObject, typeof(zzGUI) );
    //    lzzGUI.setGUI(this);
    //}

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
