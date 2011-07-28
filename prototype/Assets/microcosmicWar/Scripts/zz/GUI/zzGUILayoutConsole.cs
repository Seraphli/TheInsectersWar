using UnityEngine;

public class zzGUILayoutConsole : zzInterfaceGUI
{
    public GUIStyle textStyle;

    [System.Serializable]
    public class TextData
    {
        public string text = string.Empty;
        public Color color;
    }

    //public int maxMessageCount = 50;

    public TextData[] messagesData = new TextData[]{ };
    public int beginIndex = 0;

    public int messageCount = 0;

    public override void impGUI(Rect rect)
    {
        var lPreColor = GUI.color;
        GUILayout.BeginVertical();
        int lEnd;
        if (beginIndex + messageCount <= messagesData.Length)
            drawMessage(beginIndex, beginIndex + messageCount);
        else
        {
            drawMessage(beginIndex, messagesData.Length);
            drawMessage(0, beginIndex);
        }
        GUILayout.EndVertical();
        GUI.color = lPreColor;
    }

    public void addMessage(string pText,Color pColor)
    {
        int lWriteIndex;
        //已达最多的消息状态,删除之前的消息
        if (messageCount >= messagesData.Length)
        {
            lWriteIndex = beginIndex++;
            if (beginIndex >= messagesData.Length)
                beginIndex = 0;
        }
        else
        {
            lWriteIndex = beginIndex + messageCount;
            ++messageCount;
        }
        messagesData[lWriteIndex].text = pText;
        messagesData[lWriteIndex].color = pColor;
    }

    void drawMessage(int lBeginIndex,int lEndIndex)
    {
        for (int i = lBeginIndex; i < lEndIndex; ++i)
        {
            var lTextData = messagesData[i];
            GUI.color = lTextData.color;
            GUILayout.Label(lTextData.text, textStyle);
        }
    }
}