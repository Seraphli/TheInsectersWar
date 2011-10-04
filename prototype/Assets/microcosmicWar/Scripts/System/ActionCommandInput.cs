
using UnityEngine;
using System.Collections;

public class ActionCommandInput : MonoBehaviour
{

    public CommandControlBase actionCommandControl;

    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeyCode fire;
    public KeyCode jump;
    public KeyCode action1;
    public KeyCode action2;

    public void setToControl(CommandControlBase pActionCommandControl)
    {
        actionCommandControl = pActionCommandControl;
    }

    public UnitActionCommand GetActionCommandFromInput()
    {
        UnitActionCommand lActionCommand = new UnitActionCommand();
        if (Input.GetKey(left))
        {
            lActionCommand.FaceLeft = true;
            lActionCommand.GoForward = true;
        }
        if (Input.GetKey(right))
        {
            lActionCommand.FaceRight = true;
            lActionCommand.GoForward = true;
        }
        lActionCommand.FaceDown = Input.GetKey(down);
        lActionCommand.FaceUp = Input.GetKey(up);

        lActionCommand.Fire = Input.GetKey(fire);
        lActionCommand.Jump = Input.GetKey(jump);
        lActionCommand.Action1 = Input.GetKey(action1);
        lActionCommand.Action2 = Input.GetKey(action2);
        return lActionCommand;
    }

    void Update()
    {
        if (actionCommandControl)
        {
            actionCommandControl.setCommand(GetActionCommandFromInput());
        }
    }
}