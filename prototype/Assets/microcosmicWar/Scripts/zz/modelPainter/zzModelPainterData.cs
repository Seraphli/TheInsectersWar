using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzModelPainterData:MonoBehaviour
{
    public Texture2D picture;

    public List<zz2DConcave> concaves;
    public List<Dictionary<Vector2, int>> pointToIndexList;
    public List<zzSimplyPolygon[]> convexesList;
    public int pointNumber = 0;

    public zzActiveChart activeChart;

    public int polygonNumber = 0;
    public int holeNumber = 0;

    public GameObject models;
    public Vector2 modelsSize;

}