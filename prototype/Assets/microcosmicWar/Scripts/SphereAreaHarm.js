
//���ڱ�ը���ƻ�(ȥѪЧ��)

var oneTime=true;
var harmRadius=5.0;
var harmValueInCentre=10.0;

//[Life]=Distance  �������ڴ洢�������������,�뱬ը����������
protected var injuredLifeInTheFrame=Hashtable();

function Start () 
{
}

function Update()
{
	var lColliderList: Collider[] = Physics.OverlapSphere(transform.position,harmRadius);
	//��Ѱ��Χ�ڵ�Life,��Ѱ����̾���
	for(var i:Collider in  lColliderList)
	{
		var lLife:Life = Life.getLifeFromTransform(i.transform);
		if(lLife)
		{
			//lLife.injure(harmValueInCentre);
			var lClosestPoint = i.ClosestPointOnBounds(transform.position);
			var lDistance = Vector3.Distance(lClosestPoint, transform.position);
			
			//��Ϊһ����Life��������,�����ж��Collider
			if(injuredLifeInTheFrame.ContainsKey(lLife))
			{
				if(injuredLifeInTheFrame[lLife]>lDistance)
				{
					//print(""+injuredLifeInTheFrame[lLife]+">"+lDistance);
					injuredLifeInTheFrame[lLife] = lDistance;
				}
			}
			else
				injuredLifeInTheFrame[lLife] = lDistance;
		}
		
	}
	
	//ִ���˺�
	for(var lifeToDistance:System.Collections.DictionaryEntry in injuredLifeInTheFrame)
	{
		// The hit points we apply fall decrease with distance from the explosion point
		var lLifeImp:Life = lifeToDistance.Key;
		var lDistanceImp:float = lifeToDistance.Value;
		var lHarmRange:float = 1.0 - Mathf.Clamp01(lDistanceImp / harmRadius);
		//print(lDistanceImp);
		//print(lHarmRange);
		//print(lHarmRange *harmValueInCentre );
		lLifeImp.injure( lHarmRange *harmValueInCentre );
	}
	
	injuredLifeInTheFrame.Clear();
	if(oneTime)
	{
		Destroy(this);
	}

}


function OnDrawGizmosSelected() 
{
	Gizmos.DrawWireSphere(transform.position,harmRadius);
}