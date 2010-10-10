using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationInfo
{
    public Material material;
    public string animationName = "unnamed";
    public float animationLength = 2.0f;

    //开始的帧数,结束的帧数,数从0开始.将会播放从beginPicNum到endPicNum,共endPicNum-beginPicNum+1幅图
    public int beginPicNum = 0;
    public int endPicNum = 0;

    //动画中单幅图的像素信息
    //int pixelVerticalNum;
    //int pixelHorizonNum;

    public bool loop = true;
}

[System.Serializable]
public class AnimationData
{

    public Material material;
    public string animationName;
    public float animationLength;


    //开始的帧数,结束的帧数,数从0开始.将会播放从beginPicNum到endPicNum,共endPicNum-beginPicNum+1幅图
    public int beginPicNum;
    public int endPicNum;

    public int numOfHorizonPic = 0;
    public int numOfVerticalPic = 0;

    //每帧图在UV中占的比例
    public float picRateInU = 1.0f;
    public float picRateInV = 1.0f;

    //纹理贴图中包含的 帧数
    public int numOfPic = 0;

    //FIXME_VAR_TYPE timeInOnePic=0.0f;

    //动画中单幅图的像素信息
    public int pixelVerticalNum;
    public int pixelHorizonNum;
    public bool loop = true;
}

public class AnimationListener
{
    public virtual void updateCallback(float timePos)
    {
        //Debug.Log("AnimationListener");
    }

    public virtual void beginTheAnimationCallback() { }

    //动画过尾,在updateCallback前执行
    //virtual  void  overEndBeforeUpdateCallback (){}

    //动画过尾,在在updateCallback后执行
    //virtual  void  overEndAfterUpdateCallback (){}

    //动画结束(比如专为其他动画时)
    //virtual  void  endTheAnimationCallback (){}

    //动画越过开头,用于循环动画
    public virtual void overBeginCall()
    {
    }
}

public class ZZSprite : MonoBehaviour
{

    public string defaultAnimation = "";

    //动画中单幅图的默认像素信息
    public int defaultPixelVerticalNum = 256;
    public int defaultPixelHorizonNum = 256;

    //精灵在场景中的大小
    public float spriteWidth = 1.0f;
    public float spriteHeight = 1.0f;

    public AnimationInfo[] animationInfos;

    //AnimationInfo animationInfo;
    public Hashtable animationDatas = new Hashtable();
    public AnimationData nowAnimationData = new AnimationData();

    public float playTime;

    protected AnimationListener nowListener = new AnimationListener();

    public Hashtable animationListeners = new Hashtable();

    //playAnimation后才奏效
    public void setListener(string animationName, AnimationListener listener)
    {
        animationListeners[animationName] = listener;
    }

    //现在播放到的帧,从0开始
    protected int nowPicNum = 0;

    //protected:

    // The vertices of mesh
    // 3--2
    // |  |
    // 0--1
    protected Vector3[] vertices;

    // Indices into the vertex array
    protected int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

    // UV coordinates
    protected Vector2[] UVs = new Vector2[] { 
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(1, 1), new Vector2(0, 1) 
    };

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected Mesh mesh;   // Reference to our mesh (contained in the MeshFilter)


    protected AnimationData CreateDataFromInfo(AnimationInfo info)
    {
        AnimationData lOut = new AnimationData();

        lOut.material = info.material;
        lOut.animationName = info.animationName;
        lOut.animationLength = info.animationLength;

        //动画中单幅图的像素信息
        //if(info.pixelVerticalNum)
        //	lOut.pixelVerticalNum=info.pixelVerticalNum;
        //else//use default
        lOut.pixelVerticalNum = defaultPixelVerticalNum;

        //if(info.pixelHorizonNum)
        //	lOut.pixelHorizonNum=info.pixelHorizonNum;
        //else
        lOut.pixelHorizonNum = defaultPixelHorizonNum;

        //开始的帧数,结束的帧数,数从0开始.将会播放从beginPicNum到endPicNum,共endPicNum-beginPicNum+1幅图
        lOut.beginPicNum = info.beginPicNum;
        lOut.endPicNum = info.endPicNum;

        lOut.loop = info.loop;

        /////////////////////////////////////////////////////////

        lOut.numOfHorizonPic = info.material.mainTexture.width / lOut.pixelHorizonNum;
        lOut.numOfVerticalPic = info.material.mainTexture.height / lOut.pixelVerticalNum;

        lOut.picRateInU = 1.0f / lOut.numOfHorizonPic;
        lOut.picRateInV = 1.0f / lOut.numOfVerticalPic;
        //numOfPic = numOfVerticaPic * numOfHorizonPic;
        lOut.numOfPic = lOut.endPicNum - lOut.beginPicNum + 1;
        //lOut.timeInOnePic = lOut.animationLength / lOut.numOfPic;

        return lOut;
    }

