using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (zzSpriteAsset))]
public class zzSpriteAssetEditor:Editor
{
    public int imageMaxHeight = 150;
    public float timePos = 0;
    public int framePos = 0;
    public bool play = false;
    public float lastUpdateTime;

    void OnEnable()
    {
        EditorApplication.update += UpdateSprite;
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    void OnDisable()
    {
        EditorApplication.update -= UpdateSprite;
    }

    void UpdateSprite()
    {
        if(play)
        {
            zzSpriteAsset lSpriteAsset = (zzSpriteAsset)target;
            timePos = lSpriteAsset.moveTime(timePos, Time.realtimeSinceStartup - lastUpdateTime);
            if (lSpriteAsset.getFrameIndex(timePos) != framePos)
                Repaint();
        }
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    public static Vector2 getFitSize(float pMaxWidth, float pMaxHeigth,
        float lWidth, float lHeigth)
    {

        float lWidthHeigthRate = lWidth / lHeigth;

        if ((pMaxWidth / pMaxHeigth) > lWidthHeigthRate)
        {
            pMaxWidth = lWidthHeigthRate * pMaxHeigth;
        }
        else
        {
            pMaxHeigth = pMaxWidth / lWidthHeigthRate;
        }
        return new Vector2(pMaxWidth, pMaxHeigth);

    }

    void drawTextureClipped(Texture2D pImage, Rect pPosition, Rect pClipped)
    {

        GUI.BeginGroup(pPosition);
        var lImageWidth = pPosition.width / pClipped.width;
        var lImageHeigth = pPosition.height / pClipped.height;
        var lImageRect = new Rect(
             -lImageWidth * pClipped.x,
             -lImageHeigth * pClipped.y,
            lImageWidth,
            lImageHeigth);
        GUI.DrawTexture(lImageRect, pImage, ScaleMode.StretchToFill, true);
        GUI.EndGroup();
    }

    public override void OnInspectorGUI()
    {
        zzSpriteAsset lSpriteAsset = (zzSpriteAsset)target;
        var lImage = lSpriteAsset.image;
        var lLastTimePos = timePos;
        var lLastFramePos = framePos;
        if (lImage)
        {
            GUILayout.BeginVertical();
            float lSpace = 30;
            var lImageWidth = Screen.width - 20;
            var lImageRect = new Rect(10, lSpace, lImageWidth, imageMaxHeight);
            GUI.DrawTexture(lImageRect, lImage, ScaleMode.ScaleToFit, true);
            lSpace += imageMaxHeight + 10;


            var lSourceImageSize = getFitSize((float)lImageWidth,
                    (float)imageMaxHeight,
                    lImage.width*lSpriteAsset.spriteWidth,
                    lImage.height*lSpriteAsset.spriteHeigth);

            var lSpriteScreenPos = new Rect(((float)Screen.width - lSourceImageSize.x) / 2f,
                    lSpace, lSourceImageSize.x, lSourceImageSize.y);

            var lSpriteImagePos = lSpriteAsset.smapleFrameRect(framePos);

            drawTextureClipped(lImage, lSpriteScreenPos, lSpriteImagePos);

            lSpace += lSourceImageSize.y;

            GUILayout.Space(lSpace);
            GUILayout.BeginHorizontal();
            {
                //timePos = EditorGUILayout.FloatField("time:", timePos);
                framePos = EditorGUILayout.IntField("frame", framePos);
                if (play)
                    play = !GUILayout.Button("||");
                else
                    play = GUILayout.Button(">");
            }
            GUILayout.EndHorizontal();
            timePos = EditorGUILayout.Slider("time",timePos, 0f, lSpriteAsset.length);
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }
        base.OnInspectorGUI();
        lSpriteAsset.xCount = Mathf.Max(1, lSpriteAsset.xCount);
        lSpriteAsset.yCount = Mathf.Max(1, lSpriteAsset.yCount);
        var lPicCount = lSpriteAsset.xCount * lSpriteAsset.yCount;
        lSpriteAsset.beginIndex = Mathf.Clamp(lSpriteAsset.beginIndex, 0, lPicCount);
        lSpriteAsset.endIndex = Mathf.Clamp(lSpriteAsset.endIndex, 0, lPicCount);

        if(lLastFramePos!=framePos)
        {
            framePos = Mathf.Clamp(framePos, 0, lSpriteAsset.maxFramePos);
            timePos = lSpriteAsset.getTimePosition(framePos);
        }
        else
        {
            timePos = Mathf.Clamp(timePos, 0, lSpriteAsset.length);
            framePos = lSpriteAsset.getFrameIndex(timePos);
        }
    }

    [MenuItem("Assets/Create/zzSprite")]
    public static void CreateAsset()
    {
        var lSelection = Selection.activeObject;
        string lPath;
        if (
            lSelection
            && AssetDatabase.Contains(lSelection))
        {
            var lSelectionPath = AssetDatabase.GetAssetPath(lSelection);
            lPath = lSelectionPath.Substring(0, lSelectionPath.LastIndexOf("/") + 1);
        }
        else
            lPath = "Assets/";
        //zzSpriteAsset asset;
        string lAssetName = "zzSpriteAsset";
        string lAssetPath = lPath+lAssetName;
        int lNameIdx = 0;

        if (System.IO.File.Exists(lAssetPath + ".asset"))
        {
            lNameIdx = 1;
            while (System.IO.File.Exists(lAssetPath + lNameIdx + ".asset"))
            {
                lNameIdx++;
            }
        }

        var lAsset = new zzSpriteAsset();
        AssetDatabase.CreateAsset(lAsset, lAssetPath + (lNameIdx != 0 ? "" + lNameIdx : "") + ".asset");

        //EditorUtility.FocusProjectView();
        Selection.activeObject = lAsset;
    }
}