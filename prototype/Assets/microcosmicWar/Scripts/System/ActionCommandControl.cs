//OK
using UnityEngine;
using System.Collections;

[System.Serializable]
public class zzCharacter
{
    public CharacterController characterController;

    public float runSpeed = 5.0f;
    public float gravity = 15.0f;
    public float jumpSpeed = 10.0f;

    protected Vector3 moveV;

    public void update(UnitActionCommand pUnitActionCommand, int pFaceValue, bool isAlive)
    {
        moveV.x = 0;
        moveV.z = 0;

        if (isAlive && pUnitActionCommand.GoForward)
            moveV.x = pFaceValue;

        if (isAlive && characterController.isGrounded)
        {
            if (!pUnitActionCommand.FaceDown)
            {
                if (pUnitActionCommand.Jump)
                    moveV.y = jumpSpeed;
                else
                    moveV.y = -0.01f;	//以免飞起来
            }
        }
        else
            moveV.y -= gravity * Time.deltaTime;
        if (moveV.y > 0
            && (characterController.collisionFlags & CollisionFlags.Above)>0)
            moveV.y = 0;

        // Move the controller
        Vector3 lVelocity = new Vector3(moveV.x * runSpeed, moveV.y, 0);

        characterController.Move(lVelocity * Time.deltaTime);
    }

    public bool isGrounded()
    {
        return characterController.isGrounded;
    }

    public void OnSerializeNetworkView2D(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        Transform lTransform = characterController.transform;
        //---------------------------------------------------
        if (stream.isWriting)
        {
            pos = lTransform.position;
        }
        //---------------------------------------------------
        stream.Serialize(ref pos);
        stream.Serialize(ref moveV);
        //---------------------------------------------------
        if (stream.isReading)
        {
            lTransform.position = pos;
        }
    }

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        Transform lTransform = characterController.transform;
        //---------------------------------------------------
        if (stream.isWriting)
        {
            pos = lTransform.position;
            rot = lTransform.rotation;
        }
        //---------------------------------------------------
        stream.Serialize(ref pos);
        stream.Serialize(ref rot);
        stream.Serialize(ref moveV);
        //---------------------------------------------------
        if (stream.isReading)
        {
            lTransform.position = pos;
            lTransform.rotation = rot;
        }
    }

};

[System.Serializable]
public enum UnitFaceDirection
{
    left,
    right,
}

class UnitFace
{
    public static int leftFaceValue = -1;
    public static int rightFaceValue = 1;

    public static int getValue(UnitFaceDirection type)
    {
        if (type == UnitFaceDirection.left)
            return -1;
        return 1;
    }

    public static UnitFaceDirection getFace(int pValue)
    {
        if (pValue > 0)
            return UnitFaceDirection.right;
        return UnitFaceDirection.left;
    }

    public static UnitFaceDirection getFace(float pValue)
    {
        if (pValue > 0f)
            return UnitFaceDirection.right;
        return UnitFaceDirection.left;
    }
}

public class UnitActionCommand
{
    //None,

    public UnitActionCommand(){}

    public UnitActionCommand(UnitActionCommand pOther)
    {
        //FaceLeft = pOther.FaceLeft;//朝向左
        //FaceRight = pOther.FaceRight;//朝向右
        //FaceUp = pOther.FaceUp;
        //FaceDown = pOther.FaceDown;
        //GoForward = pOther.GoForward;//前进
        //Fire = pOther.Fire;//开火
        //Jump = pOther.Jump;//跳跃

        command = pOther.command;

    }

    //只网络同步后8位
    public int command;

    const int faceLeftCommand = 1 << 0;
    const int negFaceLeftCommand = ~faceLeftCommand;

    const int faceRightCommand      = 1 << 1;
    const int negFaceRightCommand = ~faceRightCommand;

    const int faceUpCommand         = 1 << 2;
    const int negFaceUpCommand = ~faceUpCommand;

    const int faceDownCommand       = 1 << 3;
    const int negFaceDownCommand = ~faceDownCommand;

    const int goForwardCommand      = 1 << 4;
    const int negGoForwardCommand = ~goForwardCommand;

    const int fireCommand           = 1 << 5;
    const int negFireCommand = ~fireCommand;

