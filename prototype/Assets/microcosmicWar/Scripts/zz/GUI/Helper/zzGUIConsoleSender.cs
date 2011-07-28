using UnityEngine;

public class zzGUIConsoleSender:MonoBehaviour
{
    public zzGUILayoutConsole console;
    public Color messageColor;

    public void writeMessage(string pText)
    {
        console.addMessage(pText, messageColor);
    }
}