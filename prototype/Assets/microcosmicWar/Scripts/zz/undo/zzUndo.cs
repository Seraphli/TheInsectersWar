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
        void clear();
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

        public void clear() { }
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

    LinkedList<List<UndoObject>> undoList = new LinkedList<List<UndoObject>>();
    //List<Object> undoObjectList;
    LinkedList<List<UndoObject>> redoList = new LinkedList<List<UndoObject>>();
    //List<Object> redoObjectList;

    public int nowUndoCount = 0;
    void _registerUndo(object[] pObjectsToUndo)
    {
        List<UndoObject> lUndoObjects = new List<UndoObject>(pObjectsToUndo.Length);
        foreach (var lObjectToUndo in pObjectsToUndo)
        {
            lUndoObjects.Add(new UndoObject((Object)lObjectToUndo,
            getUndoMementoCreateFun(lObjectToUndo.GetType())));
        }
        undoList.AddLast(lUndoObjects);
        redoList.Clear();
        while (undoList.Count > maxUndoCount)
        {
            foreach (var lUndoObject in undoList.First.Value)
            {
                lUndoObject.undoMemento.clear();
            }

            undoList.RemoveFirst();
        }
        nowUndoCount = undoList.Count;
    }

    void _registerUndo(object pObjectToUndo)
    {
        _registerUndo(new object[] { pObjectToUndo });
        //undoList.AddLast(new UndoObject((Object)pObjectToUndo, 
        //    getUndoMementoCreateFun(pObjectToUndo.GetType())));
        //redoList.Clear();
        //while (undoList.Count > maxUndoCount)
        //{
        //    undoList.First.Value.undoMemento.clear();
        //    undoList.RemoveFirst();
        //}
        //nowUndoCount = undoList.Count;
    }

    public static void registerUndo(object pObjectToUndo)
    {
        singletonInstance._registerUndo(pObjectToUndo);
    }

    public static void registerUndo(object[] pObjectsToUndo)
    {
        singletonInstance._registerUndo(pObjectsToUndo);
    }

    void transportUndo(LinkedList<List<UndoObject>> pFrom, LinkedList<List<UndoObject>> pTo)
    {
        if (pFrom.Count < 1)
            return;
        var lUndoObjects = pFrom.Last.Value;
        pFrom.RemoveLast();
        foreach (var lUndoObject in lUndoObjects)
        {
            lUndoObject.swapUndo();
        }
        pTo.AddLast(lUndoObjects);
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