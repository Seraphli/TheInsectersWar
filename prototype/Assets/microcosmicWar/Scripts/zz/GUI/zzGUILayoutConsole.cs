using UnityEngine;

public abstract class zzGUIConsoleBase : zzInterfaceGUI
{
    protected System.Action messagesDataAddedEvent;

    public void addDataAddedEventReceiver(System.Action pReceiver)
    {
        messagesDataAddedEvent += pReceiver;
    }

    protected static void nullEventReceiver() { }

    protected void Start()
    {
        if (messagesDataAddedEvent == null)
            messagesDataAddedEvent = nullEventReceiver;
    }

    public abstract void addMessage(string pText,Color pColor);

}

public abstract class zzGUILayoutConsoleBase : zzGUIConsoleBase
{
    public GUIStyle textStyle;

    //public int maxMessageCount = 50;
    public int beginIndex = 0;

    public int messageCount = 0;

    public void clear()
    {
        beginIndex = 0;
        messageCount = 0;
    }

    public abstract int messagesDataLength
    {
        get;
    }

    public override void impGUI(Rect rect)
    {
        var lPreColor = GUI.color;
        GUILayout.BeginVertical();
        int lEnd;
        if (beginIndex + messageCount <= messagesDataLength)
            drawMessage(beginIndex, beginIndex + messageCount);
        else
        {
            drawMessage(beginIndex, messagesDataLength);
            drawMessage(0, beginIndex);
        }
        GUILayout.EndVertical();
        GUI.color = lPreColor;
    }

    protected abstract void setMessageData(int pIndex, string pText, Color pColor);

    public override void addMessage(string pText,Color pColor)
    {
        int lWriteIndex;
        //已达最多的消息状态,删除之前的消息
        if (messageCount >= messagesDataLength)
        {
            lWriteIndex = beginIndex++;
            if (beginIndex >= messagesDataLength)
                beginIndex = 0;
        }
        else
        {
            lWriteIndex = beginIndex + messageCount;
            ++messageCount;
            messagesDataAddedEvent();
        }
        setMessageData(lWriteIndex, pText, pColor);
    }

    protected abstract void drawMessage(int lBeginIndex, int lEndIndex);

}

public class zzGUILayoutConsole : zzGUILayoutConsoleBase
{

    [System.Serializable]
    public class TextData
    {
        public string text = string.Empty;
        public Color color;
    }

    public TextData[] messagesData = new TextData[] { };

    public override int messagesDataLength
    {
        get { return messagesData.Length; }
    }

    protected override void setMessageData(int pIndex, string pText, Color pColor)
    {
        messagesData[pIndex].text = pText;
        messagesData[pIndex].color = pColor;
    }

    protected override void drawMessage(int lBeginIndex, int lEndIndex)
    {
        for (int i = lBeginIndex; i < lEndIndex; ++i)
        {
            var lTextData = messagesData[i];
            GUI.color = lTextData.color;
            GUILayout.Label(lTextData.text, textStyle);
        }
    }

}