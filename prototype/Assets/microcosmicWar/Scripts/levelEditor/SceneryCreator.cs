using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class SceneryCreator:MonoBehaviour
{

    static void BitmapToTexture2D(System.Drawing.Bitmap pBitmap,
        Rectangle pRect,Texture2D pTexture)
    {
        print("BitmapToTexture2D begin:" + pRect +" "+ System.DateTime.Now);
        for (int y = 0; y < pRect.Height; ++y)
        {
            int lBitmapY = pRect.Y+y;
            for (int x = 0; x < pRect.Width; ++x)
            {
                var lColor = pBitmap.GetPixel(pRect.X + x, lBitmapY);
                pTexture.SetPixel(x, y, new UnityEngine.Color(lColor.R, lColor.G, lColor.B, lColor.A));
            }
        }
        pTexture.Apply();
        print("BitmapToTexture2D end:" + System.DateTime.Now);
    }

    static void fillPowerOfTwo(Texture2D pImage)
    {
        int lPreWidth = pImage.width;
        int lPreHeight = pImage.height;
        if (Mathf.IsPowerOfTwo(lPreWidth) && Mathf.IsPowerOfTwo(lPreHeight))
        {
            print("don't need fillPowerOfTwo");
            return;
        }
        var lPixels = pImage.GetPixels();
        pImage.Resize(Mathf.NextPowerOfTwo(lPreWidth), Mathf.NextPowerOfTwo(lPreHeight));
        pImage.SetPixels(0, 0, lPreWidth, lPreHeight, lPixels);
        //var lNewPixels = new UnityEngine.Color[pImage.width * pImage.height];
        //int lSrcY = 0;
        //for (int y = pImage.height - lPreHeight; y < pImage.height; ++y)
        //{
        //    int lSrc = lSrcY * lPreWidth - 1;
        //    int lSrcEnd = lSrc + lPreWidth;
        //    int lDest = y * pImage.width - 1;
        //    while (lSrc < lSrcEnd)
        //    {
        //        lNewPixels[++lDest] = lPixels[++lSrc];
        //    }
        //    ++lSrcY;
        //}
        //pImage.SetPixels(lNewPixels);
        pImage.Apply();
    }

    static void fillPowerOfTwo(Texture2D pImage, UnityEngine.Color fillColor)
    {
        int lPreWidth = pImage.width;
        int lPreHeight = pImage.height;
        if (Mathf.IsPowerOfTwo(lPreWidth) && Mathf.IsPowerOfTwo(lPreHeight))
        {
            print("don't need fillPowerOfTwo");
            return;
        }
        var lPixels = pImage.GetPixels();
        pImage.Resize(Mathf.NextPowerOfTwo(lPreWidth), Mathf.NextPowerOfTwo(lPreHeight));
        //var lNewPixels = new UnityEngine.Color[pImage.width * pImage.height];
        //int lSrcY = 0;
        //for (int y = pImage.height - lPreHeight; y < pImage.height; ++y)
        //{
        //    int lSrc = lSrcY * lPreWidth - 1;
        //    int lSrcEnd = lSrc + lPreWidth;
        //    int lDest = y * pImage.width -1 ;
        //    while (lSrc < lSrcEnd)
        //    {
        //        lNewPixels[++lDest] = lPixels[++lSrc];
        //    }
        //    ++lSrcY;
        //}
        //pImage.SetPixels(lNewPixels);
        int i = -1;
        for (int y = lPreHeight; y > 0; --y)
        {
            for (int x = 0; x < lPreWidth; ++x)
            {
                pImage.SetPixel(x, y, lPixels[++i]);
            }
            for (int x = lPreWidth; x < pImage.width; ++x)
            {
                pImage.SetPixel(x, y, fillColor);
            }
        }
        for (int y = pImage.height; y < lPreHeight; ++y)
        {
            for (int x = 0; x < pImage.width; ++x)
            {
                pImage.SetPixel(x, y, fillColor);
            }
        }
        pImage.Apply();
    }

    static void BitmapToTexture2D(System.Drawing.Bitmap pBitmap,
        Texture2D pTexture)
    {
        print("BitmapToTexture2D begin:" + System.DateTime.Now);
        for (int y = 0; y < pBitmap.Height; ++y)
        {
            for (int x = 0; x < pBitmap.Width; ++x)
            {
                var lColor = pBitmap.GetPixel(x, y);
                pTexture.SetPixel(x, y, new UnityEngine.Color(lColor.R, lColor.G, lColor.B, lColor.A));
            }
        }
        //int lPixelCount = pBitmap.Height * pBitmap.Width;
        //var lPixels = new Color[lPixelCount];
        //var lBitmapData = pBitmap.LockBits(
        //    new Rectangle(0, 0, pBitmap.Width, pBitmap.Height),
        //    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //lBitmapData.Scan0
 
        //int lBufferSize = pBitmap.Height * pBitmap.Width * 4;

        // Create new memory stream and save image to stream so   
        // we don't have to save and read file  
        //using (System.IO.MemoryStream lMemoryStream =
        //    new System.IO.MemoryStream(lBufferSize))
        //{
        //    pBitmap.Save(lMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

        //    pTexture.LoadImage(lMemoryStream.GetBuffer());
        //}
        //pTexture.SetPixels(lPixels);
        pTexture.Apply();
        //pBitmap.UnlockBits(lBitmapData);
        print("BitmapToTexture2D end:" + System.DateTime.Now);
    } 

    const int maxSize = 4096;

    static Rect decomposeImage(Image pImage, PixelFormat pPixelFormat,
        out Texture2D[] lOut, out int lXCount)
    {
        lXCount = Mathf.CeilToInt((float)pImage.Width / (float)maxSize);
        int lYCount = Mathf.CeilToInt((float)pImage.Height / (float)maxSize);
        lOut = new Texture2D[lXCount * lYCount];
        //Bitmap lBitmap = new Bitmap(pImage);
        print("lXCount:" + lXCount);
        print("lYCount:" + lYCount);
        Rect lRightBottomRect = new Rect((lXCount - 1) * maxSize, (lYCount - 1) * maxSize,
            pImage.Width % maxSize, pImage.Height % maxSize);
        System.IO.MemoryStream lMemoryStream = new MemoryStream(maxSize * maxSize*4 +1024*1024);
        for (int y = 0; y < lYCount; ++y)
        {
            for (int x=0;x<lXCount;++x)
            {
                int lWidth = (x==lXCount-1) ?(pImage.Width%maxSize):maxSize;
                int lHeight = (y == lYCount - 1) ? (pImage.Height % maxSize) : maxSize;
                //using (Bitmap lBitmap = new Bitmap(lWidth, lHeight,PixelFormat.Format32bppArgb))
                //{
                print("count:" + (x + lXCount * y) + " x:" + x + " y:" + y);
                Bitmap lBitmap = new Bitmap(lWidth, lHeight, pPixelFormat);
                    //print("start:"+System.DateTime.Now);
                var lGraphics = System.Drawing.Graphics.FromImage(lBitmap);
                var lDestRect = new Rectangle(0, 0, lWidth, lHeight);
                var lSrcRect = new Rectangle(x * maxSize, y * maxSize, lWidth, lHeight);

                print("lRect:" + lSrcRect + " " + System.DateTime.Now);
                lGraphics.DrawImage(pImage, lDestRect, lSrcRect, GraphicsUnit.Pixel);
                lMemoryStream.SetLength(0);
                lMemoryStream.Position = 0;
                print("After DrawImageUnscaled:" + System.DateTime.Now);
                lBitmap.Save(lMemoryStream, ImageFormat.Png);
                print("After Bitmap.Save:" + System.DateTime.Now);
                //print("after Graphics.DrawImageUnscaled:" + System.DateTime.Now);
                Texture2D lTexture = new Texture2D(lWidth, lHeight, TextureFormat.ARGB32, false);
                //BitmapToTexture2D(lBitmap, lRect,lTexture);
                lTexture.LoadImage(lMemoryStream.GetBuffer());
                print("After Texture.LoadImage:" + System.DateTime.Now);
                //if (pCompress)
                //    lTexture.Compress(true);
                //print("After Texture.Compress:" + System.DateTime.Now);
                fillPowerOfTwo(lTexture);
                print("After fillPowerOfTwo:" + System.DateTime.Now);
                lOut[x + lXCount * y] = lTexture;
                //lGraphics.Dispose();
                //print("end:" + System.DateTime.Now);
                //}
                lBitmap.Dispose();
                lGraphics.Dispose();
            }
        }
        lMemoryStream.Dispose();
        return lRightBottomRect;
    }

    [SerializeField]
    Texture2D _image;

    [ImageUI(verticalDepth = 0)]
    public Texture2D image
    {
        get { return _image; }
        set
        {
            _image = value;
            //if (previewRenderer)
            //    previewRenderer.material.mainTexture = _image;
            if (_image)
            {
                var lPosition = previewTransform.position;
                var lDrawPosition = drawTransform.position;
                lPosition.x = lDrawPosition.x;
                lPosition.y = lDrawPosition.y;
                previewTransform.position = lPosition;
                Vector2 lSize = zzSceneImageGUI.getFitSize(drawMaxSize, _image);
                previewTransform.localScale = new Vector3(lSize.x, lSize.y, 1f);
            }
        }
    }

    public Transform drawTransform;
    public Vector2 drawMaxSize;
    public Transform previewTransform;
    public GameObject previewRenderer;



    GameObject fileBrowserDialogObject;
    zzFileBrowserDialog fileBrowserDialog;
    public string lastLocation = "";

    [ButtonUI("打开png图片文件")]
    public void fileOpenDialog()
    {
        if (!fileBrowserDialog)
        {
            fileBrowserDialog = zzFileBrowserDialog.createDialog();
            fileBrowserDialog.addFileFilter("", new string[] { "*.png", "*.jpg", "*.jpeg" });
            fileBrowserDialog.relativePosition = new Rect(0.0f, 0.0f, 6.0f / 7.0f, 4.0f / 5.0f);
            fileBrowserDialog.useRelativePosition = new zzGUIRelativeUsedInfo(false, false, true, true);
            fileBrowserDialog.horizontalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.verticalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.selectedLocation = lastLocation;
            fileBrowserDialog.fileSelectedCallFunc = OnReadImage;

        }

    }

    class OutData
    {
        public OutData(CombineImagePlane.ImageInfo lImage, string pExtension,
            int pWidth, int pHeight)
        {
            width = pWidth;
            height = pHeight;
            imageInfo = lImage;
            //if (!Mathf.IsPowerOfTwo(lImage.width)
            //    || !Mathf.IsPowerOfTwo(lImage.height))
            //{
            //    TextureScale.Point(lImage,
            //        Mathf.NextPowerOfTwo(lImage.width), Mathf.NextPowerOfTwo(lImage.height));
            //}
            //resource =GameResourceManager.Main.createImage(lImage);
            //resource.extension = pExtension;
        }
        public int width;
        public int height;
        //public GenericResource<Texture2D> resource;
        public CombineImagePlane.ImageInfo imageInfo;
    }

    Dictionary<string, OutData> imgPathToData = new Dictionary<string, OutData>();
    OutData nowOutData;


    void OnReadImage(zzInterfaceGUI pGUI)
    {
        lastLocation = fileBrowserDialog.selectedLocation;
        readImage(lastLocation);
    }

    public Texture2D[] decomposedImages;

    void readImage(string pPath)
    {
        FileInfo lFileInfo = new FileInfo(pPath);
        if (imgPathToData.ContainsKey(lFileInfo.ToString()))
        {
            nowOutData = imgPathToData[lFileInfo.ToString()];
        }
        else
        {
            var lExtension = System.IO.Path.GetExtension(fileBrowserDialog.selectedLocation);
            lExtension = lExtension.Substring(1, lExtension.Length - 1);
            //{
            int lXCount;
                var lImageFile = System.Drawing.Image.FromFile(fileBrowserDialog.selectedLocation);
                int lWidth = lImageFile.Width;
                int lHeight = lImageFile.Height;
                var lImageInfo = new CombineImagePlane.ImageInfo();
                //if (lWidth > maxSize || lHeight > maxSize)
                //{
                    print("decomposeImage(lImageFile)");
                    lImageInfo.rightBottomRect = decomposeImage(lImageFile, PixelFormat.Format24bppRgb,
                        out decomposedImages, out lXCount);
                    //lImageFile.Dispose();
                    //return;
                //}
                lImageFile.Dispose();
                lImageInfo.images = new RenderMaterialResourceInfo[decomposedImages.Length];
                lImageInfo.xCount = lXCount;
                int i = 0;
                foreach (var lDecomposedImage in decomposedImages)
                {
                    var lResource = GameResourceManager.Main.createImage(lDecomposedImage);
                    lResource.extension = lExtension;
                    lImageInfo.images[i] = lResource;
                    ++i;
                }

            //}
            //Texture2D lImage = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            //using (var lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open))
            //{
            //    BinaryReader lBinaryReader = new BinaryReader(lImageFile);
            //    lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
                //}
                nowOutData = new OutData(lImageInfo, lExtension, lWidth, lHeight);
            imgPathToData[lFileInfo.ToString()] = nowOutData;

        }
        previewRenderer.GetComponent<CombineImagePlane>().imageInfo = nowOutData.imageInfo;
        //image = nowOutData.resource.resource;
    }

    
    [ButtonUI("创建布景")]
    public void createScenery()
    {
        if(nowOutData!=null)
        {
            createObject(nowOutData);
        }
    }

    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject)
    {

    }

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    void createObject(OutData pPainterOutData)
    {
        GameObject lGameObject = GameSystem.Singleton.createObject("sceneryObject");
        lGameObject.GetComponent<CombineImagePlane>().imageInfo = pPainterOutData.imageInfo;
        //lGameObject.GetComponent<RenderMaterialProperty>().imageResource = pPainterOutData.resource;
        var lTransform = lGameObject.transform;
        lTransform.localScale = previewTransform.localScale;
        lTransform.rotation = previewTransform.rotation;
        lTransform.position = previewTransform.position;
        addObjectEvent(lGameObject);
    }
}