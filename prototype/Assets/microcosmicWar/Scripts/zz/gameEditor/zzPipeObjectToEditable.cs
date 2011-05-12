using UnityEngine;

//can delete
public class zzPipeObjectToEditable:MonoBehaviour
{
    //System.Func<GameObject> pipeIn;
    System.Action<GameObject> pipeOut;

    public void pipeEnter( GameObject pObject)
    {
        var lResult = zzEditableObjectContainer.findRoot(pObject);
        if (lResult)
            pipeOut(lResult.gameObject);
        else
            pipeOut(null);
    }

    public void addPipeOut(System.Action<GameObject> pPipeOut)
    {
        pipeOut += pPipeOut;
    }

}