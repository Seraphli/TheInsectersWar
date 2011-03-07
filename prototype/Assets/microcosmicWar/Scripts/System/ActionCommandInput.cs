
using UnityEngine;
using System.Collections;

public class ActionCommandInput : MonoBehaviour
{

    public ActionCommandControl actionCommandControl;

    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeyCode fire;
    public KeyCode jump;

    public void setToControl(ActionCommandControl pActionCommandControl)
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
        if (Input.GetKey(down))
            lActionCommand.FaceDown = true;
        if (Input.GetKey(up))
            lActionCommand.FaceUp = true;

        lActionCommand.Fire = Input.GetKeyDown(fire);
        lActionCommand.Jump = Input.GetKeyDown(jump);
        return lActionCommand;
    }

    void Update()
    {
        if (actionCommandControl)
            actionCommandControl.setCommand(GetActionCommandFromInput());
    }
}