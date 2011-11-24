using UnityEngine;


public class zzSceneTextureNumber : MonoBehaviour
{
    //[0]代表个位,以此类推
    public Renderer[] numberRenderer;

    //索引数为相同的数字的贴图
    public Texture[] numberTexture;

    [SerializeField]
    int _number = 0;

    public bool left = false;

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

    void showNum(int pRendererIndex,bool pShow)
    {
        numberRenderer[pRendererIndex].enabled = pShow;
    }

    void setNum(int pNum)
    {
        if (left)
        {
            //int i = numberRenderer.Length - 1;
            int lLength = (int)Mathf.Log10(pNum) + 1;
            int lShowIndex = numberRenderer.Length - lLength;
            while (pNum != 0)
            {
                int lNum = pNum % 10;
                showNum(lShowIndex, true);
                setNum(lShowIndex, lNum);
                pNum /= 10;
                //--i;
                ++lShowIndex;
            }
            int i = numberRenderer.Length - lLength - 1;
            for (; i >= 0; --i)
            {
                showNum(i, false);
            }

        }
        else
        {
            int i = 0;
            while (pNum != 0)
            {
                int lNum = pNum % 10;
                setNum(i, lNum);
                pNum /= 10;
                ++i;
            }
            for (; i < numberRenderer.Length; ++i)
            {
                setNum(i, 0);
            }
        }
    }

}