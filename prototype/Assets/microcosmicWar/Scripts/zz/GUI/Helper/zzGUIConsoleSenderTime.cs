using UnityEngine;

public class zzGUIConsoleSenderTime : MonoBehaviour
{
    public zzGUILayoutConsole console;
    public Color messageColor;
    public string format = "HH:mm ";

    public void writeMessage(string pText)
    {
        console.addMessage(System.DateTime.Now.ToString(format) + pText, messageColor);
    }
}