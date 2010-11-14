using UnityEngine;
using System.Collections;

public class SniperEntrance : MonoBehaviour 
{
    //标志狙击台是否开启
    private bool status=true;
    //狙击的数据
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
    /// 设置狙击台的状态
    /// </summary>
    /// <param name="statusT">true开启 false关闭</param>
    public void setStatus(bool statusT)
    {
        status = statusT;
    }

    /// <summary>
    /// 返回狙击台的状态
    /// </summary>
    /// <returns>true开启 false关闭</returns>
    public bool getStatus()
    {
        return status;
    }

    /// <summary>
    /// 返回狙击数据 
    /// </summary>
    /// <returns></returns>
    public SniperData getSniperData()
    {
        return sniperData;
    }
}
