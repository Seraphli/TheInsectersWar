
static function nullFunction()
{
	
}

static function needComponent(gameObject:GameObject,componentType :System.Type) : Component
{
	var lOut = gameObject.GetComponent(componentType);
	if(!lOut  )
	{
		lOut = gameObject.AddComponent(componentType);
	}
	return lOut;
}
/*
static function reAddComponent(gameObject:GameObject,componentType :System.Type) : Component
{
	if(gameObject.GetComponent(componentType))
		gameObject.Destroy(gameObject.GetComponent(componentType));
		
	return gameObject.AddComponent(componentType);
}
*/
//�Ƴ�Array������λ�õ�Ԫ��,��ĩβȡ��,��ʡ�ƶ�����Ŀ�֧
static function quickRemoveArrayElement(array:Array,index:int)
{
	var t=array.Pop();
	//��������Ϊ���һλʱ,���ڵ�array.lengthΪԭ�������ֵ
	if(index!=array.length)
		array[index]=t;
}

//ɾ����һ����Array���ֵ�pValue
static function removeValueInArray(array:Array, pValue)
{
		for( var i=0;i<array.length;++i)
		{
			if(array[i]== pValue)
			{
				array.RemoveAt(i);
				break;
			}
		}
}

static function normalize(pValue:float):float
{
	if(pValue>0)
		return 1;
	else if(pValue<0)
		return -1;
	return 0;
}

class DataWrap
{
	var data:Object;
	
	function ToString () : String 
	{
		return data.ToString();
	}
}

class zzPair
{
	function ToString () : String 
	{
		return "left:"+left+" right:"+right;
	}
	
	function zzPair(){}
	function zzPair(Lift,Right)
	{
		left = Lift;
		right =Right;
	}
	
	var left:Object;
	var right:Object;
}