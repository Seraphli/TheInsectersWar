

class zzIndexTable
{
	protected var dataList=Array();
	
	protected var nameToIndex=Hashtable();
	
	//return index
	function addData(name:String,data):int
	{
		dataList.Add(data);
		var lIndex = dataList.Count-1;
		nameToIndex[name]=lIndex;
		return lIndex;
	}
	
	function getIndex(name:String):int
	{
		return nameToIndex[name];
	}
	
	function getData(pIndex:int)
	{
		return dataList[pIndex];
	}
	
	function getNum()
	{
		return dataList.Count;
	}
	
	function ToString () : String 
	{
		return dataList.ToString();
	}
}