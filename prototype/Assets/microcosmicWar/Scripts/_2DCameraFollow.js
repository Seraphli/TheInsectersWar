// The target we are following
var target:Transform;
// Smooth switcher
var useSmooth = true;
// How much we 
var positionDamping = 0.75;

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
				if (Input.GetKey (KeyCode.A)) 
				{
					translationx = (target.position.x - 10.0 - transform.position.x) / positionDamping * Time.deltaTime;
					translationy = (target.position.y - transform.position.y) / positionDamping * Time.deltaTime;
					if ( transform.position.x != target.position.x && transform.position.y != target.position.y )
					transform.Translate(translationx, translationy, 0);
				}
				else if ( Input.GetKey (KeyCode.D))
				{
					translationx = (target.position.x + 10.0 - transform.position.x) / positionDamping * Time.deltaTime;
					translationy = (target.position.y - transform.position.y) / positionDamping * Time.deltaTime;
					if ( transform.position.x != target.position.x && transform.position.y != target.position.y )
					transform.Translate(translationx, translationy, 0);
				}
			else
			{
			translationx = (target.position.x - transform.position.x) / positionDamping * Time.deltaTime;
			translationy = (target.position.y - transform.position.y) / positionDamping * Time.deltaTime;
			if ( transform.position.x != target.position.x && transform.position.y != target.position.y )
			transform.Translate(translationx, translationy, 0);
			}
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