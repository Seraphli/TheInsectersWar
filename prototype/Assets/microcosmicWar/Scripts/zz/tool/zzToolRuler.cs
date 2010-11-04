using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class zzToolRuler : MonoBehaviour
{
    public enum RuleDirection
    {
        horizontal,
        vertical,
    }

    public Material verticalRulerMaterial;
    public Material horizontalRulerMaterial;

    public RuleDirection ruleDirection;
    RuleDirection mRuleDirection;

    Mesh mesh;
    const float imageLength=10.0f;


    // The vertices of mesh
    // 3--2
    // |  |
    // 0--1
    protected Vector3[] vertices = new Vector3[]{
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                new Vector3(1,1,0),
                new Vector3(0,1,0)
        };

    // Indices into the vertex array
    protected int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

    // UV coordinates
    protected Vector2[] UVs = new Vector2[] { 
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(1, 1), new Vector2(0, 1) 
    };

    MeshRenderer meshRenderer;
    void Start()
    {
        mesh = new Mesh();

        MeshFilter lMeshFilter = GetComponent<MeshFilter>();
        if (lMeshFilter==null)
            lMeshFilter = gameObject.AddComponent<MeshFilter>();
        lMeshFilter.mesh = mesh;

        MeshRenderer lMeshRenderer = GetComponent<MeshRenderer>();
        if (lMeshRenderer==null)
            lMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        //lMeshRenderer.material = verticalRulerMaterial;

        meshRenderer = lMeshRenderer;


        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.triangles = triIndices;
        mesh.normals = new Vector3[]{
            new Vector3(0,0,-1),new Vector3(0,0,-1),
            new Vector3(0,0,-1),new Vector3(0,0,-1)
        };

        mRuleDirection = ruleDirection;
        switch (mRuleDirection)
        {
            case RuleDirection.vertical:
                {
                    meshRenderer.material = verticalRulerMaterial;
                    break;
                }
            case RuleDirection.horizontal:
                {
                    meshRenderer.material = horizontalRulerMaterial;
                    break;
                }
        }
        //初始化,并使用默认动画

        //updateShow();
    }

    void Update()
    {
        switch (mRuleDirection)
        {
            case RuleDirection.vertical:
                {

                    //UVs[1] = new Vector2(transform.localScale.x , 0.0f);
                    //UVs[2] = new Vector2(transform.localScale.x , 1.0f);

                    UVs[2] = new Vector2(1.0f, transform.lossyScale.y / imageLength);
                    UVs[3] = new Vector2(0.0f, transform.lossyScale.y / imageLength);
                    break;
                }
            case RuleDirection.horizontal:
                {
                    UVs[1] = new Vector2(transform.lossyScale.x / imageLength, 0.0f);
                    UVs[2] = new Vector2(transform.lossyScale.x / imageLength, 1.0f);

                    //UVs[2] = new Vector2(1.0f, transform.localScale.y );
                    //UVs[3] = new Vector2(0.0f, transform.localScale.y );
                    break;
                }
        }
        mesh.uv = UVs;
    }
}