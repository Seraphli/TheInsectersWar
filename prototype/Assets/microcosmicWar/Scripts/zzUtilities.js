
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