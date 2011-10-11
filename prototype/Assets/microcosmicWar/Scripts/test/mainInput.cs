
using UnityEngine;
using System.Collections;

public class mainInput : MonoBehaviour
{

    public CommandControlBase[] actionCommandControl;
    public ObjectOperatives objectOperatives;

    //public void setToControl(CommandControlBase pActionCommandControl)
    //{
    //    actionCommandControl = pActionCommandControl;
    //}

    public UnitActionCommand GetActionCommandFromInput()
    {
        UnitActionCommand lActionCommand = new UnitActionCommand();
        //lActionCommand.MoveLeft=Input.GetButton ("left");
        if (Input.GetButton("left"))
        {
            lActionCommand.FaceLeft = true;
            lActionCommand.GoForward = true;
        }
        //lActionCommand.MoveRight=Input.GetButton ("right");
        if (Input.GetButton("right"))
        {
            lActionCommand.FaceRight = true;
            lActionCommand.GoForward = true;
        }
        lActionCommand.FaceDown = Input.GetButton("down");
        lActionCommand.FaceUp = Input.GetButton("up");

        lActionCommand.Fire = Input.GetButton("fire");
        //lActionCommand.Jump=Input.GetButtonDown("jump");
        lActionCommand.Jump = Input.GetButton("jump");
        lActionCommand.Action1 = Input.GetButton("action1");
        lActionCommand.Action2 = Input.GetButton("action2");
        return lActionCommand;
    }

    void Update()
    {
        //if( zzCreatorUtility.isHost())
        //if (actionCommandControl)
        //    actionCommandControl.setCommand(GetActionCommandFromInput());
        if (actionCommandControl.Length > 0)
        {
            var lCommand = GetActionCommandFromInput();
            foreach (var lControl in actionCommandControl)
            {
                lControl.setCommand(lCommand);
            }
        }

        if (objectOperatives && Input.GetButtonDown("function"))
            objectOperatives.doOperate();
    }
}