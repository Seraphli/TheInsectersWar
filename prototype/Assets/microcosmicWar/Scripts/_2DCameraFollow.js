// The target we are following
var target:Transform;
// Smooth switcher
var useSmooth = true;
// How much we 
var positionDamping = 0.74;

function Start()
{

}

function setTaget(pTarget:Transform)
{
	target = pTarget;
}

protected function tranF(OffsetValue:float, lastValue:float, nowValue:float)
{
	return (lastValue + OffsetValue /* Time.deltaTime*/ - nowValue) / positionDamping * Time.deltaTime;
}

protected function tranFX(OV:float)
{
	return tranF(OV,target.position.x,transform.position.x);
}

protected function tranFY(OV:float)
{
	return tranF(OV,target.position.y,transform.position.y);
}

protected function TarTranNotEqual()
{
	if (transform.position.x != target.position.x || transform.position.y != target.position.y)
	return 1;
	else
	return 0;
}

function Update () 
{    
	if ( target )
	{
		if ( useSmooth)
		{
			var translationx:float;
			var translationy:float;
			
				if (Input.GetButton ("left")) 
				{
					translationx  = tranFX(-10.0);
					if ( Input.GetButton ("up"))
					{
					translationy = tranFY(4.0);
					}
					else if (Input.GetButton ("down"))
					{
					translationy = tranFY(-4.0);
					}
					else
					translationy = tranFY(0.0);
					if ( TarTranNotEqual() )
					transform.Translate(translationx, translationy, 0);
				}
				else if ( Input.GetButton ("right"))
				{
					translationx  = tranFX(10.0);
					if ( Input.GetButton ("up"))
					{
					translationy = tranFY(4.0);
					}
					else if (Input.GetButton ("down"))
					{
					translationy = tranFY(-4.0);
					}
					else
					translationy = tranFY(0.0);
					if ( TarTranNotEqual() )
					transform.Translate(translationx, translationy, 0);
				}
				else if ( Input.GetButton ("up"))
				{
					translationx = tranFX(0.0);
					translationy = tranFY(4.0);
					if ( TarTranNotEqual() )
					transform.Translate(translationx, translationy, 0);
				}
				else if (Input.GetButton ("down"))
				{
					translationx = tranFX(0.0);
					translationy = tranFY(-4.0);
					if ( TarTranNotEqual() )
					transform.Translate(translationx, translationy, 0);
				}
			else
			{
				translationx = tranFX(0.0);
				translationy = tranFY(0.0);
				if ( TarTranNotEqual() )
				transform.Translate(translationx, translationy, 0);
			}
			//transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, timeDamping);
			//transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 1.74);
			//transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 1.74); 
			//transform.position = target.position;
		}
		else
		{
			transform.position.x = target.position.x;
			transform.position.y = target.position.y;
		}
	}
}