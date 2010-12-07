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
    public GameObject planeToCreate;
    //private  GameObject planeGame;

    public delegate void InitSupplyObjectFunc(GameObject pGameObject);

    static void initSupplyObjectNullFunc(GameObject pGameObject)
    {}

    public InitSupplyObjectFunc initSupplyObjectFunc = initSupplyObjectNullFunc;


	public  void startSupplyPlane(float velocity,float startX,float putX,float endX,float heightY)
	{
        GameObject lPlane = create(startX,heightY);
        if (lPlane)
		{
            SupplyAirplane.FlyInfo data = new SupplyAirplane.FlyInfo();
			data.velocity=velocity;
			data.startX=startX;
			data.putX=putX;
			data.endX=endX;
			data.heightY=heightY;
            //lPlane.GetComponent<SupplyAirplane>().startPlane(data);
            initPlane(lPlane, data);
		}
		
	}
    public void startSupplyPlane(SupplyAirplane.FlyInfo data)
    {
        GameObject lPlane = create(data.startX, data.heightY);
		if(lPlane)
		{
            initPlane(lPlane, data);
		}
		
	}

    void initPlane(GameObject pPlane, SupplyAirplane.FlyInfo data)
    {
        SupplyAirplane lSupplyAirplane = pPlane.GetComponent<SupplyAirplane>();
        lSupplyAirplane.startPlane(data);
        initSupplyObjectFunc(lSupplyAirplane.supplyBox);
    }


    private GameObject create(float x, float y)
	{
        GameObject lOut = null;
        //if(planeGame==null)
        //{
            lOut = (GameObject)Instantiate(planeToCreate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            //return true;
        //}
        return lOut;
	}
	
	
}
