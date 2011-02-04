using UnityEngine;
using System.Collections;

public class zzLayerMaskSwitch:MonoBehaviour
{
    public LayerMask layerMask;

    [SerializeField]
    bool _isOn = false;

    public bool isOn
    {
      get { return _isOn; }
      set 
      { 
          _isOn = value; 
          int lLayerMask = getLayerMaskFunc();
          if(_isOn)
          {
              lLayerMask|=layerMask;
          }
          else
          {
              lLayerMask&=~(layerMask);
          }
          setLayerMaskFunc(lLayerMask);
      }
    }

    public delegate int GetLayerMaskFunc();
    public delegate void SetLayerMaskFunc(int pLayerMask);

    GetLayerMaskFunc getLayerMaskFunc;
    SetLayerMaskFunc setLayerMaskFunc;

    public void setGetLayerMaskFunc(GetLayerMaskFunc pGetLayerMaskFunc)
    {
        getLayerMaskFunc = pGetLayerMaskFunc;
    }

    public void addSetLayerMaskReceiver(SetLayerMaskFunc pSetLayerMaskFunc)
    {
        setLayerMaskFunc += pSetLayerMaskFunc;
    }

}