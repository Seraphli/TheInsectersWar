
static function nullFunction()
{
	
}

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