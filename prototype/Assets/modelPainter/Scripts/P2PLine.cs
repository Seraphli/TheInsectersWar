using UnityEngine;
using System.Collections;

public class P2PLine : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer;
    
    [SerializeField]
    Vector3 _beginPosition = new Vector3();

    public bool visible
    {
        get { return _lineRenderer.enabled; }
        set { _lineRenderer.enabled = value; }
    }

    public Vector3 beginPosition
    {
        get { return _beginPosition; }
        set 
        { 
            _beginPosition = value;
            _lineRenderer.SetPosition(0, value);
        }
    }
    
    [SerializeField]
    Vector3 _endPosition = new Vector3();

    public Vector3 endPosition
    {
        get { return _endPosition; }
        set 
        {
            _endPosition = value;
            _lineRenderer.SetPosition(1, value);
        }
    }

}