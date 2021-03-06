using UnityEngine;
using System.Collections;

public class SupplyAirplane : MonoBehaviour {
	public GameObject crateToCreate;

    [SerializeField]
	private GameObject _supplyBox;

    public GameObject supplyBox
    {
        get { return _supplyBox; }
    }

	private bool supplyBool=false;
	private bool boxHaveThrown=false;
	//private float randomNumber;
	//每秒移动的距离
    [SerializeField]
	private float velocity;

    [SerializeField]
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
	
	
    //void Start()
    //{

    //}
	
	// Update is called once per frame
	void Update ()
    {
        if (!supplyBool)
        {
            return;
        }

        Vector3 lNowPos = gameObject.transform.position;
        gameObject.transform.position 
            = new Vector3(lNowPos.x + velocity * Time.deltaTime, lNowPos.y, 0);
        if (!boxHaveThrown)
            _supplyBox.transform.position = gameObject.transform.position;

        if (zzCreatorUtility.isHost())
        {
            //跑出范围,销毁
            if (identicalBool(gameObject.transform.position.x, data.endX))
            {
                if (boxHaveThrown)
                {
                    zzCreatorUtility.Destroy(gameObject);
                    supplyBool = false;
                }
            }


            if (!boxHaveThrown)
            {
                //到达投放点,投放
                if (identicalBool(_supplyBox.transform.position.x, data.putX))
                {
                    animation.Play("jia");
                    _supplyBox.GetComponent<Rigidbody>().isKinematic = false;
                    //supplyBox.GetComponent<Rigidbody>().AddForce(new Vector3(speed*smoothF*100000,0,0));
                    _supplyBox.GetComponent<Rigidbody>().velocity = new Vector3(velocity, 0, 0);
                    boxHaveThrown = true;
                    print(boxHaveThrown);
                }

            }
        }

        

    }

    [RPC]
    public void setTransportedObject(NetworkViewID pID)
    {
        _supplyBox = NetworkView.Find(pID).gameObject;
        //_supplyBox.gameObject.networkView.enabled = false;
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
			velocity=data.velocity*1;
		}
		else
		{
			//randomNumber=Random.Range(data.startX-10.0f,data.endX+10.0f);
			velocity=data.velocity*-1;
		}

        _supplyBox = zzCreatorUtility.Instantiate(
            crateToCreate,
            transform.position,
            Quaternion.identity, 0) as GameObject;

        if(Network.peerType!=NetworkPeerType.Disconnected)
            gameObject.networkView.RPC("setTransportedObject", RPCMode.Others, _supplyBox.networkView.viewID);
        //_supplyBox.gameObject.networkView.enabled = false;

        _supplyBox.GetComponent<Rigidbody>().isKinematic = true;
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

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        var lPlanePos = new Vector3();
        //var lBosIsKinematic = true;
        //var lBoxPos =  new Vector3();
        if(stream.isWriting)
        {
            lPlanePos = transform.position;
            //lBosIsKinematic = _supplyBox.GetComponent<Rigidbody>().isKinematic;
            //lBoxPos = _supplyBox.transform.position;
        }

        stream.Serialize(ref velocity);
        stream.Serialize(ref supplyBool);
        stream.Serialize(ref boxHaveThrown);
        //stream.Serialize(ref lBosIsKinematic);
        stream.Serialize(ref lPlanePos);
        //stream.Serialize(ref lBoxPos);

        if (stream.isReading)
        {
            transform.position = lPlanePos;
            //_supplyBox.GetComponent<Rigidbody>().isKinematic = lBosIsKinematic;
            //_supplyBox.transform.position = lBoxPos;
        }

        //if (!lBosIsKinematic)
        //{
        //    _supplyBox.gameObject.networkView.enabled = true;
        //}
    }
	
	
	
	
	
	
	
	
	
}
