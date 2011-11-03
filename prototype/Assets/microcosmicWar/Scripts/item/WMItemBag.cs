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

        //使用时的效果
        void doEffect(GameObject pHero);
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
        //在包里的位置
        public int bagItemIndex;
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
            set
            {
                if (cell != null)
                    cell.count = value;
            }
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

        public void doEffect(GameObject pObject)
        {
            cell.doEffect(pObject);
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

        public void changed()
        {
            changedEvent();
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

    public BagSpace getByItemID(int pItemID)
    {
        return System.Array.Find(items, (x) => x.itemId == pItemID);
    }

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
            for (int a = 0; a < items.Length;++a )
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
        for (int i = 0; i < items.Length; ++i)
        {
            var lBagSpace = items[i];
            lBagSpace.bagItemIndex = i;
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
        {
            items[pIndex].doEffect(getOwnerFunc());
            networkView.RPC("ItemBagUse", RPCMode.Server, pIndex);
        }
    }

    //only in server
    [RPC]
    void ItemBagUse(int pIndex)
    {
        var lOwner = getOwnerFunc();
        items[pIndex].doEffect(lOwner);
        items[pIndex].use(lOwner);
    }

    //      数量          物品ID    包位置索引
    //|<-----14----->|<-----14----->|<---4--->|
    //only in server
    [RPC]
    void ItemBagChange(int pData)
    {
        var lBitData = new zz.IntBitIO(pData);
        ItemBagChange(lBitData.readToInt(4), lBitData.readToInt(14), lBitData.readToInt(14));
    }

    [RPC]
    void ItemBagChange2(int pData1,int pData2)
    {
        ItemBagChange(pData1);
        ItemBagChange(pData2);
    }

    [RPC]
    void ItemBagChange3(int pData1, int pData2, int pData3)
    {
        ItemBagChange(pData1);
        ItemBagChange(pData2);
        ItemBagChange(pData3);
    }

    [RPC]
    void ItemBagChange4(int pData1, int pData2, int pData3, int pData4)
    {
        ItemBagChange(pData1);
        ItemBagChange(pData2);
        ItemBagChange(pData3);
        ItemBagChange(pData4);
    }

    [RPC]
    void ItemBagChange5(int pData1, int pData2, int pData3, int pData4, int pData5)
    {
        ItemBagChange(pData1);
        ItemBagChange(pData2);
        ItemBagChange(pData3);
        ItemBagChange(pData4);
        ItemBagChange(pData5);
    }

    int toInt(ItemChangeInfo pItemChangeInfo)
    {
        var lBitData = new zz.IntBitIO();
        lBitData.write(pItemChangeInfo.count, 14);
        lBitData.write(pItemChangeInfo.itemId, 14);
        lBitData.write(pItemChangeInfo.index, 4);
        return lBitData.date;
    }

    public class ItemChangeInfo
    {
        public int index;
        public int itemId;
        public int count;
    }

    public void ItemBagChange(ItemChangeInfo[] pItemChangeInfo)
    {
        if (Network.isClient)
        {
            switch (pItemChangeInfo.Length)
            {
                case 1: networkView.RPC("ItemBagChange", RPCMode.Server,
                    toInt(pItemChangeInfo[0]));
                    break;
                case 2: networkView.RPC("ItemBagChange2", RPCMode.Server,
                    toInt(pItemChangeInfo[0]),
                    toInt(pItemChangeInfo[1]));
                    break;
                case 3: networkView.RPC("ItemBagChange3", RPCMode.Server,
                    toInt(pItemChangeInfo[0]),
                    toInt(pItemChangeInfo[1]),
                    toInt(pItemChangeInfo[2]));
                    break;
                case 4: networkView.RPC("ItemBagChange4", RPCMode.Server,
                    toInt(pItemChangeInfo[0]),
                    toInt(pItemChangeInfo[1]),
                    toInt(pItemChangeInfo[2]),
                    toInt(pItemChangeInfo[3]));
                    break;
                case 5: networkView.RPC("ItemBagChange5", RPCMode.Server,
                    toInt(pItemChangeInfo[0]),
                    toInt(pItemChangeInfo[1]),
                    toInt(pItemChangeInfo[2]),
                    toInt(pItemChangeInfo[3]),
                    toInt(pItemChangeInfo[4]));
                    break;
                default:
                    Debug.LogError("error pItemChangeInfo.Length:" + pItemChangeInfo.Length);
                    break;
            }
        }
        else
        {
            foreach (var lInfo in pItemChangeInfo)
            {
                ItemBagChange(lInfo.index, lInfo.itemId, lInfo.count);
            }
        }
    }

    public void ItemBagChange(int pIndex, int pItemId, int pCount)
    {
        var lBagSpace = items[pIndex];
        if (pItemId==0)
        {
            lBagSpace.clear();
        }
        else
        {
            if (lBagSpace.itemId != pItemId)
                changeCell(lBagSpace, pItemId);
            lBagSpace.count = pCount;
        }
        lBagSpace.changed();
    }
}