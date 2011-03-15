using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class zzModelPainterProcessor : zzIModelPainterProcessor
{

    public enum SweepMode
    {
        ignoreColor,
        designatedColor,
        ignoreAlphaZero,
    }
    //Texture2D prePicture;
    public float ignoreDistanceInSweeping = 1.7f;
    public float thickness = 10.0f;

    public SweepMode sweepMode;

    public Color colorInSweepSetting;


    public override void showPicture(){}

    public override void pickPicture()
    {
        activeChart = new zzActiveChart(picture.width, picture.height);
        if (sweepMode == SweepMode.ignoreColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) != colorInSweepSetting);
                }
            }

        }
        else if (sweepMode == SweepMode.designatedColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) == colorInSweepSetting);
                }
            }

        }
        else //if (sweepMode == SweepMode.ignoreAlphaZero)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y).a != 0);
                }
            }

        }
        zzOutlineSweeper.removeIsolatedPoint(activeChart);

    }


    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject)
    {
        return addSimplyPolygon(pPoints, pName, pDebugerObject, Color.red);
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject, Color lDebugLineColor)
    {
        if (pPoints.Length < 3)
            return null;

        zzSimplyPolygon lPolygon = new zzSimplyPolygon();
        lPolygon.setShape(pPoints);

        modelPainterData.pointNumber += lPolygon.pointNum;
        return lPolygon;
    }

    public override void convexDecompose()
    {
        modelPainterData.convexesList = new List<zzSimplyPolygon[]>();
        int index = 0;
        foreach (var lConcave in concaves)
        {
            zzSimplyPolygon[] ldecomposed = lConcave.decompose();
            modelPainterData.convexesList.Add(ldecomposed);
        }
    }


}