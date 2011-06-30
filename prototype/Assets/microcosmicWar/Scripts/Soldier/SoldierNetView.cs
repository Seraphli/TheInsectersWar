
using UnityEngine;
using System.Collections;

public class SoldierNetView : MonoBehaviour
{


    public Soldier soldier;
    public Life life;
    public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    //FIXME_VAR_TYPE transform;
    public float disappearTime = 3f;
    public zzTimer disappearTimer;
    public MonoBehaviour[] disenableWhenDisappear;

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

        if (Network.isClient)
        {
            Destroy(soldier.GetComponentInChildren<SoldierAI>());
            disappearTimer = gameObject.AddComponent<zzTimer>();
            disappearTimer.setInterval(disappearTime);
            disappearTimer.addImpFunction(disappear);
        }
        if(disenableWhenDisappear ==null||disenableWhenDisappear.Length==0)
        {
            disenableWhenDisappear = new MonoBehaviour[] { soldier };
        }
        //if( !zzCreatorUtility.isMine(gameObject.networkView ) )
        //{
        //	Destroy(soldier.GetComponentInChildren<SoldierAI>());
        //}
        //if(!soldier)
        //	Debug.LogError(gameObject.name);
    }

    readonly Vector3 disappearPostion = new Vector3(-100f, -100f, 0f);

    void disappear()
    {
        gameObject.transform.position = disappearPostion;
        disappearTimer.enabled = false;
        foreach (var lScript in disenableWhenDisappear)
        {
            lScript.enabled = false;
        }
    }

    void appear()
    {
        disappearTimer.timePos = 0f;
        foreach (var lScript in disenableWhenDisappear)
        {
            lScript.enabled = true;
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isReading)
            appear();
        life.OnSerializeNetworkView(stream, info);
        actionCommandControl.OnSerializeNetworkView(stream, info);
        character.OnSerializeNetworkView2D(stream, info);
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