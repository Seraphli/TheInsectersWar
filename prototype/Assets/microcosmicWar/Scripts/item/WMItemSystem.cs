using UnityEngine;
using System.Collections.Generic;

public abstract class WMBagCellCreator:MonoBehaviour
{
    public abstract WM.IBagCell getBagCell();
}

public class WMGenericBagCell:WM.IBagCell
{

    public WM.BagCellState state 
    {
        get { return WM.BagCellState.none; }
        set {}
    }

    int _count;

    public int count
    {
        get { return _count; }
        set { _count = value; }
    }

    static void nullDoEffectFunc(GameObject p){}

    public System.Func<GameObject, bool> useFunc;
    public System.Action<GameObject> doEffectFunc = nullDoEffectFunc;
    System.Action changedReceiver;

    public void doEffect(GameObject pHero)
    {
        doEffectFunc(pHero);
    }

    public bool use(GameObject pHero)
    {
        if (pHero.GetComponent<Life>().isDead())
            return false;
        if(useFunc(pHero))
        {
            count = count - 1;
            changedReceiver();
            return true;
        }
        return false;
    }

    public void addChangedReceiver(System.Action pReceiver)
    {
        changedReceiver += pReceiver;
    }
    public void release(){}
}

public class WMItemSystem:MonoBehaviour
{
    [System.Serializable]
    public class InfoElement
    {
        public int id;
        public string name;
        public string showName;
        public Texture2D image;
        public bool isShow = true;
        public int maxCount = 999;

        //oneMode 下maxCount无用
        public bool oneMode = false;

        public int buyingPrice = 10;
        public int sellingPrice = 8;

        public WMBagCellCreator bagCellCreator;
    }

    [System.Serializable]
    public class InfoNode
    {
        //名字不能重复
        public string name;
        public string showName;
        public Texture2D image;
        public Texture2D expandedImage;
        public InfoNode[] nodes = new InfoNode[0];
        public InfoElement[] elements = new InfoElement[0];
    }

    public List<InfoElement> items = new List<InfoElement>();
    public Dictionary<string, InfoElement> nameToItem = new Dictionary<string, InfoElement>();
    public Dictionary<string, InfoNode> typeToNode = new Dictionary<string, InfoNode>();
    //public WMItemInfo[] items = new WMItemInfo[]{};
    public InfoNode itemTree;

    public InfoNode getTypeTree(string pTypeName)
    {
        InfoNode lOut;
        if (typeToNode.TryGetValue(pTypeName, out lOut))
            return lOut;
        return null;
    }

    public InfoElement getItem(string pName)
    {
        return nameToItem[pName];
    }

    public InfoElement getItem(int pID)
    {
        return items[pID];
    }

    void Awake()
    {
        if (singletonInstance != null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        init();
    }

    void init()
    {
        //让ID从1开始
        items.Add(null);
        addItemElements(itemTree);
    }

    void addItemElement(InfoElement pInfoElement)
    {
        pInfoElement.id = items.Count;
        items.Add(pInfoElement);
        nameToItem[pInfoElement.name] = pInfoElement;
    }

    void addItemElements(InfoNode pNode)
    {
        foreach (var lElement in pNode.elements)
        {
            addItemElement(lElement);
        }
        foreach (var lNode in pNode.nodes)
        {
            typeToNode[lNode.name] = lNode;
            addItemElements(lNode);
        }
    }
    static protected WMItemSystem singletonInstance;

    public static WMItemSystem Singleton
    {
        get { return singletonInstance; }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }
}