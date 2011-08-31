using UnityEngine;

public abstract class IPropertyGUI
{
    public GUISkin skin;

    public Rect windowRect;

    public virtual void beginImpGUI(ObjectPropertyWindow pWindow) { }

    public abstract void OnPropertyGUI(MonoBehaviour pObject);

    public virtual void endImpGUI() { }
}