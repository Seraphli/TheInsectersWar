using UnityEngine;


public class zzSceneTextureNumber : MonoBehaviour
{
    //[0]代表个位,以此类推
    public Renderer[] numberRenderer;

    //索引数为相同的数字的贴图
    public Texture[] numberTexture;

    [SerializeField]
    int _number = 0;

    void Start()
    {
        setNum(_number);
    }

    public int number
    {
        get 
        { 
            return _number; 
        }

        set 
        { 
            _number = value;
            setNum(_number);
        }
    }

    void setNum(int pRendererIndex,int pNum)
    {
        numberRenderer[pRendererIndex].material.mainTexture = numberTexture[pNum];
    }

    void setNum(int pNum)
    {
        int i = 0;
        while(pNum!=0)
        {
            int lNum = pNum % 10;
            setNum(i, lNum);
            pNum /= 10;
            ++i;
        }
        for (; i < numberRenderer.Length;++i )
        {
            setNum(i, 0);
        }
    }

}