    //初始化,并指定默认动画
    protected void InitShow()
    {

        vertices = new Vector3[]{
                new Vector3(-spriteWidth/2,-spriteWidth/2,0),
                new Vector3(spriteWidth/2,-spriteWidth/2,0),
                new Vector3(spriteWidth/2,spriteWidth/2,0),
                new Vector3(-spriteWidth/2,spriteWidth/2,0)
        };
        // iterate through the array
        foreach (AnimationInfo value in animationInfos)
        {
            //print(animationInfo);
            animationDatas[value.animationName] = CreateDataFromInfo(value);
        }
        //nowAnimationData = animationDatas[defaultAnimation];
        //print(nowAnimationData.animationName);

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        //mesh = meshFilter.mesh;
        //mesh = meshFilter.sharedMesh;
        meshFilter.mesh = new Mesh();
        mesh = meshFilter.mesh;

        //meshRenderer.material=nowAnimationData.material;
        mesh.vertices = vertices;
        mesh.uv = UVs;
        //mesh.normals = normals;
        mesh.triangles = triIndices;
        mesh.normals = new Vector3[]{
        new Vector3(0,0,-1),new Vector3(0,0,-1),
        new Vector3(0,0,-1),new Vector3(0,0,-1)
    };

        playAnimation(defaultAnimation);

        /*
        numOfVerticalPic = material.mainTexture.width/pixelVerticalNum;
        numOfHorizonPic = material.mainTexture.height/pixelHorizonNum;
	
        nowPicNum = beginPicNum;
	
        picRateInU = 1.0f/numOfHorizonPic;
        picRateInV = 1.0f/numOfVerticalPic;
        //numOfPic = numOfVerticaPic * numOfHorizonPic;
        numOfPic=endPicNum-beginPicNum+1;
        timeInOnePic = AnimationLength / numOfPic;
        */
    }

    protected void updateShow()
    {
        //FIXME_VAR_TYPE lTextureHeight= material.mainTexture.height;
        //FIXME_VAR_TYPE lTextureWidth= material.mainTexture.width;

        int lLine = nowPicNum / nowAnimationData.numOfHorizonPic;
        int lColumn = nowPicNum - lLine * nowAnimationData.numOfHorizonPic;


        UVs[3].x = lColumn * nowAnimationData.picRateInU;
        UVs[3].y = 1.0f - nowAnimationData.picRateInV * lLine;

        UVs[0].x = UVs[3].x;
        UVs[0].y = UVs[3].y - nowAnimationData.picRateInV;


        UVs[2].x = UVs[3].x + nowAnimationData.picRateInU;
        UVs[2].y = UVs[3].y;

        UVs[1].x = UVs[2].x;
        UVs[1].y = UVs[0].y;

        mesh.uv = UVs;
    }

    //function UpdateUV()
    //{
    //	mesh.uv = UVs;
    //}

    void Start()
    {
        //初始化,并使用默认动画
        InitShow();

        //updateShow();
    }

    void Update()
    {
        playTime += Time.deltaTime;
        //FIXME_VAR_TYPE lOverEnd= playTime>nowAnimationData.animationLength;
        //if(lOverEnd)
        //	nowListener.overEndBeforeUpdateCallback();

        if (nowAnimationData.loop && playTime > nowAnimationData.animationLength)
        {
            nowListener.overBeginCall();
            playTime = playTime % nowAnimationData.animationLength;
        }
        //int lPicNum = Mathf.FloorToInt(playTime/nowAnimationData.timeInOnePic);
        int lPicNum = Mathf.FloorToInt(playTime / (nowAnimationData.animationLength / nowAnimationData.numOfPic));
        lPicNum += nowAnimationData.beginPicNum;

        //使动画帧不会超出范围
        if (lPicNum >= nowAnimationData.endPicNum)
            lPicNum = nowAnimationData.endPicNum;

        if (lPicNum != nowPicNum)
        {
            nowPicNum = lPicNum;
            updateShow();
        }
        nowListener.updateCallback(playTime);

        //if(lOverEnd)
        //	nowListener.overEndAfterUpdateCallback();
        //FIXME_VAR_TYPE lTimePos= 
    }

    public void playAnimation(string animationName)
    {
        //print(animationName);
        if (nowAnimationData.animationName == animationName)
            return;
        //nowListener.endTheAnimationCallback();
        nowAnimationData = animationDatas[animationName] as AnimationData;
        nowListener = animationListeners[animationName] as AnimationListener;
        if (nowListener==null)
            nowListener = new AnimationListener();
        nowListener.beginTheAnimationCallback();
        meshRenderer.material = nowAnimationData.material;
        playTime = 0;
        updateShow();
    }

}