    const int jumpCommand           = 1 << 6;
    const int negJumpCommand = ~jumpCommand;


    //朝向左
    public bool FaceLeft
    {
        get { return (command & faceLeftCommand)!=0; }
        set 
        {
            if (value)
                command |= faceLeftCommand;
            else
                command &= negFaceLeftCommand;
        }
    }

    //朝向右
    public bool FaceRight
    {
        get { return (command & faceRightCommand) != 0; }
        set
        {
            if (value)
                command |= faceRightCommand;
            else
                command &= negFaceRightCommand;
        }
    }

    public bool FaceUp
    {
        get { return (command & faceUpCommand) != 0; }
        set
        {
            if (value)
                command |= faceUpCommand;
            else
                command &= negFaceUpCommand;
        }
    }

    public bool FaceDown
    {
        get { return (command & faceDownCommand) != 0; }
        set
        {
            if (value)
                command |= faceDownCommand;
            else
                command &= negFaceDownCommand;
        }
    }

    //前进
    public bool GoForward
    {
        get { return (command & goForwardCommand) != 0; }
        set
        {
            if (value)
                command |= goForwardCommand;
            else
                command &= negGoForwardCommand;
        }
    }

    //开火
    public bool Fire
    {
        get { return (command & fireCommand) != 0; }
        set
        {
            if (value)
                command |= fireCommand;
            else
                command &= negFireCommand;
        }
    }

    //跳跃
    public bool Jump
    {
        get { return (command & jumpCommand) != 0; }
        set
        {
            if (value)
                command |= jumpCommand;
            else
                command &= negJumpCommand;
        }
    }

    public void clear()
    {
        command = 0;
    }

    public override string ToString()
    {
        return "FaceLeft:" + FaceLeft + " FaceRight:" + FaceRight + " FaceUp:" + FaceUp
            + " FaceDown:" + FaceDown + " GoForward:" + GoForward + " Fire:" + Fire + " Jump:" + Jump;
    }
}
public class ActionCommandControl : MonoBehaviour
{

    [SerializeField]
    protected UnitFaceDirection face = UnitFaceDirection.left;

    //返回在x上的值
    public int getFaceValue()
    {
        //Debug.Log("face:"+face+" "+UnitFace.getValue(face));
        return UnitFace.getValue(face);
    }

    public UnitFaceDirection getFace()
    {
        return face;
    }



    public UnitActionCommand unitActionCommand = new UnitActionCommand();

    public void setCommand(UnitActionCommand pUnitActionCommand)
    {
        /*
            if(gameObject.name!="pismireHero1(Clone)")
            {
                print(gameObject.name);
                print("setCommand");
                print(pUnitActionCommand);
            }
        */
        unitActionCommand = pUnitActionCommand;
    }

    public UnitActionCommand getCommand()
    {
        return new UnitActionCommand(unitActionCommand);
    }

    //根据现在命令的方向更新朝向,若朝向有变则返回true
    public bool updateFace()
    {
        if (unitActionCommand.FaceLeft != unitActionCommand.FaceRight)
        {
            if (unitActionCommand.FaceLeft && face == UnitFaceDirection.right)
            {
                face = UnitFaceDirection.left;
                //Debug.Log("Change to left");
                return true;
            }
            if (unitActionCommand.FaceRight && face == UnitFaceDirection.left)
            {
                face = UnitFaceDirection.right;
                //Debug.Log("Change to right");
                return true;
            }
        }
        return false;
    }


    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        char lCommand = (char)System.Convert.ToByte(unitActionCommand.command & 0xff);

        //---------------------------------------------------
        stream.Serialize(ref lCommand);
        //---------------------------------------------------
        if (stream.isReading)
        {
            unitActionCommand.command = ((byte)lCommand)&0xff;
        }
        //int lFace = face;
        //stream.Serialize(lFace);
        //face =  lFace;

        //stream.Serialize(ref unitActionCommand.command);
        
        /*
        if(info.networkView.name!="NS")
        {
            print(info.networkView.viewID );
            print(info.networkView.name );
		
            print("isWriting"+stream.isWriting);
            print("GoForward"+unitActionCommand.GoForward);
            print("FaceRight"+unitActionCommand.FaceRight);
            print("----------------------------------------------");
        }
        */

    }

}
