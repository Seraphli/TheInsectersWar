using UnityEngine;
using System.Collections.Generic;

public class CustomObjectType:zzEditableObject
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

    [SerializeField]
    bool haveInit = false;

    [SerializeField]
    Board board;

    List<Collider> objectCollider;

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

            //ToDo:优化,等模型先生成时,模型会被setLayer两次
            //被设置类型时,都要更新,因为至少要更新MapManagerType
            updateObjectType();
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
            objectType = (ObjectType)System.Enum.Parse(typeof(ObjectType), value);
        }
    }

    void updateObjectType()
    {
        objectCollider = new List<Collider>();
        GameSceneManager.MapManagerType lMapManagerType;
        switch (_objectType)
        {
            case ObjectType.ground:
                setLayer(layers.ground, GameSceneManager.MapManagerType.ground);
                removeBoard();
                break;
            case ObjectType.board:
                setLayer(layers.board, GameSceneManager.MapManagerType.board);
                getBoard().boardColliders = objectCollider.ToArray();
                break;
            case ObjectType.moveableObject:
                setLayer(layers.moveableObject, GameSceneManager.MapManagerType.moveableObject);
                removeBoard();
                break;
            default:
                Debug.LogError("switch(_objectType)");
                break;
        }
        objectCollider = null;

    }

    void removeBoard()
    {
        if(board)
        {
            Destroy(board);
            board = null;
        }
    }

    Board getBoard()
    {
        if (!board)
            board = gameObject.AddComponent<Board>();
        return board;
    }


    void Start()
    {
        if(!haveInit)
        {
            updateObjectType();
            haveInit = true;
        }
    }

    void setObjectCollider(Transform pObject, int pLayerIndex)
    {
        if (pObject.collider)
        {
            objectCollider.Add(pObject.collider);
            pObject.gameObject.layer = pLayerIndex;
        }
    }

    void setLayer(int pLayerIndex,GameSceneManager.MapManagerType pMapManagerType)
    {
        GetComponent<WMGameObjectType>().mapType = pMapManagerType;

        setObjectCollider(objectRoot.transform, pLayerIndex);
        foreach (Transform lObject in objectRoot.transform)
        {
            setObjectCollider(lObject, pLayerIndex);
        }
    }
    
}