using UnityEngine;

public class zzInputPassword : MonoBehaviour
{
    public KeyCode[] password;

    public int inputPostion = 0;

    System.Action passEvent;

    public void addPassReceiver(System.Action pReceiver)
    {
        passEvent += pReceiver;
    }

    void OnGUI()
    {
        var lEvent = Event.current;

        if (lEvent.type == EventType.keyDown && lEvent.keyCode!= KeyCode.None)
        {
            if (lEvent.keyCode == password[inputPostion])
            {
                if ((++inputPostion) == password.Length)
                {
                    passEvent();
                    enabled = false;
                }
            }
            else
            {
                inputPostion = 0;
                if (lEvent.keyCode == password[inputPostion])
                    ++inputPostion;
            }
        }
    }
}