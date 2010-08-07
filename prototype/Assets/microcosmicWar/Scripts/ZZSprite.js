
class AnimationInfo
{
	var material:Material;
	var animationName:String="unnamed";
	var animationLength:float=2.0;

	//��ʼ��֡��,������֡��,����0��ʼ.���Ქ�Ŵ�beginPicNum��endPicNum,��endPicNum-beginPicNum+1��ͼ
	var beginPicNum=0;
	var endPicNum=0;

	//�����е���ͼ��������Ϣ
	var pixelVerticalNum:int;
	var pixelHorizonNum:int;
	
	var loop=true;
}

class AnimationData
{
	
	var material:Material;
	var animationName:String;
	var animationLength:float;


	//��ʼ��֡��,������֡��,����0��ʼ.���Ქ�Ŵ�beginPicNum��endPicNum,��endPicNum-beginPicNum+1��ͼ
	var beginPicNum:int;
	var endPicNum:int;

	var numOfHorizonPic=0;
	var numOfVerticalPic=0;

	//ÿ֡ͼ��UV��ռ�ı���
	var picRateInU=1.0;
	var picRateInV=1.0;

	//������ͼ�а����� ֡��
	var numOfPic=0;

	//var timeInOnePic=0.0;

	//�����е���ͼ��������Ϣ
	var pixelVerticalNum:int;
	var pixelHorizonNum:int;
	var loop=true;
}

class AnimationListener
{
	virtual  function updateCallback(timePos:float)
	{
		//Debug.Log("AnimationListener");
	}
	
	virtual function beginTheAnimationCallback(){}
	
	//������β,��updateCallbackǰִ��
	//virtual  function overEndBeforeUpdateCallback(){}
	
	//������β,����updateCallback��ִ��
	//virtual  function overEndAfterUpdateCallback(){}
	
	//��������(����רΪ��������ʱ)
	//virtual  function endTheAnimationCallback(){}
	
	//����Խ����ͷ,����ѭ������
	virtual function overBeginCall()
	{
	}
}

var defaultAnimation="";

//�����е���ͼ��Ĭ��������Ϣ
var defaultPixelVerticalNum:int=256;
var defaultPixelHorizonNum:int=256;

//�����ڳ����еĴ�С
var spriteWidth=1.0;
var spriteHeight=1.0;

var animationInfos:AnimationInfo[];

//var animationInfo:AnimationInfo;
var animationDatas=Hashtable();
var nowAnimationData=AnimationData();

var playTime:float;

protected var nowListener=AnimationListener();

var animationListeners=Hashtable();

//playAnimation�����Ч
function setListener(animationName:String,listener:AnimationListener)
{
	animationListeners[animationName]=listener;
}

//���ڲ��ŵ���֡,��0��ʼ
protected var nowPicNum=0;

//protected:

// The vertices of mesh
// 3--2
// |  |
// 0--1
protected var vertices:Vector3[];

 // Indices into the vertex array
protected var triIndices:int[]=[0,2,1,3,2,0];   

 // UV coordinates
protected var UVs:Vector2[]=[Vector2(0,0),Vector2(1,0),Vector2(1,1),Vector2(0,1)];

protected var meshFilter:MeshFilter;
protected var meshRenderer:MeshRenderer;
protected var mesh:Mesh;   // Reference to our mesh (contained in the MeshFilter)


protected function CreateDataFromInfo(info:AnimationInfo)
{
	var lOut=AnimationData();
	
	lOut.material=info.material;
	lOut.animationName=info.animationName;
	lOut.animationLength=info.animationLength;

	//�����е���ͼ��������Ϣ
	if(info.pixelVerticalNum)
		lOut.pixelVerticalNum=info.pixelVerticalNum;
	else//use default
		lOut.pixelVerticalNum=defaultPixelVerticalNum;
		
	if(info.pixelHorizonNum)
		lOut.pixelHorizonNum=info.pixelHorizonNum;
	else
		lOut.pixelHorizonNum=defaultPixelHorizonNum;

	//��ʼ��֡��,������֡��,����0��ʼ.���Ქ�Ŵ�beginPicNum��endPicNum,��endPicNum-beginPicNum+1��ͼ
	lOut.beginPicNum=info.beginPicNum;
	lOut.endPicNum=info.endPicNum;
	
	lOut.loop=info.loop;
	
	/////////////////////////////////////////////////////////
	
	lOut.numOfHorizonPic = info.material.mainTexture.width/lOut.pixelHorizonNum;
	lOut.numOfVerticalPic = info.material.mainTexture.height/lOut.pixelVerticalNum;
	
	lOut.picRateInU = 1.0/lOut.numOfHorizonPic;
	lOut.picRateInV = 1.0/lOut.numOfVerticalPic;
	//numOfPic = numOfVerticaPic * numOfHorizonPic;
	lOut.numOfPic=lOut.endPicNum-lOut.beginPicNum+1;
	//lOut.timeInOnePic = lOut.animationLength / lOut.numOfPic;
	
	return lOut;
}

