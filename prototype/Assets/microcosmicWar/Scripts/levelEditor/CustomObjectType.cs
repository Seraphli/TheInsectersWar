using UnityEngine;
using System.Collections.Generic;

public class CustomObjectType:MonoBehaviour
{
    public GameObject objectRoot;

    public enum ObjectType
    {
        ground,
        board,
        moveableObject,
    }

    [SerializeField]
    ObjectType _objectType;

    List<Collider> objectCollider = new List<Collider>();

    [EnumUI(new string[] { "地面", "跳板", "物体" },
         new int[] { (int)ObjectType.ground, (int)ObjectType.board, (int)ObjectType.moveableObject })]
    public ObjectType objectType
    {
        get
        {
            return _objectType;
        }

        set
        {
            _objectType = value;
        }
    }

    [zzSerialize]
    public string objectTypeID
    {
        get
        {
            return _objectType.ToString();
        }

        set
        {
            _objectType = (ObjectType)System.Enum.Parse(typeof(ObjectType), value);
        }
    }


    void Start()
    {
        switch(_objectType)
        {
            case ObjectType.ground:
                setLayer(layers.ground, GameSceneManager.MapManagerType.ground);
                break;
            case ObjectType.board:
                setLayer(layers.board, GameSceneManager.MapManagerType.board);
                gameObject.AddComponent<Board>().boardColliders = objectCollider.ToArray();
                break;
            case ObjectType.moveableObject:
                setLayer(layers.moveableObject, GameSceneManager.MapManagerType.moveableObject);
                break;
            default:
                Debug.LogError("switch(_objectType)");
                break;
        }
        objectCollider = null;
    }

    void addObjectCollider(Transform pObject )
    {
        if (pObject.collider)
            objectCollider.Add(pObject.collider);
    }

    void setLayer(int layerIndex,GameSceneManager.MapManagerType pMapManagerType)
    {
        GameSceneManager.Singleton.addObject(pMapManagerType, objectRoot);
        objectRoot.layer = layerIndex;
        addObjectCollider(objectRoot.transform);
        foreach (Transform lObject in objectRoot.transform)
        {
            addObjectCollider(lObject);
            lObject.gameObject.layer = layerIndex;
        }
    }
    
}