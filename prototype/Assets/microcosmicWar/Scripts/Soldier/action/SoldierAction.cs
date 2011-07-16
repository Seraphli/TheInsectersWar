using UnityEngine;

public abstract class SoldierAction:MonoBehaviour
{
    public ActionCommandControl actionCommandControl;
    public Character2D character;
    public CharacterController characterController;
    public bool canMove;
    public int commandValue;

    public virtual bool inActing
    {
        get { return _inActing; }

        set { _inActing = value; }
    }

    protected void Awake()
    {
        characterController = character.characterController;
    }
    //public virtual bool canInterrupt
    //{
    //    get { return true; }
    //}

    //public virtual bool canJump
    //{
    //    get { return false; }
    //}

    //public virtual bool canXMove
    //{
    //    get { return false; }
    //}

    [SerializeField]
    protected bool _inActing = false;

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns>是否成功开启动作</returns>
    //public virtual bool beginAction()
    //{
    //    _inActing = true;
    //    return true;
    //}

    public abstract void processCommand( UnitActionCommand pUnitActionCommand);
    //public virtual void interruptAction() { }

}