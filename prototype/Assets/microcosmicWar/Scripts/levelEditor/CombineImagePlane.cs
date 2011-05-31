using UnityEngine;

public class CombineImagePlane:MonoBehaviour
{
    [System.Serializable]
    public class ImageInfo
    {
        [zzSerialize]
        public int xCount
        {
            get { return _xCount; }
            set { _xCount = value; }
        }
        public int _xCount = 1;

        [zzSerialize]
        public RenderMaterialResourceInfo[] images
        {
            get { return _images; }
            set { _images = value; }
        }

        public RenderMaterialResourceInfo[] _images;


        [zzSerialize]
        public Rect rightBottomRect
        {
            get { return _rightBottomRect; }
            set { _rightBottomRect = value; }
        }
        public Rect _rightBottomRect;
    }

    [SerializeField]
    ImageInfo _imageInfo;

    [zzSerialize]
    public ImageInfo imageInfo
    {
        get { return _imageInfo; }
        set
        {
            _imageInfo = value;
            int lPlaneNum = _imageInfo.images.Length;
            int lXCount = _imageInfo.xCount;
            int lYCount = lPlaneNum / lXCount;
            if( lPlaneNum% lXCount >0)
            {
                Debug.LogError("imageInfo.images.Length % lXCount >0");
                return;
            }
            GameObject[] lPlanes = new GameObject[lPlaneNum];
            Texture2D[] lImages = new Texture2D[lPlaneNum];
            for (int i=0;i<lPlaneNum;++i)
            {
                var lPlane = (GameObject)Instantiate(planePrefab);
                lPlane.transform.parent = planeParent;
                lPlane.transform.localRotation = Quaternion.identity;
                var lImage = _imageInfo.images[i].resource.resource;
                lImage.wrapMode = TextureWrapMode.Clamp;
                lPlane.renderer.material.mainTexture = lImage;
                lPlanes[i] = lPlane;
                lImages[i]=lImage;
            }
            int lFirstPicWidth = lImages[0].width;
            int lFirstPicHeight = lImages[0].height;
            var lRightBottomRect = _imageInfo.rightBottomRect;
            int lRightBottomRectWidth = Mathf.RoundToInt(lRightBottomRect.width);
            int lRightBottomRectHeight = Mathf.RoundToInt(lRightBottomRect.height);
            int lXPixelNum = lFirstPicWidth * (lXCount - 1) + lRightBottomRectWidth;
            int lYPixelNum = lFirstPicHeight * (lYCount - 1) + lRightBottomRectHeight;
            //{
            //    for (int i = 0; i < lXCount; ++i)
            //    {
            //        lXPixelNum += lImages[i].width;
            //    }
            //    for (int i = lXCount - 1; i < lPlaneNum; i += lXCount)
            //    {
            //        lXPixelNum += lImages[i].height;
            //    }
            //}
            for (int y = 0; y < lYCount; ++y)
            {
                for (int x = 0; x < lXCount; ++x)
                {
                    int lPlaneIndex = x + lXCount * y;
                    int lWidth = (x == lXCount - 1) ? lRightBottomRectWidth : lFirstPicWidth;
                    int lHeight = (y == lYCount - 1) ? lRightBottomRectHeight : lFirstPicHeight;
                    var lPlaneObject = lPlanes[lPlaneIndex];
                    lPlaneObject.transform.localPosition
                        = new Vector3((x * lFirstPicWidth + (float)lWidth / 2f) / lXPixelNum - 0.5f,
                            -(y * lFirstPicHeight + (float)lHeight / 2f) / lYPixelNum + 0.5f, 0f);
                    lPlaneObject.transform.localScale
                        = new Vector3((float)lWidth / lXPixelNum,
                            (float)lHeight / lYPixelNum , 1f);
                    Vector2 lMaterailScale = Vector2.one;
                    if (x == lXCount - 1)
                        lMaterailScale.x = lRightBottomRect.width / (float)lImages[lPlaneIndex].width;
                    if (y == lYCount - 1)
                        lMaterailScale.y = lRightBottomRect.height /(float)lImages[lPlaneIndex].height; 
                    lPlaneObject.renderer.material.mainTextureScale = lMaterailScale;
                    print("x:" + x+" y:"+y);
                    print("lWidth:" + lWidth+" lHeight:"+lHeight);
                    print(new Vector3((x * lFirstPicWidth + (float)lWidth / 2f) / lXPixelNum - 0.5f,
                            (y * lFirstPicHeight + (float)lHeight / 2f) / lYPixelNum - 0.5f, 0f));
                    print(new Vector3((float)lWidth / lXPixelNum,
                            (float)lHeight / lYPixelNum,1f));
                    print(lMaterailScale);
                }
            }

            //for (int i = 0; i < lPlaneNum; ++i)
            //{
            //}

        }
    }

    public GameObject planePrefab;
    public Transform planeParent;

    //public float width = 1f;
    //public float heigth = 1f;
}