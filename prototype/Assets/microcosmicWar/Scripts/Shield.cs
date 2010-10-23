using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public GameObject shield;
    public float countTimer = 10;          //持续时间
    public float changeSpeed = 1;          //变化速度
    public int adversaryLayer = 11;

    private Vector3 vector;                 //到碰撞点的向量
    private ShieldBasic shieldLeft = new ShieldBasic();     //左面护甲
    private ShieldBasic shieldRight = new ShieldBasic();    //右面护甲
    private float Timerbefore;                  //碰撞时的时间
    private float Timerafter;                   //碰撞后的时间
    private GameObject clone;                   //防护盾Prefab副本

	// Use this for initialization
	void Awake () {
        ShieldInstantiate();
        SetColor();
	}
    // Update is called once per frame
    void Update()
    {
        alphaChange();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == adversaryLayer)
        {
            vector = other.transform.position - transform.position;
            Destroy(other.gameObject);
            Timerbefore = Time.time;
            if (vector.x >= 0)
            {
                shieldLeft.use = true;
            }
            if (vector.x <= 0)
            {
                shieldRight.use = true;
            }
        }
    }
    //判断变化时间
    private void Change(ShieldBasic shield)
        {
            if (shield.use == true && (float)(Time.time - Timerbefore) < countTimer)
            {
                ShieldAppear(shield);
            }
            else
            {
                shield.use = false;
                ShieldDispear(shield);
            }
        }
    //透明度变化
	public void alphaChange () {
        if (clone)
        {
            SetColor();
            Change(shieldLeft);
            Change(shieldRight);
        }
	}
    //防护盾初始化
    private void ShieldInstantiate()
    {
        clone = zzCreatorUtility.Instantiate(shield, transform.position, transform.rotation, 0);
        shieldLeft.shield = clone.transform.Find("ShieldLeft");
        shieldRight.shield = clone.transform.Find("ShieldRight");
    }
    //设置颜色,透明度
    private void SetColor()
    {
        shieldLeft.shield.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
        shieldRight.shield.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    }
    //防护盾消失效果
    private void ShieldDispear(ShieldBasic changeShield)
    {
        if ((float)(Time.time - Timerbefore) > countTimer)
        {
            if (changeShield.alpha > 0)
                changeShield.alpha = changeShield.alpha - changeSpeed * Time.deltaTime;
        }
    }
    //防护盾显示效果
    private void ShieldAppear(ShieldBasic changeShield)
    {
        if (changeShield.alpha < 1)
        {
            changeShield.alpha = changeShield.alpha + changeSpeed * Time.deltaTime;
            Timerbefore = Time.time;
        }
    }
}