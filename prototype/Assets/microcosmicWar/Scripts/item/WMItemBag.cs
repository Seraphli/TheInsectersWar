using UnityEngine;

namespace WM
{
    public enum BagCellState
    {
        none,
        inUsing,
    }

    public interface IBagCell
    {
        BagCellState state{ get; set; }
        bool use(GameObject pHero);
        void addChangedReceiver(System.Action Receiver);
        void release();

        //子弹需要时时变动数字
        int count { get; set; }
    }
}

//[System.Serializable]
//public class WMBagCell
//{
//    public WM.IBagCell control;
//    public WM.BagCellState state
//    {
//        get { return control.count; }
//    }
//    [SerializeField]
//    int _itemIndex;
//    public int itemIndex
//    {
//        get { return _itemIndex; }
//    }

//    [SerializeField]
//    int _count;

//    public void use(GameObject pHero)
//    {

//    }

//    public void addChangedReceiver(System.Action Receiver)
//    {

//    }
//}

public class WMItemBag:MonoBehaviour
{
    [System.Serializable]
    public class BagSpace
    {
        public const int noneId = 0;
        public int itemId;
        public bool canSale = true;
        public bool unlimited = false;
        WM.IBagCell _cell;

        public void clear()
        {
            itemId = noneId;
            _cell = null;
        }

        public WM.IBagCell cell
        {
            get { return _cell; }
            set
            {
                _cell = value;
                _cell.addChangedReceiver(cellChange);
            }
        }

        public void cellChange()
        {
            if (!unlimited && count == 0)
                clear();
            changedEvent();
        }

        public int count
        {
            set { cell.count = value; }
            get 
            {
                if (cell!=null)
                    return cell.count;
                return 0;
            }
        }

        public void use()
        {
            if (!isEmpty)
            {
                useAction();
            }
        }

        public void use(GameObject pObject)
        {
            cell.use(pObject);
        }

        public System.Action useAction;

        public bool isEmpty
        {
            get { return itemId == noneId; }
        }

        System.Action changedEvent;

        public void addChangedReceiver(System.Action pReceiver)
        {
            changedEvent += pReceiver;
        }

        //public bool haveThing
        //{
        //    get { return cell != noneId; }
        //}
    }

    System.Action bagUpdateEvent;

    public System.Func<GameObject> getOwnerFunc;

    public void addBagInitEndReceiver(System.Action pReceiver)
    {
        bagUpdateEvent+=pReceiver;
        if (started)
            pReceiver();
    }

    bool started = false;

    public BagSpace[] items = new BagSpace[] { };

    [System.Serializable]
    public class BagCellInfo
    {
        public string name;
        public bool unlimited = false;
        public int count;
        public bool canSale = true;
    }

    public BagCellInfo[] itemsInfo = new BagCellInfo[] { };
    NetworkPlayer _owner;
    bool ownerSetted = false;
    public NetworkPlayer owner
    {
        get { return _owner; }
        set
        {
            _owner = value;
            ownerSetted = true;
            if (started)
                initNetworkUpdate();
        }
    }

    void initNetworkUpdate()
    {
        if (ownerSetted
            && Network.isServer
            && owner != Network.player)
        {
            for (int a = 0; a < itemsInfo.Length;++a )
            {
                int i = a;
                var lBagSpace = items[i];
                lBagSpace.addChangedReceiver(
                    () => networkView.RPC("ItemBagUpdate", owner,
                        i, lBagSpace.itemId, lBagSpace.count));
            }
        }
    }

    void Start()
    {
        var lItemSystem = WMItemSystem.Singleton;
        for (int i = 0; i < itemsInfo.Length; ++i)
        {
            var lBagSpace = items[i];
            BagCellInfo lInfo;
            if (i < itemsInfo.Length)
            {
                lInfo = itemsInfo[i];
                if (!string.IsNullOrEmpty(lInfo.name))
                {
                    var lItem = lItemSystem.getItem(lInfo.name);
                    changeCell(lBagSpace, lItem.id);
                }
            }
            else
                lInfo = new BagCellInfo();
            //lBagSpace.itemId = lItem.id;
            //lBagSpace.cell = lItem.bagCellCreator.getBagCell();
            lBagSpace.canSale = lInfo.canSale;
            lBagSpace.unlimited = lInfo.unlimited;
            lBagSpace.count = lInfo.count;
            var lI = i;
            lBagSpace.useAction = () => use(lI);
            //if (Network.isServer
            //    && owner
            //    && owner != Network.player)
            //    lBagSpace.addChangedReceiver(
            //        () => networkView.RPC("ItemBagUpdate", owner,
            //            lI, lBagSpace.itemId, lBagSpace.count));
        }
        initNetworkUpdate();
        started = true;
        if (bagUpdateEvent != null)
            bagUpdateEvent();
    }

    void changeCell(BagSpace lBagSpace, int pItemID)
    {
        var lItem = WMItemSystem.Singleton.getItem(pItemID);
        lBagSpace.itemId = lItem.id;
        lBagSpace.cell = lItem.bagCellCreator.getBagCell();
    }

    //only in client
    [RPC]
    void ItemBagUpdate(int pIndex,int pItemID,int pCount)
    {
        var lItem = items[pIndex];
        if (pItemID == BagSpace.noneId)
            lItem.clear();
        else
        {
            if (lItem.itemId != pItemID)
                changeCell(lItem, pItemID);
            lItem.count = pCount;
        }
        lItem.cellChange();
    }

    void use(int pIndex)
    {
        if (items[pIndex].isEmpty)
            return;
        if (!Network.isClient)
            ItemBagUse(pIndex);
        else
            networkView.RPC("ItemBagUse", RPCMode.Server, pIndex);
    }

    //only in server
    [RPC]
    void ItemBagUse(int pIndex)
    {
        items[pIndex].use(getOwnerFunc());
    }
}