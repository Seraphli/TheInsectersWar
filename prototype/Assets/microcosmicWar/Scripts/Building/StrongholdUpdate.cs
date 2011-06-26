using UnityEngine;

public class StrongholdUpdate:MonoBehaviour
{
    public GameObject attachmentPrefab;
    public Transform attachmentParent;

    public float waitTime = 10f;

    zzTimer updateTimer;

    public Race race = Race.eNone;

    public string updateAnimation;

    public Animation strongholdAnimation;

    void Start()
    {
        if(zzCreatorUtility.isHost())
        {
            updateTimer = gameObject.AddComponent<zzTimer>();
            if (race == Race.eNone)
                updateTimer.enabled = false;
            else
                occupied(race);
            updateTimer.addImpFunction(updateStronghold);

        }
    }

    void occupied(Race pRace)
    {
        race = pRace;
        updateTimer.interval = waitTime;
    }

    void updateStronghold()
    {
        updateTimer.enabled = false;
        Destroy(updateTimer);
        if (Network.peerType == NetworkPeerType.Disconnected)
            attachUpdate(((GameObject)Instantiate(attachmentPrefab)).transform);
        else
        {
            var lAttachmentParent = (GameObject)Network.Instantiate(attachmentPrefab,
                attachmentParent.position, attachmentParent.rotation,0);
            attachUpdate(lAttachmentParent.transform);
            networkView.RPC("RPCAttachUpdate", RPCMode.Others,
                lAttachmentParent.networkView.viewID);
        }
    }

    [RPC]
    void RPCAttachUpdate(NetworkViewID pViewID)
    {
        attachUpdate(NetworkView.Find(pViewID).transform);
    }

    void attachUpdate(Transform pAttachment)
    {
        strongholdAnimation.Play(updateAnimation);
        pAttachment.parent = attachmentParent;
        pAttachment.localPosition = Vector3.zero;
        pAttachment.localRotation = Quaternion.identity;
        pAttachment.gameObject.layer = gameObject.layer;
    }
}