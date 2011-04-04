using UnityEngine;

public class zzTransparencyCaptureExample:MonoBehaviour
{
    public Renderer captureShow;
    public Texture2D capturedImage;
    public Transform cameraTransform;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
    }

    public void capture()
    {
        //capture whole screen
        Rect lRect = new Rect(0f,0f,Screen.width,Screen.height);
        if(capturedImage)
            Destroy(capturedImage);
        capturedImage = zzTransparencyCapture.capture(lRect);
        captureShow.material.mainTexture = capturedImage;
    }

    Vector3 lastMousePosition;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            capture();

        Vector3 lTranslate = Input.mousePosition - lastMousePosition;
        lTranslate*=0.15f;
        cameraTransform.Translate(lTranslate);
        lastMousePosition = Input.mousePosition;
    }

    void OnGUI()
    {
        GUI.color = Color.black;
        GUILayout.Label("press C to do transparent capturing");
    }
}