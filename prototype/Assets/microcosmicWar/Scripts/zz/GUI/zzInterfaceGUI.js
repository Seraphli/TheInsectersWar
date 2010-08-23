
//@RequireComponent(zzGUI)

class zzInterfaceGUI extends MonoBehaviour
{
	//var data:int;
	var position : Rect;
	var depth:int;
	
	virtual function impGUI()
	{
	
	}
	
	function OnDrawGizmosSelected () 
	{
		DrawGizmos(Color.white);
	}
	
	function OnDrawGizmos () 
	{
		//Gizmos.color = Color.blue;
		DrawGizmos(Color.blue);
	}
	
	function DrawGizmos(pColor:Color)
	{
		Gizmos.color = pColor;
		
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax,-position.y,0));
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax,0));
		
		
		Gizmos.DrawLine (Vector3(position.xMax,-position.y,0), Vector3(position.xMax,-position.yMax,0));
		Gizmos.DrawLine (Vector3(position.x,-position.yMax,0), Vector3(position.xMax,-position.yMax,0));
		/*
			
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax-position.width/2,-position.y,0));
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax+position.height/2,0));
		
		
		Gizmos.DrawLine (Vector3(position.xMax-position.width/2,-position.y,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		Gizmos.DrawLine (Vector3(position.x,-position.yMax+position.height/2,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		*/
	}
}