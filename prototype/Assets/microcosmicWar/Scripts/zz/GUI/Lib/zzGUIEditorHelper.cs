using UnityEngine;

[ExecuteInEditMode]
public class zzGUIEditorHelper:MonoBehaviour
{
    public zzInterfaceGUI selection;

    //public class Palette
    //{
    //}
    public Color normalColor = Color.white;
    public Color hoverColor = Color.blue;
    public Color activeColor = Color.yellow;

    public float lineWidth = 1f;

    public float boxExtent = 5f;

    //void Update()
    //{
    //    print(Input.GetMouseButton(0));
    //}
    //void OnDrawGizmos()
    //{
    //    //Gizmos.color = Color.blue;
    //    print(Input.mousePosition);
    //    print(Input.GetMouseButton(0));
    //}
    bool mouseDown = false;

    bool draging = false;

    public Vector2 dragBeginMousePos;

    public Rect dragBeginUiPosition;

    public Rect dragBeginUiScreenPosition;

    public bool showCameraRender = false;

    public Rect windowPosition;

    public Color backgroudColor = Color.clear;

    public void useMouseEvent(Event lEvent)
    {
        if (lEvent.type == EventType.MouseDown && lEvent.button == 0)
        {
            mouseDown = true;
            lEvent.Use();
        }
        else if (lEvent.type == EventType.MouseUp && lEvent.button == 0)
        {
            mouseDown = false;
            lEvent.Use();
        }
    }

    bool needScaleWidth = false;
    bool needScaleHeight = false;

    void doScale()
    {
        var lUIScreenPosition = selection.screenPosition;
        var lMousePosition = GUIEvent.mousePosition;

        var lUiPosition = selection.getPosition();
        var lRelativePosition = selection.relativePosition;
        var lUseRelativePosition = selection.useRelativePosition;
        var lParentPositon = selection.getParent().getPosition();

        if (needScaleWidth)
        {
            var lTranslationX = lMousePosition.x - dragBeginMousePos.x;
            var lWantWidth = dragBeginUiPosition.width + lTranslationX;
            if (lUseRelativePosition.width)
            {
                lRelativePosition.width = lWantWidth / lParentPositon.width;
            }
            else
            {
                lUiPosition.width = lWantWidth;
            }

        }

        if (needScaleHeight)
        {
            var lTranslationY = lMousePosition.y - dragBeginMousePos.y;
            var lWantHeight = dragBeginUiPosition.height + lTranslationY;
            if (lUseRelativePosition.height)
            {
                lRelativePosition.height = lWantHeight / lParentPositon.height;
            }
            else
            {
                lUiPosition.height = lWantHeight;
            }

        }

        selection.position = lUiPosition;
        selection.relativePosition = lRelativePosition;
    }

    enum DockPosition
    {
        min,
        center,
        max,
    }

    [SerializeField]
    class PointDock
    {
        public bool docked = false;
        public DockPosition dockX;
        public DockPosition dockY;
    }

    PointDock dragedPoint = new PointDock();

    class ControlPoint
    {
        //public ControlPoint(Vector2 pPosition,float boxExtent)
        //{
        //    position = pPosition;
        //    boxExtent = boxExtent;
        //}

        public Vector2 position;
        public float boxExtent;
        public Color color;
        public DockPosition dockX;
        public DockPosition dockY;

        public Color normalColor;
        public Color hoverColor;
        public Color activeColor;
        public PointDock dragedPoint;
        //public Color activeColor = Color.yellow;

        public bool isFocuseSelf
        {
            get
            {
                return dragedPoint.docked
                    && dragedPoint.dockX == dockX
                    && dragedPoint.dockY == dockY;
            }
        }

        public void drawGUI()
        {
            zzGUILibDrawLine.DrawBox(position, new Vector2(boxExtent, boxExtent), color, 0f);
        }

        public void checkDraged(Vector2 pMousePos,bool pIsDown)
        {
            if (isFocuseSelf)
            {
                color = activeColor;
                if (!pIsDown)
                    dragedPoint.docked = false;
            }
            else if ((position - pMousePos).sqrMagnitude < boxExtent * boxExtent)
            {
                if (pIsDown)
                {
                    dragedPoint.docked = true;
                    dragedPoint.dockX = dockX;
                    dragedPoint.dockY = dockY;
                }
                color = hoverColor;
            }
            else
                color = normalColor;
        }
    }

    void OnDrag()
    {
        dragBeginMousePos = GUIEvent.mousePosition;
        dragBeginUiPosition = selection.getPosition();
        dragBeginUiScreenPosition = selection.screenPosition;
    }

    ControlPoint createControlPoint(Vector2 pPosition,
        DockPosition pDockPositionX, DockPosition pDockPositionY)
    {
        return new ControlPoint() { 
            position = pPosition,
            boxExtent = boxExtent,
            dockX = pDockPositionX,
            dockY = pDockPositionY,
            normalColor = normalColor,
            hoverColor = hoverColor,
            activeColor = activeColor,
            dragedPoint = dragedPoint,
            //activeColor = activeColor,
        };
    }

    Event GUIEvent;

    ControlPoint[] controlPoints;

