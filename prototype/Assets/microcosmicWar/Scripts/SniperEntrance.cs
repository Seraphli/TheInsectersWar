using UnityEngine;
using System.Collections;

public class SniperEntrance : MonoBehaviour 
{
    //��־�ѻ�̨�Ƿ���
    private bool status=true;
    //�ѻ�������
    private SniperData sniperData;
	
	public  int damage =500;
	public int interval=3;



	// Use this for initialization
	void Start () {
        sniperData.damage = damage;
		sniperData.interval =interval;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /// <summary>
    /// ���þѻ�̨��״̬
    /// </summary>
    /// <param name="statusT">true���� false�ر�</param>
    public void setStatus(bool statusT)
    {
        status = statusT;
    }

    /// <summary>
    /// ���ؾѻ�̨��״̬
    /// </summary>
    /// <returns>true���� false�ر�</returns>
    public bool getStatus()
    {
        return status;
    }

    /// <summary>
    /// ���ؾѻ����� 
    /// </summary>
    /// <returns></returns>
    public SniperData getSniperData()
    {
        return sniperData;
    }
}
