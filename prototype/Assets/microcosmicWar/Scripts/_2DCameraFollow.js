// The target we are following
var target:Transform;
// Smooth switcher
var useSmooth = true;
// How much we 
var positionDamping = 1.0;

function Start()
{
	
}

function setTaget(pTarget:Transform)
{
	target = pTarget;
}


function Update () 
{    
	if ( target )
	{
		if ( useSmooth)
		{
			translationx = (target.position.x - transform.position.x) / positionDamping * Time.deltaTime;
			translationy = (target.position.y - transform.position.y) / positionDamping * Time.deltaTime;
			if ( transform.position.x != target.position.x && transform.position.y != target.position.y )
			transform.Translate = (translationx, translationy,o);
			//transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, timeDamping);
			//transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 1.75);
			//transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 1.75); 
			//transform.position = target.position;
		}
		else
		{
			transform.position.x = target.position.x;
			transform.position.y = target.position.y;
		}
	}
}