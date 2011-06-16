
var outerhalo:ParticleEmitter;
var halotrail:ParticleEmitter;

function Start () 
{
    while(true)
    {
        outerhalo.Emit();
	    yield WaitForSeconds(0.14);
	    halotrail.Emit();
    	
	    yield WaitForSeconds(1.8);
    
    }
	
}
