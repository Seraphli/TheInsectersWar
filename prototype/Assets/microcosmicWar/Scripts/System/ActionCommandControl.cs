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
    public float minYVelocity = -10.0f;

    //因为网络同步中也有update(推进了时间),所以不能用Time.DeltaTime
    public float lastUpdateTime = 0f;

    //protected Vector3 moveV;

    //public void update2D(float pDeltaTime)
    //{
    //    if (!characterController.isGrounded)
    //        moveV.y = Mathf.Max(moveV.y - gravity * pDeltaTime, minYVelocity);
    //    Vector3 lVelocity = new Vector3(moveV.x * runSpeed, moveV.y, 0);
    //    characterController.Move(lVelocity * pDeltaTime);
    //}

    public float yVelocity;

    public void update2D(UnitActionCommand pUnitActionCommand, int pFaceValue, bool isAlive)
    {
        update2D(pUnitActionCommand, pFaceValue, isAlive, Time.time - lastUpdateTime);
        lastUpdateTime = Time.time;
    }

    public const float yNullVelocity = -0.01f;

    public void update2D(UnitActionCommand pUnitActionCommand, int pFaceValue, bool isAlive, float pDeltaTime)
    {

        if (!isAlive |!pUnitActionCommand.GoForward)
            pFaceValue = 0;

        if (isAlive && characterController.isGrounded)
        {
            if (!pUnitActionCommand.FaceDown)
            {
                if (pUnitActionCommand.Jump)
                    yVelocity = jumpSpeed;
                else
                    yVelocity = yNullVelocity;	//以免飞起来
            }
        }
        else
            yVelocity = Mathf.Max(yVelocity - gravity * pDeltaTime, minYVelocity);
        if (yVelocity > 0
            && (characterController.collisionFlags & CollisionFlags.Above)>0)
            yVelocity = 0;

        // Move the controller
        Vector3 lVelocity = new Vector3(pFaceValue * runSpeed, yVelocity, 0);

        characterController.Move(lVelocity * pDeltaTime);
    }

    public bool isGrounded()
    {
        return characterController.isGrounded;
    }

    public void OnSerializeNetworkView2D(BitStream stream, NetworkMessageInfo info,
        UnitActionCommand pUnitActionCommand, bool pIsAlive)
    {
        Transform lTransform = characterController.transform;
        Vector3 lData = Vector3.zero;
        //---------------------------------------------------
        if (stream.isWriting)
        {
            lData = lTransform.position;
            lData.z = yVelocity;
        }
        //---------------------------------------------------
        stream.Serialize(ref lData);
        //---------------------------------------------------
        if (stream.isReading)
        {
            yVelocity = lData.z;
            lData.z = 0f;
            lTransform.position = lData;
            
            var lDeltaTime = (float)(Network.time - info.timestamp);
            if (lDeltaTime > 0.02f)
                update2D(pUnitActionCommand, UnitFace.getValue(pUnitActionCommand.face),
                    pIsAlive, lDeltaTime * Time.timeScale);
            lastUpdateTime = Time.time;

        }
    }

    //public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{
    //    Vector3 pos = new Vector3();
    //    Quaternion rot = new Quaternion();
    //    Transform lTransform = characterController.transform;
    //    //---------------------------------------------------
    //    if (stream.isWriting)
    //    {
    //        pos = lTransform.position;
    //        rot = lTransform.rotation;
    //    }
    //    //---------------------------------------------------
    //    stream.Serialize(ref pos);
    //    stream.Serialize(ref rot);
    //    stream.Serialize(ref moveV);
    //    //---------------------------------------------------
    //    if (stream.isReading)
    //    {
    //        lTransform.position = pos;
    //        lTransform.rotation = rot;
    //    }
    //}

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

    public static int toSerializeNetworkView(UnitFaceDirection pFace)
    {
        return pFace == UnitFaceDirection.left ? 0 : 1;
    }

    public static UnitFaceDirection fromSerializeNetworkView(int pValue)
    {
        return pValue == 0 ? UnitFaceDirection.left : UnitFaceDirection.right;
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

    const int faceLeftCommand = 1 << 30;
    const int negFaceLeftCommand = ~faceLeftCommand;

    const int faceRightCommand      = 1 << 31;
    const int negFaceRightCommand = ~faceRightCommand;

    const int faceLeftValue = 1 << 0;
    const int faceRightValue = ~faceLeftValue;

    const int faceUpCommand         = 1 << 1;
    const int negFaceUpCommand = ~faceUpCommand;

    const int faceDownCommand       = 1 << 2;
    const int negFaceDownCommand = ~faceDownCommand;

    const int goForwardCommand      = 1 << 3;
    const int negGoForwardCommand = ~goForwardCommand;

    const int fireCommand           = 1 << 4;
    const int negFireCommand = ~fireCommand;

    const int jumpCommand           = 1 << 5;
    const int negJumpCommand = ~jumpCommand;


    public UnitFaceDirection face
    {
        get
        {
            return (command & faceLeftValue) != 0 ?
                UnitFaceDirection.left : UnitFaceDirection.right;
        }
        set
        {
            //Debug.Log("UnitFaceDirection");
            //Debug.Log(value);
            //Debug.Log(System.Convert.ToString(command, 2));
            switch (value)
            {
                case UnitFaceDirection.left:
                    command |= faceLeftValue;
                    break;
                case UnitFaceDirection.right:
                    command &= faceRightValue;
                    break;
            }
            //Debug.Log(System.Convert.ToString(command, 2));
            //Debug.Log(face);
        }
    }

    public void calculateFace(UnitFaceDirection pLastFace)
    {
        //if (FaceLeft != FaceRight)
        //{
        //    if (FaceLeft && pLastFace == UnitFaceDirection.right)
        //    {
        //        face = UnitFaceDirection.left;
        //        //Debug.Log("Change to left");
        //        return true;
        //    }
        //    if (FaceRight && pLastFace == UnitFaceDirection.left)
        //    {
        //        face = UnitFaceDirection.right;
        //        //Debug.Log("Change to right");
        //        return true;
        //    }
        //}
        //face = pLastFace;
        //return false;
        if (FaceLeft != FaceRight)
        {
            if (FaceLeft)
            {
                face = UnitFaceDirection.left;
                return ;
            }
            else//if (FaceRight)
            {
                face = UnitFaceDirection.right;
                return ;
            }
        }
        face = pLastFace;
    }

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
    protected UnitFaceDirection _lastFace = UnitFaceDirection.left;

    //返回在x上的值
    public int getFaceValue()
    {
        //Debug.Log("face:"+face+" "+UnitFace.getValue(face));
        return UnitFace.getValue(face);
    }

    public UnitFaceDirection face
    {
        get { return unitActionCommand.face; }
    }

    void Awake()
    {
        unitActionCommand.face = _lastFace;
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
        pUnitActionCommand.calculateFace(unitActionCommand.face);
        unitActionCommand.command = pUnitActionCommand.command;
    }

    public UnitActionCommand getCommand()
    {
        return new UnitActionCommand(unitActionCommand);
    }

    //根据现在命令的方向更新朝向,若朝向有变则返回true
    public bool updateFace()
    {
        if (_lastFace == unitActionCommand.face)
            return false;
        _lastFace = unitActionCommand.face;
        return true;
    }

    public int commandValue
    {
        get { return unitActionCommand.command; }
        set { unitActionCommand.command = value; }
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
