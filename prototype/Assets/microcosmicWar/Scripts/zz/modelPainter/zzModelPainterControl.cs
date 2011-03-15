using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class zzIModelPainterProcessor : MonoBehaviour
{

    public zzModelPainterData modelPainterData;

    public Texture2D picture
    {
        get { return modelPainterData.picture; }
        set { modelPainterData.picture = value; }
    }

    public List<zz2DConcave> concaves
    {
        get { return modelPainterData.concaves; }
        set { modelPainterData.concaves = value; }
    }
    public List<Dictionary<Vector2, int>> pointToIndexList
    {
        get { return modelPainterData.pointToIndexList; }
        set { modelPainterData.pointToIndexList = value; }
    }

    public List<zzSimplyPolygon[]> convexesList
    {
        get { return modelPainterData.convexesList; }
        set { modelPainterData.convexesList = value; }
    }
    public int pointNumber
    {
        get { return modelPainterData.pointNumber; }
        set { modelPainterData.pointNumber = value; }
    }

    public zzActiveChart activeChart
    {
        get { return modelPainterData.activeChart; }
        set { modelPainterData.activeChart = value; }
    }

    public int polygonNumber
    {
        get { return modelPainterData.polygonNumber; }
        set { modelPainterData.polygonNumber = value; }
    }
    public int holeNumber
    {
        get { return modelPainterData.holeNumber; }
        set { modelPainterData.holeNumber = value; }
    }

    public GameObject models
    {
        get { return modelPainterData.models; }
        set { modelPainterData.models = value; }
    }
    public Vector2 modelsSize
    {
        get { return modelPainterData.modelsSize; }
        set { modelPainterData.modelsSize = value; }
    }

    public Texture2D[] imagePatterns
    {
        get { return modelPainterData.imagePatterns; }
        set { modelPainterData.imagePatterns = value; }
    }

    public zzPointBounds[] imagePatternBounds
    {
        get { return modelPainterData.imagePatternBounds; }
        set { modelPainterData.imagePatternBounds = value; }
    }

    public abstract void showPicture();
    public abstract void pickPicture();
    public abstract void sweepPicture();
    public abstract void convexDecompose();
    public abstract void draw();
    public abstract void clear();
}

public class zzModelPainterControl:MonoBehaviour
{
    enum Step
    {
        nothing = 1,
        showPocture,
        pickPicture,
        sweepPicture,
        convexDecompose,
        draw,
        clear,
    }

    public zzIModelPainterProcessor[] modelPainterProcessor;

    void showPicture()
    {
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.showPicture();
        }
    }

    void pickPicture()
    {
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.pickPicture();
        }
    }

    void sweepPicture()
    {
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.sweepPicture();
        }
    }

    void convexDecompose()
    {
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.convexDecompose();
        }
    }

    void draw()
    {
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.draw();
        }
    }


    [SerializeField]
    Step step = Step.nothing;

    [ContextMenu("Step")]
    public bool doStep()
    {
        int lStepValue = (int)step;
        if (lStepValue < (int)Step.clear)
            step = (Step)(lStepValue + 1);
        switch (step)
        {
            case Step.showPocture:
                showPicture();
                break;
            case Step.pickPicture:
                pickPicture();
                break;
            case Step.sweepPicture:
                sweepPicture();
                break;
            case Step.convexDecompose:
                convexDecompose();
                break;
            case Step.draw:
                draw();
                return false;
        }
        return true;
    }

    [ContextMenu("clear")]
    public void clear()
    {
        step = Step.nothing;
        foreach (var lProcessor in modelPainterProcessor)
        {
            lProcessor.clear();
        }
    }
}