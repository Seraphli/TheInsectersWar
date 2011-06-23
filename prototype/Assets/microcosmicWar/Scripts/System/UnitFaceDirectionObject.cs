using UnityEngine;

public class UnitFaceDirectionObject:MonoBehaviour
{
    public Transform turnObject;

    [SerializeField]
    UnitFaceDirection _face = UnitFaceDirection.left;

    public UnitFaceDirection face
    {
        get { return _face; }
        set 
        {
            if (_face != value)
            {
                setFace(value);
                if (Network.peerType != NetworkPeerType.Disconnected)
                    networkView.RPC("RPCSetFace", RPCMode.Others, (char)value);
            }
        }
    }

    [RPC]
    void RPCSetFace(char pFace)
    {
        setFace((UnitFaceDirection)pFace);
    }

    void setFace(UnitFaceDirection pFace)
    {
        _face = pFace;
        var lScale = turnObject.localScale;
        lScale.x = -lScale.x;
        turnObject.localScale = lScale;
    }

    //原始的朝向
    //public UnitFaceDirection originalFace = UnitFaceDirection.left;
}