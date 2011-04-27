var risingSpeed=2.0;
var eigenLength=4.0;
var angAccel=5.0;
var duration=16.0;

var fire:GameObject;
var smoke:GameObject;
var whitecloud:GameObject;
var blastwave:GameObject;
var brightlight:GameObject;
private var timeOnCreation:float;
private var centerPosition:Vector3;
private var particles:Particle[];
function Awake()
{
	fire=transform.Find("fire").gameObject;
	smoke=transform.Find("smoke").gameObject;
	whitecloud=transform.Find("condensationcloud").gameObject;

	timeOnCreation=Time.time;
	centerPosition=Vector3.zero;//fire.transform.position;
	blastwave=transform.Find("blastwave").gameObject;
	blastwave.particleEmitter.Emit();
	brightlight=transform.Find("brightlight").gameObject;
}
function LateUpdate () {    
    particles = fire.particleEmitter.particles;	
	
	if(Time.time-timeOnCreation>duration*0.4) fire.particleEmitter.emit=false;

    for (var i = 0; i < particles.Length; i++) {
		var localPosition=particles[i].position-centerPosition;
		
		//Inner particles lift up, outer down
		if(Vector3(localPosition.x,0,localPosition.z).magnitude<eigenLength*0.2)
		
			localPosition.y+=2*Mathf.Atan(1-localPosition.y/(eigenLength*0.5))*angAccel*Time.deltaTime;
		else if(Vector3(localPosition.x,0,localPosition.z).magnitude>eigenLength*0.75)
		{
			localPosition.y-=1.2*angAccel*Time.deltaTime;
		}

		//Higher particles scatter, lower gather
		if(localPosition.y>eigenLength*0.25){
			var acc=Vector3(localPosition.x,0,localPosition.z).normalized
				*1.2*angAccel*Time.deltaTime;
			localPosition+=acc;
		}
		else if(localPosition.y<-eigenLength*0.25)
			localPosition-=
				Vector3(localPosition.x,0,localPosition.z).normalized
				*angAccel*Time.deltaTime;
		//update position with localposition
		
		particles[i].position=centerPosition+localPosition;
    }
	centerPosition.y+=risingSpeed*Time.deltaTime;
    // copy them back to the particle system
    fire.particleEmitter.particles = particles;
	
	//===================================================
	if(Time.time-timeOnCreation>duration*0.375) smoke.particleEmitter.emit=false;
	
	//===================================================
	if(Time.time-timeOnCreation>duration*0.5) transform.Find("firebottom").particleEmitter.emit=false;
	
	//===================================================
	if(Time.time-timeOnCreation>duration*0.07) whitecloud.particleEmitter.emit=true;
	if(Time.time-timeOnCreation>duration*0.425) whitecloud.particleEmitter.emit=false;
	//===================================================
	
	if(Time.time-timeOnCreation>duration*0.55)
		Destroy(brightlight);
	else
	{
		brightlight.light.intensity=1.0*Mathf.Sin(Mathf.PI/0.55*(Time.time-timeOnCreation)/duration);
		brightlight.GetComponent(LensFlare).brightness=1.5*Mathf.Sin(Mathf.PI/0.4*(Time.time-timeOnCreation)/duration);
	}
	if(Time.time-timeOnCreation>duration) {Destroy(this.gameObject);}
}