using UnityEngine;
using System.Collections.Generic;

public class zzUndo:MonoBehaviour
{

    #region 单实例
    static protected zzUndo singletonInstance;

    public static zzUndo Singleton
    {
        get { return singletonInstance; }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance != null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
    } 
    #endregion

    void Start()
    {
        registerUndoMemento();
    }

    public interface UndoMemento
    {
        void save(object pObject);
        void restore(object pObject);
    }

    public abstract class UndoMementoFactory:MonoBehaviour
    {
        public abstract UndoMemento createUndoMemento();
        public abstract System.Type undoType { get; }
    }

    class TransformUndoMemento : UndoMemento
    {
        Vector3 position;
        Quaternion rotation;
        Vector3 scale;
        public void save(object pObject)
        {
            var lTransform =(Transform)pObject;
            position = lTransform.localPosition;
            rotation = lTransform.localRotation;
            scale = lTransform.localScale;
        }
        public void restore(object pObject)
        {
            var lTransform = (Transform)pObject;
            lTransform.localPosition = position;
            lTransform.localRotation = rotation;
            lTransform.localScale = scale;
        }
    }

    public UndoMementoFactory[] undoMementoFactory;

    public int maxUndoCount = 100;

    Dictionary<System.Type, System.Func<UndoMemento>> typeToUndo 
        = new Dictionary<System.Type,System.Func<UndoMemento>>();

    void registerUndoMemento()
    {
        registerUndoMemento(typeof(Transform),()=>new TransformUndoMemento());
        foreach (var lUndoMementoFactory in undoMementoFactory)
        {
            registerUndoMemento(lUndoMementoFactory.undoType,
                lUndoMementoFactory.createUndoMemento);
        }
    }

    void registerUndoMemento(System.Type pUndoType,System.Func<UndoMemento> pFunc)
    {
        typeToUndo[pUndoType] = pFunc;
    }

    struct UndoObject
    {
        public Object objectToUndo;
        public UndoMemento undoMemento;
        public UndoMemento oppositeUndoMemento;
        public System.Func<UndoMemento> createMementoFunc;

        public UndoObject(Object pObjectToUndo, System.Func<UndoMemento> pCreateMementoFunc)
        {
            createMementoFunc = pCreateMementoFunc;
            objectToUndo = pObjectToUndo;
            undoMemento = createMementoFunc();
            undoMemento.save(objectToUndo);
            oppositeUndoMemento = null;
        }

        public void swapUndo()
        {
            if (oppositeUndoMemento == null)
            {
                oppositeUndoMemento = createMementoFunc();
                oppositeUndoMemento.save(objectToUndo);
            }
            undoMemento.restore(objectToUndo);
            var lTemp = undoMemento;
            undoMemento = oppositeUndoMemento;
            oppositeUndoMemento = lTemp;
        }
    }

    System.Func<UndoMemento> getUndoMementoCreateFun(System.Type pType)
    {
        return typeToUndo[pType];
    }

    LinkedList<UndoObject> undoList = new LinkedList<UndoObject>();
    //List<Object> undoObjectList;
    LinkedList<UndoObject> redoList = new LinkedList<UndoObject>();
    //List<Object> redoObjectList;
    void _registerUndo(object pObjectToUndo)
    {
        undoList.AddLast(new UndoObject((Object)pObjectToUndo, 
            getUndoMementoCreateFun(pObjectToUndo.GetType())));
        redoList.Clear();
        while (undoList.Count > maxUndoCount)
        {
            undoList.RemoveFirst();
        }
    }

    public static void registerUndo(object pObjectToUndo)
    {
        singletonInstance._registerUndo(pObjectToUndo);
    }

    void transportUndo(LinkedList<UndoObject> pFrom, LinkedList<UndoObject> pTo)
    {
        if (pFrom.Count < 1)
            return;
        var lUndoObject = pFrom.Last.Value;
        pFrom.RemoveLast();
        lUndoObject.swapUndo();
        pTo.AddLast(lUndoObject);
    }

    void _performUndo()
    {
        transportUndo(undoList, redoList);
    }

    public static void performUndo() 
    {
        singletonInstance._performUndo();
    }

    void _performRedo() 
    {
        transportUndo(redoList, undoList);
    }

    public static void performRedo()
    {
        singletonInstance._performRedo();
    }
}