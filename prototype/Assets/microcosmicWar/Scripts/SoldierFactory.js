//#pragma strict

var adversaryName="";

var finalAim:Transform;

var produceInterval=1.0;

var soldierToProduce:GameObject;

protected var timePos=0.0;

protected var adversaryLayerValue:int;

var produceTransform:Transform;

//Component.SendMessage ("dieCallFunction")
//var dieCallFunction:Component;
var objectListener=IobjectListener();

function Start()
{
	if(!produceTransform)
		produceTransform=transform;
		
	collisionLayer.addCollider(gameObject);
	
	adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
	
	var lLife:Life=gameObject.GetComponent(Life);
	//lLife.setDieCallback(dieCall);
	lLife.addDieCallback(dieCall);
	
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
			var lClone = zzCreatorUtility.Instantiate(soldierToProduce, produceTransform.position, Quaternion(), 0);
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
	//if(dieCallFunction)
	//	dieCallFunction.SendMessage ("dieCallFunction");
	//else
	if(objectListener)
		objectListener.removedCall();
	Destroy(gameObject);
		//GameScene.getSingleton().gameResult(adversaryName);
}