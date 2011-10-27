using UnityEngine;

public class ShopUI:MonoBehaviour
{
    [System.Serializable]
    public class ItemUIControl
    {
        public ItemUIControl() { }
        public ItemUIControl(zzInterfaceGUI pItem)
        {
            item = pItem;
        }
        [SerializeField]
        zzInterfaceGUI _item;
        public zzInterfaceGUI item
        {
            get { return _item; }
            set
            {
                _item = value;
                iconImage = value.getSubElement("icon");
                countLabel = value.getSubElement("count");
                changedCountLabel = value.getSubElement("changed");
                priceLabel = value.getSubElement("price");
            }
        }

        public bool visible
        {
            get { return _item.visible; }
            set { _item.visible = value; }
        }

        //图标
        public zzInterfaceGUI iconImage;
        //买或卖价
        public zzInterfaceGUI priceLabel;
        //现在的数量
        public zzInterfaceGUI countLabel;
        //改变的数量
        public zzInterfaceGUI changedCountLabel;

        public Texture2D icon
        {
            set { iconImage.setImage(value); }
        }

        public int price
        {
            set { priceLabel.setText("$"+value.ToString()); }
        }

        public int count
        {
            set { countLabel.setText(value.ToString()); }
        }

        public int changedCount
        {
            set
            {
                if (value == 0)
                    changedCountLabel.setText("");
                else if (value > 0)
                    changedCountLabel.setText("+" + value.ToString());
                else
                    changedCountLabel.setText(value.ToString());
            }
        }
    }

    public int flowerItemCount
    {
        get { return flowerItemUIList.Length; }
    }

    public ItemUIControl[] flowerItemUIControl;
    public ItemUIControl[] equipmentItemUIControl;

    public zzInterfaceGUI[] flowerItemUIList;
    public zzInterfaceGUI[] equipmentUIList;

    public zzInterfaceGUI costLabel;

    public int resultPurse
    {
        set
        {
            //if (value > 0)
            //    costLabel.setText("-$" + value);
            //else if (value < 0)
            //    costLabel.setText("+$" + (-value));
            //else
            //    costLabel.setText("$0");
            costLabel.setText("$" + value);
        }
    }

    public zzGUIAniToTargetAngle panToTargetAngle;

    public float angularSpeed = 180f;

    public void rotateToTarget(float pTarget)
    {
        panToTargetAngle.rotateToTarget(pTarget);
    }

    public float panAngle
    {
        get { return panToTargetAngle.angle; }
        set { panToTargetAngle.angle = value; }
    }

    void Start()
    {
        flowerItemUIControl = new ItemUIControl[flowerItemUIList.Length];
        for (int i = 0; i < flowerItemUIList.Length;++i )
        {
            flowerItemUIControl[i] =new ItemUIControl( flowerItemUIList[i] );
        }

        equipmentItemUIControl = new ItemUIControl[equipmentUIList.Length];
        for (int i = 0; i < equipmentUIList.Length; ++i)
        {
            equipmentItemUIControl[i] = new ItemUIControl(equipmentUIList[i]);
        }
    }

    public void clear()
    {
        foreach (var lControl in flowerItemUIControl)
        {
            lControl.visible = false;
        }
        foreach (var lControl in equipmentItemUIControl)
        {
            lControl.visible = false;
        }
    }

}