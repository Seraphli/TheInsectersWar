
var messageReceiver:Transform;

function Start()
{
	//if(!messageReceiver)
	//	messageReceiver=transform.parent;
}

function messageRedirectReceiver(methodName : String)
{
	//print("@@@@@@@methodName: "+methodName);
	messageReceiver.gameObject.SendMessage(methodName);
}