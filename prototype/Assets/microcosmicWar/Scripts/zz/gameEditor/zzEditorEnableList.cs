using UnityEngine;
using System.Collections;


public class zzEditorEnableList : zzEditableObject
{
    public MonoBehaviour[] onlyEnableWhenPlay;

    void Awake()
    {
        foreach (var lComponent in onlyEnableWhenPlay)
        {
            lComponent.enabled = false;
        }
    }

    public override void applyPlayState()
    {
        foreach (var lComponent in onlyEnableWhenPlay)
        {
            lComponent.enabled = true;
        }
    }
}