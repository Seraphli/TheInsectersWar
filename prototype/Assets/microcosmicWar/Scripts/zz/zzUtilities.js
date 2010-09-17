
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
//移除Array中索引位置的元素,用末尾取代,节省移动数组的开支
static function quickRemoveArrayElement(array:Array,index:int)
{
	var t=array.Pop();
	//当索引不为最后一位时,现在的array.length为原最后索引值
	if(index!=array.length)
		array[index]=t;
}

//删除第一个在Array出现的pValue
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