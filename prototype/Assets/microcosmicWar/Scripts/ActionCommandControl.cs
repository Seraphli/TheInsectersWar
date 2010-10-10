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
                    moveV.y = -0.01f;	//���������
            }
        }
        else
            moveV.y -= gravity * Time.deltaTime;

        // Move the controller
        Vector3 lVelocity = new Vector3(moveV.x * runSpeed, moveV.y, 0);

        characterController.Move(lVelocity * Time.deltaTime);
    }

    public bool isGrounded()
    {
        return characterController.isGrounded;
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
}

public class UnitActionCommand
{
    //None,

    public UnitActionCommand(){}

    public UnitActionCommand(UnitActionCommand pOther)
    {
        FaceLeft = pOther.FaceLeft;//������
        FaceRight = pOther.FaceRight;//������
        FaceUp = pOther.FaceUp;
        FaceDown = pOther.FaceDown;
        GoForward = pOther.GoForward;//ǰ��
        Fire = pOther.Fire;//����
        Jump = pOther.Jump;//��Ծ

    }

    public bool FaceLeft = false;//������
    public bool FaceRight = false;//������
    public bool FaceUp = false;
    public bool FaceDown = false;
    public bool GoForward = false;//ǰ��
    public bool Fire = false;//����
    public bool Jump = false;//��Ծ

    public void clear()
    {
        //MoveLeft=false;
        //MoveRight=false;
        FaceLeft = false;
        FaceRight = false;
        FaceUp = false;
        FaceDown = false;

        GoForward = false;

        Fire = false;
        Jump = false;
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

    //������x�ϵ�ֵ
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

    //������������ķ�����³���,�������б��򷵻�true
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
        //int lFace = face;
        //stream.Serialize(lFace);
        //face =  lFace;

        //stream.Serialize(unitActionCommand.FaceLeft);
        //stream.Serialize(unitActionCommand.FaceRight);
        //stream.Serialize(unitActionCommand.FaceUp);
        //stream.Serialize(unitActionCommand.FaceDown);

        //stream.Serialize(unitActionCommand.GoForward);

        //stream.Serialize(unitActionCommand.Fire);
        //stream.Serialize(unitActionCommand.Jump);

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