    void checkAndDrawPoint()
    {
        var lPreDocked = dragedPoint.docked;
        foreach (var lControlPoint in controlPoints)
        {
            lControlPoint.checkDraged(GUIEvent.mousePosition, mouseDown);

            lControlPoint.drawGUI();
        }

        if (dragedPoint.docked)
        {
            if (!lPreDocked)
            {
                OnDrag();
            }
            else
            {
                if (dragedPoint.dockX == DockPosition.max)
                    needScaleWidth = true;
                if (dragedPoint.dockY == DockPosition.max)
                    needScaleHeight = true;
                doScale();
                needScaleWidth = false;
                needScaleHeight = false;

            }
        }
    }

    bool needXTranslate = false;
    bool needYTranslate = false;

    void doTranslate()
    {
        var lUIScreenPosition = selection.screenPosition;
        var lMousePosition = GUIEvent.mousePosition;

        var lUiPosition = selection.getPosition();
        var lRelativePosition = selection.relativePosition;
        var lUseRelativePosition = selection.useRelativePosition;
        var lParentPositon = selection.getParent().getPosition();

        if (needXTranslate)
        {
            var lTranslationX = lMousePosition.x - dragBeginMousePos.x;
            var lWantPosX = dragBeginUiPosition.x + lTranslationX;
            if (lUseRelativePosition.x)
            {
                lRelativePosition.x = lWantPosX / lParentPositon.width;
            }
            else
            {
                lUiPosition.x = lWantPosX;
            }

        }

        if (needYTranslate)
        {
            var lTranslationY = lMousePosition.y - dragBeginMousePos.y;
            var lWantPosY = dragBeginUiPosition.y + lTranslationY;
            if (lUseRelativePosition.y)
            {
                lRelativePosition.y = lWantPosY / lParentPositon.height;
            }
            else
            {
                lUiPosition.y = lWantPosY;
            }

        }

        selection.position = lUiPosition;
        selection.relativePosition = lRelativePosition;
    }

    public void OnGUI()
    {
        GUIEvent = Event.current;

        if (selection && !(selection is zzGUIRender))
        {
            if (dragedPoint == null)
                dragedPoint = new PointDock();

            var lUIScreenPosition = selection.screenPosition;
            var lMousePosition = GUIEvent.mousePosition;
            controlPoints = new ControlPoint[]{
                createControlPoint(new Vector2(lUIScreenPosition.xMax,lUIScreenPosition.y+lUIScreenPosition.height/2f),
                DockPosition.max, DockPosition.center),
                
                createControlPoint(new Vector2(lUIScreenPosition.x+lUIScreenPosition.width/2f, lUIScreenPosition.yMax),
                DockPosition.center, DockPosition.max),
                
                createControlPoint(new Vector2(lUIScreenPosition.xMax, lUIScreenPosition.yMax),
                DockPosition.max, DockPosition.max),
            };

            Color lColorToAll = normalColor;
            bool lFocuseOnCenter = false;

            {
                if( dragedPoint.docked
                    &&dragedPoint.dockX== DockPosition.center
                    &&dragedPoint.dockY== DockPosition.center )
                {
                    lFocuseOnCenter = true;
                    lColorToAll = activeColor;
                    if (!mouseDown)
                        dragedPoint.docked = false;

                    needXTranslate = true;
                    needYTranslate = true;
                    doTranslate();
                }
                else if (!dragedPoint.docked)
                {
                    var lCenterRect = new Rect(
                        lUIScreenPosition.x + boxExtent,
                        lUIScreenPosition.y + boxExtent,
                        lUIScreenPosition.width - boxExtent * 2f,
                        lUIScreenPosition.height - boxExtent * 2f);

                    if (lCenterRect.Contains(lMousePosition))
                    {
                        lFocuseOnCenter = true;
                        lColorToAll = hoverColor;
                        if (mouseDown)
                        {
                            dragedPoint.docked = true;
                            dragedPoint.dockX = DockPosition.center;
                            dragedPoint.dockY = DockPosition.center;
                            OnDrag();
                        }
                    }

                }
            }

            zzGUILibDrawLine.DrawLine(selection.screenPosition, lColorToAll, lineWidth);

            if (lFocuseOnCenter)
            {
                foreach (var lControlPoint in controlPoints)
                {
                    lControlPoint.color = lColorToAll;

                    lControlPoint.drawGUI();
                }
            }
            else
            {
                checkAndDrawPoint();
            }


            //var lRightPoint = new Vector2(lUIScreenPosition.xMax, lUIScreenPosition.yMax/2f);
            //var lButtomPoint = new Vector2(lUIScreenPosition.xMax/2f, lUIScreenPosition.yMax);
            //var lRightButtomPoint = new Vector2(lUIScreenPosition.xMax, lUIScreenPosition.yMax);
            //Color lColor;

            //if (draging)
            //{
            //    lColor = activeColor;
            //    if (!mouseDown)
            //        draging = false;
            //}
            //else if ((lRightButtomPoint - lMousePosition).sqrMagnitude < boxExtent * boxExtent)
            //{
            //    if (mouseDown)
            //    {
            //        draging = true;
            //        dragBeginMousePos = lMousePosition;
            //        dragBeginUiPosition = selection.getPosition();
            //        dragBeginUiScreenPosition = lUIScreenPosition;
            //    }
            //    lColor = hoverColor;
            //}
            //else
            //    lColor = normalColor;


            //if (draging)
            //{
            //    doDrag();
            //}

            //zzGUILibDrawLine.DrawBox(lRightButtomPoint, new Vector2(boxExtent, boxExtent), lColor, 0f);

        }
    }
}