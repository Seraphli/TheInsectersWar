using UnityEngine;

public class zzSerializeSelfID:MonoBehaviour
{
    [zzSerialize]
    public int ID
    {
        get
        {
            return gameObject.GetInstanceID();
        }
        set
        {
            zzGetObjectByID.addObject(value, gameObject);
        }
    }
}