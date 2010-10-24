using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
    [System.Serializable]
    public class ShieldBasic
    {

        public float alpha = 0;
        public float Timerbefore = 0;         //碰撞时的时间
        public float countTimer;              //持续时间
        public float changeSpeed;         //变化速度

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

        //判断变化时间
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

        //防护盾消失效果
        private void ShieldDispear()
        {
            ShieldBasic changeShield = this;
            if ((float)(Time.time - Timerbefore) > countTimer)
            {
                if (changeShield.alpha > 0)
                    changeShield.alpha = changeShield.alpha - changeSpeed * Time.deltaTime;
            }
        }
        //防护盾显示效果
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
    public float countTimer = 10;          //持续时间
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

            //使用将生命值设为0的方式,消除子弹
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
    //设置颜色,透明度
    //private void SetColor()
    //{
    //    shieldLeft.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
    //    shieldRight.shieldTransform.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    //}
}