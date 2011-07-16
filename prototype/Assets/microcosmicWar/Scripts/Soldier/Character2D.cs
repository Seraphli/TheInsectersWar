using UnityEngine;

public class Character2D:MonoBehaviour
{
    public CharacterController characterController;

    public float runSpeed = 5.0f;
    public float gravity = 15.0f;
    public float jumpSpeed = 10.0f;
    public float minYVelocity = -10.0f;

    //因为网络同步中也有update(推进了时间),所以不能用Time.DeltaTime
    public float lastUpdateTime = 0f;

    public float yVelocity;

    void Awake()
    {
        lastUpdateTime = Time.time;
    }

    public void update2D(UnitActionCommand pUnitActionCommand, int pFaceValue, bool isAlive)
    {
        update2D(pUnitActionCommand, pFaceValue, isAlive, Time.time - lastUpdateTime);
        lastUpdateTime = Time.time;
    }

    public const float yNullVelocity = -0.01f;

    public void update2D(UnitActionCommand pUnitActionCommand, int pFaceValue, bool isAlive, float pDeltaTime)
    {

        if (!isAlive | !pUnitActionCommand.GoForward)
            pFaceValue = 0;

        var lLastYVelocity = yVelocity;

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
            && (characterController.collisionFlags & CollisionFlags.Above) > 0)
            yVelocity = 0;

        // Move the controller
        Vector3 lMove = new Vector3(pFaceValue * runSpeed * pDeltaTime,
            (yVelocity + lLastYVelocity) / 2f * pDeltaTime, 0);

        characterController.Move(lMove);
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

}