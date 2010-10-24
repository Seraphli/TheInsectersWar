using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
    [System.Serializable]
    public class ShieldBasic
    {

        public float alpha = 0;
        public float Timerbefore = 0;         //��ײʱ��ʱ��
        public float countTimer;              //����ʱ��
        public float changeSpeed;         //�仯�ٶ�

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

        //�жϱ仯ʱ��
        public void Change()
        {
            ShieldBasic lShield = this;
            if (lShield.use == true && (float)(Time.time - Timerbefore) < countTimer)
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

        //��������ʧЧ��
        private void ShieldDispear()
        {
            ShieldBasic changeShield = this;
            if ((float)(Time.time - Timerbefore) > countTimer)
            {
                if (changeShield.alpha > 0)
                    changeShield.alpha = changeShield.alpha - changeSpeed * Time.deltaTime;
            }
        }
        //��������ʾЧ��
        private void ShieldAppear()
        {
            ShieldBasic changeShield = this;
            if (changeShield.alpha < 1)
            {
                changeShield.alpha = changeShield.alpha + changeSpeed * Time.deltaTime;
                Timerbefore = Time.time;
            }
        }

        private void applyColor()
        {
            shield.renderer.material.color = new Color(1, 1, 1, alpha);
        }
    }

    public GameObject shield;
    public float countTimer = 10;          //����ʱ��
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

        shieldLeft.countTimer = countTimer;
        shieldRight.countTimer = countTimer;

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
        if (other.gameObject.layer == adversaryWeaponLayer)
        {
            vector = other.transform.position - transform.position;

            //ʹ�ý�����ֵ��Ϊ0�ķ�ʽ,�����ӵ�
            Life lLife = other.gameObject.GetComponent<Life>();
            lLife.setBloodValue(0);

            //Timerbefore = Time.time;
            if (vector.x >= 0)
            {
                shieldRight.use = true;
                shieldRight.Timerbefore = Time.time;
            }
            else if (vector.x <= 0)
            {
                shieldLeft.use = true;
                shieldLeft.Timerbefore = Time.time;
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
    //������ɫ,͸����
    //private void SetColor()
    //{
    //    shieldLeft.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
    //    shieldRight.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    //}
}