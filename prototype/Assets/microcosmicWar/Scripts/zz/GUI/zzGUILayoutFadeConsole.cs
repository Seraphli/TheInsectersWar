using UnityEngine;

public class zzGUILayoutFadeConsole : zzGUILayoutConsoleBase
{
    public GUIStyle textStyle;

    [System.Serializable]
    public class TextData
    {
        public string text = string.Empty;
        public Color color;
        public float timestamp;
    }

    public TextData[] messagesData = new TextData[] { };

    public override int messagesDataLength
    {
        get { return messagesData.Length; }
    }

    public bool useTimeFade = false;

    public float fullShowLength = 5f;
    public float fadeOutLength = 2f;

    public float fadeOutBeginPos = 5f;
    public float fadeOutEndPos = 7f;

    protected override void setMessageData(int pIndex, string pText, Color pColor)
    {
        messagesData[pIndex] = new TextData()
        {
            timestamp = Time.realtimeSinceStartup,
            color = pColor,
            text = pText,
        };
    }

    protected override void drawMessage(int lBeginIndex, int lEndIndex)
    {
        if (useTimeFade)
        {
            var lTime = Time.realtimeSinceStartup;
            for (int i = lBeginIndex; i < lEndIndex; ++i)
            {
                var lTextData = messagesData[i];
                //var lDelta = lTime + fullShowLength - lTextData.timestamp;
                //if (lDelta > 0f)
                //    lDelta = Mathf.InverseLerp(fadeOutLength - Mathf.Clamp(lDelta, 0f, fadeOutLength)) / fadeOutLength;
                var lColor = lTextData.color;
                lColor.a *= 1f-Mathf.InverseLerp(
                    lTextData.timestamp+fadeOutBeginPos,
                    lTextData.timestamp+fadeOutEndPos,
                    lTime);
                GUI.color = lColor;
                GUILayout.TextArea(lTextData.text, textStyle);
            }
        }
        else
        {
            var lTime = Time.realtimeSinceStartup;
            for (int i = lBeginIndex; i < lEndIndex; ++i)
            {
                var lTextData = messagesData[i];
                GUI.color = lTextData.color;
                GUILayout.TextArea(lTextData.text, textStyle);
            }
        }
    }
}