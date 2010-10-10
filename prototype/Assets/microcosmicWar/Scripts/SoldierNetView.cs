
using UnityEngine;
using System.Collections;

public class SoldierNetView : MonoBehaviour
{


    public Soldier soldier;
    public Life life;
    public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    //FIXME_VAR_TYPE transform;

    void Awake()
    {
        if (!soldier)
            soldier = gameObject.GetComponentInChildren<Soldier>();
        //if(!soldier)
        //	soldier=gameObject.GetComponentInChildren<Soldier>().getCharacter();
        //character = gameObject.GetComponentInChildren<Soldier>().getCharacter();
        character = soldier.getCharacter();
        actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();
        if (!life)
            life = gameObject.GetComponentInChildren<Life>();

        if (!zzCreatorUtility.isHost())
        {
            Destroy(soldier.GetComponentInChildren<SoldierAI>());
        }
        //if( !zzCreatorUtility.isMine(gameObject.networkView ) )
        //{
        //	Destroy(soldier.GetComponentInChildren<SoldierAI>());
        //}
        //if(!soldier)
        //	Debug.LogError(gameObject.name);
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        life.OnSerializeNetworkView(stream, info);
        character.OnSerializeNetworkView(stream, info);
        actionCommandControl.OnSerializeNetworkView(stream, info);
        /*
        FIXME_VAR_TYPE pos=Vector3();
        FIXME_VAR_TYPE rot=Quaternion();
        FIXME_VAR_TYPE lVelocity=Vector3();
        FIXME_VAR_TYPE lActionCommand= UnitActionCommand();
        //float lBloodValue;
	
        //---------------------------------------------------
        if (stream.isWriting)
        {
            pos=transform.position;
            rot=transform.rotation;
            lVelocity = soldier.getVelocity();
            lActionCommand= soldier.getCommand();
            //lBloodValue=life.getBloodValue();
            //FIXME_VAR_TYPE cc;
        }
	
        //---------------------------------------------------
        stream.Serialize(pos);
        stream.Serialize(rot);
        stream.Serialize(lVelocity);
        //stream.Serialize(lBloodValue);
        stream.Serialize(lActionCommand.FaceLeft);
        stream.Serialize(lActionCommand.FaceRight);
        stream.Serialize(lActionCommand.GoForward);
        stream.Serialize(lActionCommand.Fire);
        stream.Serialize(lActionCommand.Jump);
	
        //---------------------------------------------------
        if(stream.isReading)
        {
            transform.position=pos;
            transform.rotation=rot;
		
            soldier.setVelocity(lVelocity);
            soldier.setCommand(lActionCommand);
		
        }
        */
    }

    //void  Update (){
    //}
}