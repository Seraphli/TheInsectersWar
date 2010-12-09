using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
    [System.Serializable]
    public class ShieldBasic
    {

        public float alpha = 0;

        /// <summary>
        /// ��ײʱ��ʱ��
        /// </summary>
        public float allAppearTimePos = 0;     
    
        /// <summary>
        /// ��ȫ��ʾ�� ������ʱ��
        /// </summary>
        public float allAppearContinueTime ;

        /// <summary>
        /// �仯�ٶ�
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
        /// �жϱ仯ʱ��
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
        /// ��������ʧЧ��
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
        /// ��������ʾЧ��
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
    /// ��ȫ��ʾ�� ������ʱ��
    /// </summary>
    public float allAppearContinueTime = 0.4f; 
    public float changeSpeed = 1;          //�仯�ٶ�
    public int adversaryWeaponLayer = 11;   //�赲���ӵ��Ĳ�

    private Vector3 vector;                 //����ײ�������
    private ShieldBasic shieldLeft = new ShieldBasic();     //���滤��
    private ShieldBasic shieldRight = new ShieldBasic();    //���滤��
    //private float Timerbefore;                  //��ײʱ��ʱ��
    //private float Timerafter;                   //��ײ���ʱ��
    private GameObject shieldObject;                   //������Prefab����

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

            //ʹ�ý�����ֵ��Ϊ0�ķ�ʽ,�����ӵ�
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
    //͸���ȱ仯
    public void alphaChange()
    {
        //if (shieldObject)
        //{
            //SetColor();
            shieldLeft.Change();
            shieldRight.Change();
        //}
    }
    //�����ܳ�ʼ��
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

    //������ɫ,͸����
    //private void SetColor()
    //{
    //    shieldLeft.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
    //    shieldRight.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    //}
    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        shieldLeft.OnSerializeNetworkView(stream,info);
        shieldRight.OnSerializeNetworkView(stream, info);
    }
}