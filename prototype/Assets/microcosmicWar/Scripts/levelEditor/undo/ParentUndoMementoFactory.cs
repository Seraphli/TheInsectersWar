﻿using UnityEngine;

public class ParentUndoMementoFactory : zzUndo.UndoMementoFactory
{
    public override zzUndo.UndoMemento createUndoMemento()
    {
        return new ParentUndoMemento();
    }
    public override System.Type undoType
    {
        get { return typeof(GameObject); }
    }

    class ParentUndoMemento : zzUndo.UndoMemento
    {
        Transform parent;
        GameObject undoObject;
        bool objectActive;
        public void save(object pObject)
        {
            undoObject = ((GameObject)pObject);
            objectActive = undoObject.active;
            parent = undoObject.transform.parent;
        }
        public void restore(object pObject)
        {
            undoObject.transform.parent = parent;
            undoObject.SetActiveRecursively(objectActive);
        }

        public void clear() 
        {
            Debug.Log("clear:" + undoObject.name);
            Destroy(undoObject);
        }

    }
}