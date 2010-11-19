using UnityEngine;
using System.Collections;

public class StartSupply : MonoBehaviour {

	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame

	public GameObject planeLeft;
	public GameObject planeRight;
	public GameObject supplyBox;
	private GameObject plane;
	public float speed=10;
	private bool supplyBool=false;
	private bool boxBool=false;
	private float randomX;
	private float end;
	
	private float smoothF;
	
	
	void Update () 
	{
		if(supplyBool)
		{
			
			plane.transform.position=new Vector3(plane.transform.position.x+speed*smoothF,plane.transform.position.y,plane.transform.position.z);
			if((plane.transform.position.x-end)<5.0f&&(plane.transform.position.x-end)>-5.0f)
			{
				if(!boxBool)
				{
					Destroy(plane);	
					supplyBool=false;
				}
			}
			
			
			
			
			if(boxBool)
			{
				supplyBox.transform.position=new Vector3(supplyBox.transform.position.x+speed*smoothF,supplyBox.transform.position.y,supplyBox.transform.position.z);
				if((supplyBox.transform.position.x-randomX)<1.0f&&(supplyBox.transform.position.x-randomX)>-1.0f)
				{
					supplyBox.GetComponent<Rigidbody>().isKinematic =false;
					//supplyBox.GetComponent<Rigidbody>().AddForce(new Vector3(speed*smoothF*100000,0,0));
					boxBool=false;
				}
					
			}
		}
	
	}
	
	
	
	
	
	
	public void startSupplyPlane(float velocity,float startX,float endX,float hightY)
	{
		if(supplyBool!=true)
		{
			supplyBool=true;
			boxBool=true;
			
		}
		else
		{
				return ;
		}
		end=endX;
		
		
		supplyBox=Instantiate (supplyBox,new Vector3(startX, hightY-1.0f, 0), Quaternion.identity) as GameObject;
		supplyBox.GetComponent<Rigidbody>().isKinematic =true;
		
		randomX=Random.Range(startX,endX);
		
		if(startX<endX)
		{
				smoothF=Time.deltaTime;
				
				plane=Instantiate (planeRight,new Vector3(startX,hightY, 0), Quaternion.identity) as GameObject;
		}
		else
		{
				smoothF=Time.deltaTime*-1;
				plane=Instantiate (planeLeft,new Vector3(startX,hightY, 0), Quaternion.identity) as GameObject;
				
		}
		
	}
	
	
	
}
