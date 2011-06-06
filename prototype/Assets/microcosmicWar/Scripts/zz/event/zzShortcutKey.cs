using UnityEngine;

public class zzShortcutKey:MonoBehaviour
{
    [System.Serializable]
    public class ShortcutKeyInfo
    {
        public KeyCode holdKey;
        public KeyCode downKey;

        public zzOnAction action;
    }

    public ShortcutKeyInfo[] shortcutKeyList = new ShortcutKeyInfo[0]{};

    void Update()
    {
        foreach (var lInfo in shortcutKeyList)
        {
            if (
                Input.GetKey(lInfo.holdKey)
                && Input.GetKeyDown(lInfo.downKey))
            {
                //print(lInfo.action.name);
                lInfo.action.impAction();
            }
        }
    }
}