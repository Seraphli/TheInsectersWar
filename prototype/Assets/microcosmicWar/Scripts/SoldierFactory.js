//#pragma strict

var adversaryName="";

var finalAim:Transform;

var produceInterval=1.0;

var soldierToProduce:GameObject;

protected var timePos=0.0;

protected var adversaryLayerValue:int;


function Start()
{
	collisionLayer.addCollider(gameObject);
	
	adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
	
	var lLife:Life=gameObject.GetComponent(Life);
	lLife.setDieCallback(dieCall);
	
	if(!zzCreatorUtility.isHost())
		Destroy(this);
}

function Update () {
	//if(zzCreatorUtility.isHost())
	//{
		timePos+=Time.deltaTime;
		if(timePos>produceInterval)
		{
			//var lClone = Network.Instantiate(soldierToProduce, transform.position+Vector3(0,2.5,0), Quaternion(), 0);
			var lClone = zzCreatorUtility.Instantiate(soldierToProduce, transform.position+Vector3(0,2.5,0), Quaternion(), 0);
			timePos=0.0;
			var soldierAI:SoldierAI = lClone.GetComponent(SoldierAI);
			soldierAI.SetFinalAim(finalAim);
			soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
			//lClone.GetComponent(SoldierAI).SetSoldier(lClone.GetComponent(Soldier));
			//lClone.GetComponent(SoldierAI).SetAdversaryLayerValue(adversaryLayerValue);
		}
	//}
}

function dieCall()
{
	GameScene.getSingleton().gameResult(adversaryName);
}