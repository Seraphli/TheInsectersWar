
function Start () {
	for(var i=0;i<100;i++){
	transform.Find("outerhalo").particleEmitter.Emit();
	yield WaitForSeconds(0.14);
	transform.Find("halotrail").particleEmitter.Emit();
	
	yield WaitForSeconds(1.8);
	}
	
}
