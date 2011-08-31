using UnityEngine;
using System.Collections;

public class ObjectPickList : ObjectPickBase
{
    public ObjectPickBase[] picks;

    [SerializeField]
    int _selected;

    public int selected
    {
        get { return _selected; }
        set 
        {
            if (value < picks.Length)
            {
                OnLeftOff(null);
                OnRightOff(null);
                _selected = value; 
            }
        }
    }

    public override void OnLeftOn(GameObject pObject)
    {
        picks[_selected].OnLeftOn(pObject);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        picks[_selected].OnLeftOff(pObject);
    }

    public override void OnRightOn(GameObject pObject)
    {
        picks[_selected].OnRightOn(pObject);
    }

    public override void OnRightOff(GameObject pObject)
    {
        picks[_selected].OnRightOff(pObject);
    }

}