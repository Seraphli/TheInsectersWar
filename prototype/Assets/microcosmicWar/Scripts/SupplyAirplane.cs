using UnityEngine;
using System.Collections;

public class SupplyAirplane : MonoBehaviour {
	public GameObject crate;
	private GameObject supplyBox;
	private bool supplyBool=false;
	private bool boxBool=false;
	private float randomNumber;
	//每秒移动的距离
	private float distance;
	private FlyInfo data;


    [System.Serializable]
    public class FlyInfo
    {
        public float velocity;
        public float startX;
        public float endX;
        public float heightY;
    }
	
	

	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	void Update () {
	if(supplyBool)
		{
			
			gameObject.transform.position=new Vector3(gameObject.transform.position.x+distance*Time.deltaTime,gameObject.transform.position.y,0);
			if(identicalBool(gameObject.transform.position.x,data.endX))
			{
				if(boxBool)
				{
					Destroy(gameObject);	
					supplyBool=false;
				}
			}
			
			
			if(!boxBool)
			{
				supplyBox.transform.position=gameObject.transform.position;
				if(identicalBool(supplyBox.transform.position.x,randomNumber))
				{
					animation.Play("jia");
					supplyBox.GetComponent<Rigidbody>().isKinematic =false;
					//supplyBox.GetComponent<Rigidbody>().AddForce(new Vector3(speed*smoothF*100000,0,0));
					supplyBox.GetComponent<Rigidbody>().velocity = new Vector3(distance, 0, 0);
					boxBool=true;
					print(boxBool);
				}
					
			}
		}
	
	}
	
	//启动空投 成功返回true
	public bool startPlane(FlyInfo dataTemp)
	{
		if(supplyBool==true)
		{
				return false;
		}

		supplyBool=true;
		data=dataTemp;
		
		if(data.startX<data.endX)
		{
			gameObject.transform.localScale=new Vector3(-1,1,1);
			randomNumber=Random.Range(data.startX+10.0f,data.endX-10.0f);
			distance=data.velocity*1;
		}
		else
		{
			randomNumber=Random.Range(data.startX-10.0f,data.endX+10.0f);
			distance=data.velocity*-1;
		}
		
		supplyBox=Instantiate (crate,gameObject.transform.position, Quaternion.identity) as GameObject;
		
		
		supplyBox.GetComponent<Rigidbody>().isKinematic =true;
		
		
		return true;
	}
	
	public bool identicalBool(float x1,float x2)
	{
		if((x1-x2)<=1.0f&&(x1-x2)>=-1.0f)
		{
			return true;
		}
		return false;
	}
	
	
	
	
	
	
	
	
	
}
