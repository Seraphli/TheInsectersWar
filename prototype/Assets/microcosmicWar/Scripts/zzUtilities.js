
static function nullFunction()
{
	
}

//移除Array中索引位置的元素,用末尾取代,节省移动数组的开支
static function quickRemoveArrayElement(array:Array,index:int)
{
	var t=array.Pop();
	//当索引不为最后一位时,现在的array.length为原最后索引值
	if(index!=array.length)
		array[index]=t;
}