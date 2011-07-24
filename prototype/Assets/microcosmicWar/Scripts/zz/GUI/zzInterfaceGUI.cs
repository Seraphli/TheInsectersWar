
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

public enum zzGUIDirection
{
    horizontal,
    vertical,
}

//class zzGUIRelativeLength
//{
//	FIXME_VAR_TYPE useRelative=false;
//	float relativeLength;//[0,1]
//}

[System.Serializable]
public class zzGUIRelativeUsedInfo
{
    public zzGUIRelativeUsedInfo()
    {
        x = false;
        y = false;
        width = false;
        height = false;

    }

    public zzGUIRelativeUsedInfo(bool pX, bool pY, bool pWidth, bool pHeight)
    {
        x = pX;
        y = pY;
        width = pWidth;
        height = pHeight;

    }

    public bool x = false;
    public bool y = false;
    public bool width = false;
    public bool height = false;
}

public abstract class zzInterfaceGUI : MonoBehaviour
{
    static int sID = 0;
    protected   int newID()
    {
        return ++sID;
    }

    public static bool canChangeLayout
    {
        get
        {
            return Event.current.type != EventType.Layout
                        && Event.current.type != EventType.Repaint;

        }
    }
    //[System.Serializable]
    //public class UIVector2
    //{
    //    public int x;
    //    public int y;
    //}

    //int data;
    public Rect position;
    public Rect relativePosition;

    public zzGUIRelativeUsedInfo useRelativePosition = new zzGUIRelativeUsedInfo();


    //小的,先被渲染,会被深度大的遮住
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

    public delegate void VoidCallFunc();

    protected static void nullVoidCallFunc() { }

    public delegate void StringCallFunc(string text);

    protected void nullStringCallFunc(string text) { }

    public void renderGUI()
    {
        _beginRender();
        _renderGUI(getPosition());
        _endRender();
        //if (getVisible())
        //{
        //    calculatePosition();
        //    GUISkin lSkin = getSkin();
        //    if (lSkin)
        //    {
        //        //使用 Skin
        //        GUISkin lPreSkin = GUI.skin;
        //        GUI.skin = lSkin;
        //        impGUI();
        //        GUI.skin = lPreSkin;
        //    }
        //    else
        //        impGUI();
        //}

    }

    //public virtual bool isCoordinateReseted
    //{
    //    get
    //    {
    //        return false;
    //    }
    //}

    [SerializeField]
    private Rect _screenPosition;

    /// <summary>
    /// Todo: 改用originOfCoordinates和position获取
    /// </summary>
    public Rect screenPosition
    {
        get { return _screenPosition; }
    }

    //bool _isCursorOver;

    //public zzInterfaceGUI root;

    /// <summary>
    /// only can be call by sub widget
    /// </summary>
    public virtual Vector2 originOfCoordForSub
    {
        get
        {
            return originOfCoordinates;
        }
    }

    void _beginRender()
    {
        originOfCoordinates = getParent().originOfCoordForSub;
        calculateAndSetPosition();
        //if (isCoordinateReseted)
        //{
        //    zzGUI.originOfCoordinates = new Vector2(screenPosition.x, screenPosition.y);
        //}

    }

    [SerializeField]
    protected Vector2 originOfCoordinates;

    void _endRender()
    {
        zzGUI.originOfCoordinates = originOfCoordinates;
    }

    protected void _renderGUI(Rect rect)
    {
        if (getVisible())
        {
            //root = zzGUI.root;
            GUISkin lSkin = getSkin();
            if (lSkin)
            {
                //使用 Skin
                GUISkin lPreSkin = GUI.skin;
                GUI.skin = lSkin;
                impGUI(rect);
                GUI.skin = lPreSkin;
            }
            else
                impGUI(rect);
        }
    }

    public void renderGUI(Rect rect)
    {
        _beginRender();
        _renderGUI(rect);
        _endRender();
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

    //public virtual void impGUI()
    //{
    //    impGUI(getPosition());
    //}

    public abstract void impGUI(Rect rect);

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

    public virtual float calculateWidth()
    {
        if (useRelativePosition.width)
            return relativePosition.width * getParent().getWidth();
        return position.width;
    }

    public float getWidth()
    {
        return position.width;
    }

    public virtual float calculateHeight()
    {
        if (useRelativePosition.height)
            return relativePosition.height * getParent().getHeight();
        return position.height;
    }

    public float getHeight()
    {
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

    protected void calculateAndSetPosition()
    {
        position = calculatePosition();
        _screenPosition = new Rect(
                position.x + originOfCoordinates.x,
                position.y + originOfCoordinates.y,
                position.width,
                position.height
            );
    }

    protected void calculatePosition(ref Rect lOut)
    {
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
    }

    public virtual Rect calculatePosition()
    {
        Rect lOut = new Rect();

        lOut.width = calculateWidth();
        //print(gameObject.name);
        //print(relativeWidth.useRelative);
        //print(lOut.width);
        lOut.height = calculateHeight();
        calculatePosition(ref lOut);
        return lOut;
    }

    public virtual Rect getPosition()
    {
        return position;
    }

    public virtual int getDepth()
    {
        return depth;
    }

    //void OnDrawGizmosSelected()
    //{
    //    DrawGizmos(Color.white);
    //}

    //void OnDrawGizmos()
    //{
    //    //Gizmos.color = Color.blue;
    //    DrawGizmos(Color.blue);
    //}

    //void DrawGizmos(Color pColor)
    //{
    //    Gizmos.color = pColor;

    //    Gizmos.matrix = transform.localToWorldMatrix;

    //    //OnDrawGizmos 使用不同的摄像机,所以避免改变UI位置,直接获取值
    //    //Rect lPosition = getPosition();
    //    Rect lPosition = position;
    //    Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.y, 0), new Vector3(lPosition.xMax, -lPosition.y, 0));
    //    Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.y, 0), new Vector3(lPosition.x, -lPosition.yMax, 0));


    //    Gizmos.DrawLine(new Vector3(lPosition.xMax, -lPosition.y, 0), new Vector3(lPosition.xMax, -lPosition.yMax, 0));
    //    Gizmos.DrawLine(new Vector3(lPosition.x, -lPosition.yMax, 0), new Vector3(lPosition.xMax, -lPosition.yMax, 0));

    //}
}
