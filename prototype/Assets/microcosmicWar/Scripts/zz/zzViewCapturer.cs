
using UnityEngine;
using System.Collections;
using System.IO;

public class zzViewCapturer:MonoBehaviour
{
    public Texture2D capturedImage;
    public string savePath;
    public KeyCode captureButton;

    //[ContextMenu("capture")]
    public void  capture()
    {
        if (capturedImage)
            DestroyImmediate(capturedImage);
        //capturedImage = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        //capturedImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        //capturedImage.Apply();
        Camera lCamera = Camera.mainCamera;
        Color lPreBackgroudColor = lCamera.backgroundColor;

        lCamera.backgroundColor = Color.black;
        lCamera.Render();
        var lBlackBackgroundCapture = captureView();
        removeBackgroud(lBlackBackgroundCapture, Color.black);

        lCamera.backgroundColor = Color.white;
        lCamera.Render();
        var lWhiteBackgroundCapture = captureView();
        removeBackgroud(lWhiteBackgroundCapture, Color.white);

        for (int x = 0; x < lWhiteBackgroundCapture.width; ++x)
        {
            for (int y = 0; y < lWhiteBackgroundCapture.height; ++y)
            {
                Color lColor = lWhiteBackgroundCapture.GetPixel(x, y);
                if (lColor != Color.clear)
                {
                    lBlackBackgroundCapture.SetPixel(x, y, lColor);
                }
            }
        }
        DestroyImmediate(lWhiteBackgroundCapture);

        capturedImage = lBlackBackgroundCapture;
    }

    Texture2D captureView()
    {
        Texture2D lOut = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        lOut.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        return lOut;
    }

    public void save()
    {
        writeData(capturedImage, Application.dataPath + "/../" + savePath+".png");
    }

    public void writeData(Texture2D pData, string pFullName)
    {
        using (var lFile = new FileStream(pFullName, FileMode.Create))
        {
            BinaryWriter lWriter = new BinaryWriter(lFile);
            lWriter.Write(pData.EncodeToPNG());
        }
    }

    static void removeBackgroud(Texture2D pImage,Color pBackgroudColor)
    {
        for (int x = 0; x < pImage.width;++x )
        {
            for (int y = 0; y < pImage.height;++y )
            {
                if (pImage.GetPixel(x, y) == pBackgroudColor)
                {
                    pImage.SetPixel(x, y,Color.clear);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(captureButton))
        {
            capture();
            save();
            DestroyImmediate(capturedImage);
        }
    }
}