//��ʼ��,��ָ��Ĭ�϶���
protected function InitShow()
{
	
	vertices=[Vector3(0,0,0),Vector3(spriteWidth,0,0),Vector3(spriteWidth,spriteHeight,0),Vector3(0,spriteHeight,0)];
	// iterate through the array
	for (var value in animationInfos)
	{
		//print(animationInfo);
		animationDatas[value.animationName]=CreateDataFromInfo(value);
	}
	//nowAnimationData = animationDatas[defaultAnimation];
	//print(nowAnimationData.animationName);
	
	meshFilter = GetComponent(MeshFilter);
	meshRenderer = GetComponent(MeshRenderer);
	//mesh = meshFilter.mesh;
	//mesh = meshFilter.sharedMesh;
	meshFilter.mesh=Mesh ();
	mesh = meshFilter.mesh ;
	
	//meshRenderer.material=nowAnimationData.material;
	mesh.vertices = vertices;
	mesh.uv = UVs;
	//mesh.normals = normals;
	mesh.triangles = triIndices;
	mesh.normals = [Vector3(0,0,-1),Vector3(0,0,-1),Vector3(0,0,-1),Vector3(0,0,-1)];
	
	playAnimation(defaultAnimation);
	
	/*
	numOfVerticalPic = material.mainTexture.width/pixelVerticalNum;
	numOfHorizonPic = material.mainTexture.height/pixelHorizonNum;
	
	nowPicNum = beginPicNum;
	
	picRateInU = 1.0/numOfHorizonPic;
	picRateInV = 1.0/numOfVerticalPic;
	//numOfPic = numOfVerticaPic * numOfHorizonPic;
	numOfPic=endPicNum-beginPicNum+1;
	timeInOnePic = AnimationLength / numOfPic;
	*/
}

protected function updateShow()
{
	//var lTextureHeight = material.mainTexture.height;
	//var lTextureWidth = material.mainTexture.width;
	
	var lLine = nowPicNum / nowAnimationData.numOfHorizonPic;
	var lColumn = nowPicNum - lLine*nowAnimationData.numOfHorizonPic;
	
	
	UVs[3].x=lColumn*nowAnimationData.picRateInU;
	UVs[3].y=1.0-nowAnimationData.picRateInV*lLine;
	
	UVs[0].x=UVs[3].x;
	UVs[0].y=UVs[3].y-nowAnimationData.picRateInV;
	
	
	UVs[2].x=UVs[3].x+nowAnimationData.picRateInU;
	UVs[2].y=UVs[3].y;
	
	UVs[1].x=UVs[2].x;
	UVs[1].y=UVs[0].y;
	
	mesh.uv = UVs;
}

//function UpdateUV()
//{
//	mesh.uv = UVs;
//}

function Start()
{
	//��ʼ��,��ʹ��Ĭ�϶���
	InitShow();
	
	//updateShow();
}

function Update ()  
{
	playTime += Time.deltaTime;
	//var lOverEnd = playTime>nowAnimationData.animationLength;
	//if(lOverEnd)
	//	nowListener.overEndBeforeUpdateCallback();
		
	if(nowAnimationData.loop && playTime>nowAnimationData.animationLength)
	{
		nowListener.overBeginCall();
		playTime = playTime % nowAnimationData.animationLength;
	}
	//var lPicNum:int = Mathf.FloorToInt(playTime/nowAnimationData.timeInOnePic);
	var lPicNum:int = Mathf.FloorToInt(playTime/( nowAnimationData.animationLength / nowAnimationData.numOfPic ) );
	lPicNum+=nowAnimationData.beginPicNum;
	
	//ʹ����֡���ᳬ����Χ
	if(lPicNum>=nowAnimationData.endPicNum)
		lPicNum=nowAnimationData.endPicNum;
		
	if( lPicNum != nowPicNum)
	{
		nowPicNum = lPicNum;
		updateShow();
	}
	nowListener.updateCallback(playTime);
	
	//if(lOverEnd)
	//	nowListener.overEndAfterUpdateCallback();
	//var lTimePos = 
}

function playAnimation(animationName:String)
{
	//print(animationName);
	if(nowAnimationData.animationName==animationName)
		return;
	//nowListener.endTheAnimationCallback();
	nowAnimationData = animationDatas[animationName];
	nowListener= animationListeners[animationName];
	if(!nowListener)
		nowListener=AnimationListener();
	nowListener.beginTheAnimationCallback();
	meshRenderer.material=nowAnimationData.material;
	playTime=0;
	updateShow();
}

//function OnInspectorGUI ()
//{
//	InitShow();
//}

@script RequireComponent(MeshRenderer,MeshFilter)