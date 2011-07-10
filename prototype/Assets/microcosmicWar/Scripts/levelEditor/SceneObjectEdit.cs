using UnityEngine;

public class SceneObjectEdit:MonoBehaviour
{
    public Transform recycleBin;

    public delegate void AddObjectEvent(GameObject pObject);

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    public GameObject[] selectedObjects;

    public void selectObject(GameObject pObject)
    {
        var lResult = zzEditableObjectContainer.findRoot(pObject);
        if (lResult)
            setObjectEdited(lResult.gameObject);
    }

    public void setObjectEdited(GameObject pObject)
    {
        selectedObjects = new GameObject[1] { pObject };
    }

    public void deleteObject()
    {
        if (selectedObjects.Length == 1
            && selectedObjects[0].GetComponent<EditorObject>() != null)
            return;

        foreach (var lObject in selectedObjects)
        {
            if (lObject.active && lObject.GetComponent<EditorObject>() == null)
            {
                zzUndo.registerUndo(lObject);
                lObject.SetActiveRecursively(false);
                lObject.transform.parent = recycleBin;
            }
        }
        selectedObjects = new GameObject[0] { };
    }

    public void copyObject()
    {
        foreach (var lObject in selectedObjects)
        {
            if (lObject.active && lObject.GetComponent<EditorObject>() == null)
                addObjectEvent((GameObject)Instantiate(lObject));
        }
    }
}