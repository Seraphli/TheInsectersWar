using UnityEngine;
using System.Collections;

public class StartSupply:MonoBehaviour{

	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame

	//void Update () 
	//{
	
	//}
	public  GameObject plane;
	private  GameObject planeGame;
	
	
	
	
	
	public  void startSupplyPlane(float velocity,float startX,float endX,float heightY)
	{
		if(create(startX,heightY))
		{
			SupplyData data;
			data.velocity=velocity;
			data.startX=startX;
			data.endX=endX;
			data.heightY=heightY;
			planeGame.GetComponent<SupplyAirplane>().startPlane(data);
		}
		
	}
	public  void startSupplyPlane(SupplyData data)
	{
		if(create(data.startX,data.heightY))
		{
			planeGame.GetComponent<SupplyAirplane>().startPlane(data);
		}
		
	}
	
	
	private  bool create(float x,float y)
	{
		if(planeGame==null)
		{
			planeGame=Instantiate (plane,new Vector3(x,y,0), Quaternion.identity) as GameObject;
			return true;
		}
		return false;
	}
	
	
}
