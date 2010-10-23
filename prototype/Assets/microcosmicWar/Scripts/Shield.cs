using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public GameObject shield;
    public float countTimer = 10;          //����ʱ��
    public float changeSpeed = 1;          //�仯�ٶ�
    public int adversaryLayer = 11;

    private Vector3 vector;                 //����ײ�������
    private ShieldBasic shieldLeft = new ShieldBasic();     //���滤��
    private ShieldBasic shieldRight = new ShieldBasic();    //���滤��
    private float Timerbefore;                  //��ײʱ��ʱ��
    private float Timerafter;                   //��ײ���ʱ��
    private GameObject clone;                   //������Prefab����

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
    //�жϱ仯ʱ��
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
    //͸���ȱ仯
	public void alphaChange () {
        if (clone)
        {
            SetColor();
            Change(shieldLeft);
            Change(shieldRight);
        }
	}
    //�����ܳ�ʼ��
    private void ShieldInstantiate()
    {
        clone = zzCreatorUtility.Instantiate(shield, transform.position, transform.rotation, 0);
        shieldLeft.shield = clone.transform.Find("ShieldLeft");
        shieldRight.shield = clone.transform.Find("ShieldRight");
    }
    //������ɫ,͸����
    private void SetColor()
    {
        shieldLeft.shield.renderer.material.color = new Color(1, 1, 1, shieldLeft.alpha);
        shieldRight.shield.renderer.material.color = new Color(1, 1, 1, shieldRight.alpha);
    }
    //��������ʧЧ��
    private void ShieldDispear(ShieldBasic changeShield)
    {
        if ((float)(Time.time - Timerbefore) > countTimer)
        {
            if (changeShield.alpha > 0)
                changeShield.alpha = changeShield.alpha - changeSpeed * Time.deltaTime;
        }
    }
    //��������ʾЧ��
    private void ShieldAppear(ShieldBasic changeShield)
    {
        if (changeShield.alpha < 1)
        {
            changeShield.alpha = changeShield.alpha + changeSpeed * Time.deltaTime;
            Timerbefore = Time.time;
        }
    }
}