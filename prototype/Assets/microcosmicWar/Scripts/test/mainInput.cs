﻿
using UnityEngine;
using System.Collections;

public class mainInput : MonoBehaviour
{

    public ActionCommandControl actionCommandControl;

    public void setToControl(ActionCommandControl pActionCommandControl)
    {
        actionCommandControl = pActionCommandControl;
    }

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
        if (Input.GetButton("down"))
            lActionCommand.FaceDown = true;
        if (Input.GetButton("up"))
            lActionCommand.FaceUp = true;

        lActionCommand.Fire = Input.GetButton("fire");
        //lActionCommand.Jump=Input.GetButtonDown("jump");
        lActionCommand.Jump = Input.GetButton("jump");
        return lActionCommand;
    }

    void Update()
    {
        //if( zzCreatorUtility.isHost())
        if (actionCommandControl)
            actionCommandControl.setCommand(GetActionCommandFromInput());
    }
}