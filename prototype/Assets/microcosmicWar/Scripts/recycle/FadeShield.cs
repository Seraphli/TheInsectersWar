using UnityEngine;

public class FadeShield : MonoBehaviour
{
    [System.Serializable]
    public class ShieldBasic
    {

        public float alpha = 0;

        /// <summary>
        /// 碰撞时的时间
        /// </summary>
        public float allAppearTimePos = 0;

        /// <summary>
        /// 完全显示后 持续的时间
        /// </summary>
        public float allAppearContinueTime;

        /// <summary>
        /// 变化速度
        /// </summary>
        public float changeSpeed;

        public bool use = false;
        private Transform shield;

        public Transform shieldTransform
        {
            //get { return shield; }
            set
            {
                shield = value;
                applyColor();
            }
        }
        public Color color;

        /// <summary>
        /// 判断变化时间
        /// </summary>
        public void Change()
        {
            ShieldBasic lShield = this;
            if (lShield.use == true && (float)(Time.time - allAppearTimePos) < allAppearContinueTime)
            {
                ShieldAppear();
            }
            else
            {
                lShield.use = false;
                ShieldDispear();
            }
            applyColor();
        }

        /// <summary>
        /// 防护盾消失效果
        /// </summary>
        private void ShieldDispear()
        {
            ShieldBasic changeShield = this;
            if ((float)(Time.time - allAppearTimePos) > allAppearContinueTime)
            {
                if (changeShield.alpha > 0)
                    changeShield.alpha = changeShield.alpha - changeSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        /// 防护盾显示效果
        /// </summary>
        private void ShieldAppear()
        {
            ShieldBasic changeShield = this;
            if (changeShield.alpha < 1)
            {
                changeShield.alpha = changeShield.alpha + changeSpeed * Time.deltaTime;
                allAppearTimePos = Time.time;
            }
        }

        private void applyColor()
        {
            shield.renderer.material.color = new Color(1, 1, 1, alpha);
        }

        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            stream.Serialize(ref alpha);
            stream.Serialize(ref allAppearTimePos);
        }
    }

    //public GameObject shield;

    /// <summary>
    /// 完全显示后 持续的时间
    /// </summary>
    public float allAppearContinueTime = 0.4f;
    public float changeSpeed = 1;          //变化速度
    public int adversaryWeaponLayer = 11;   //阻挡的子弹的层

    private Vector3 vector;                 //到碰撞点的向量
    private ShieldBasic shieldLeft = new ShieldBasic();     //左面护甲
    private ShieldBasic shieldRight = new ShieldBasic();    //右面护甲
    //private float Timerbefore;                  //碰撞时的时间
    //private float Timerafter;                   //碰撞后的时间
    private GameObject shieldObject;                   //防护盾Prefab副本

    // Use this for initialization
    //void Awake()
    //{
    //}

    void Start()
    {
        shieldObject = gameObject;
        ShieldInstantiate();
        //SetColor();

        shieldLeft.allAppearContinueTime = allAppearContinueTime;
        shieldRight.allAppearContinueTime = allAppearContinueTime;

        shieldLeft.changeSpeed = changeSpeed;
        shieldRight.changeSpeed = changeSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        alphaChange();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!zzCreatorUtility.isHost())
            return;

        if (other.gameObject.layer == adversaryWeaponLayer)
        {
            vector = other.transform.position - transform.position;

            //使用将生命值设为0的方式,消除子弹
            Life lLife = Life.getLifeFromTransform(other.transform);

            lLife.setBloodValue(0);

            //Timerbefore = Time.time;
            if (vector.x >= 0)
            {
                shieldRight.use = true;
                shieldRight.allAppearTimePos = Time.time;
            }
            else if (vector.x <= 0)
            {
                shieldLeft.use = true;
                shieldLeft.allAppearTimePos = Time.time;
            }
        }
    }
    //透明度变化
    public void alphaChange()
    {
        //if (shieldObject)
        //{
        //SetColor();
        shieldLeft.Change();
        shieldRight.Change();
        //}
    }
    //防护盾初始化
    private void ShieldInstantiate()
    {
        //shieldObject = zzCreatorUtility.Instantiate(shield, transform.position, transform.rotation, 0);
        shieldLeft.shieldTransform = shieldObject.transform.Find("ShieldLeft");
        shieldRight.shieldTransform = shieldObject.transform.Find("ShieldRight");
    }

    public void setOwner(GameObject pOwner)
    {
        _setOwner(pOwner);
        if (Network.peerType != NetworkPeerType.Disconnected)
            gameObject.networkView.RPC("RPCSetOwner", RPCMode.Others, pOwner.networkView.viewID);
    }

    void _setOwner(GameObject pOwner)
    {
        transform.parent = pOwner.transform;
        transform.localPosition = Vector3.zero;
    }

    [RPC]
    void RPCSetOwner(NetworkViewID pOwner)
    {
        _setOwner(NetworkView.Find(pOwner).gameObject);
    }

    //设置颜色,透明度
    //private void SetColor()
    //{
    //    shieldLeft.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
    //    shieldRight.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    //}
    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        shieldLeft.OnSerializeNetworkView(stream, info);
        shieldRight.OnSerializeNetworkView(stream, info);
    }
}