using UnityEngine;
using System.Collections;

public class SupplyAirplane : MonoBehaviour {
	public GameObject crateToCreate;
	private GameObject _supplyBox;
    public GameObject supplyBox
    {
        get { return _supplyBox; }
    }
	private bool supplyBool=false;
	private bool boxBool=false;
	//private float randomNumber;
	//每秒移动的距离
	private float distance;
	private FlyInfo data;


    [System.Serializable]
    public class FlyInfo
    {
        public float velocity;
        public float startX;
		public float putX;
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
				_supplyBox.transform.position=gameObject.transform.position;
				if(identicalBool(_supplyBox.transform.position.x,data.putX))
				{
					animation.Play("jia");
					_supplyBox.GetComponent<Rigidbody>().isKinematic =false;
					//supplyBox.GetComponent<Rigidbody>().AddForce(new Vector3(speed*smoothF*100000,0,0));
					_supplyBox.GetComponent<Rigidbody>().velocity = new Vector3(distance, 0, 0);
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
			gameObject.transform.localScale+=new Vector3(0,0,-2*gameObject.transform.localScale.y);
			gameObject.transform.rotation= Quaternion.Euler(0,180, 0);
			
			//randomNumber=Random.Range(data.startX+10.0f,data.endX-10.0f);
			distance=data.velocity*1;
		}
		else
		{
			//randomNumber=Random.Range(data.startX-10.0f,data.endX+10.0f);
			distance=data.velocity*-1;
		}
		
		_supplyBox=Instantiate (crateToCreate,gameObject.transform.position, Quaternion.identity) as GameObject;
		
		
		_supplyBox.GetComponent<Rigidbody>().isKinematic =true;
		
		
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
