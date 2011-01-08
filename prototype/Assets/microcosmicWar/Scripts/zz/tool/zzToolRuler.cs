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


    //// The vertices of mesh
    //// 3--2
    //// |  |
    //// 0--1
    //protected Vector3[] vertices = new Vector3[]{
    //            new Vector3(0,0,0),
    //            new Vector3(1,0,0),
    //            new Vector3(1,1,0),
    //            new Vector3(0,1,0)
    //    };

    //// Indices into the vertex array
    //protected int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

    //// UV coordinates
    //protected Vector2[] UVs = new Vector2[] { 
    //    new Vector2(0, 0), new Vector2(1, 0),
    //    new Vector2(1, 1), new Vector2(0, 1) 
    //};
    zzPlaneMesh planeMesh = new zzPlaneMesh();

    MeshRenderer meshRenderer;
    void Start()
    {
        //planeMesh.useSharedDataInEdit = false;
        planeMesh.Init(gameObject);
        planeMesh.resize(1.0f, 1.0f, zzPlaneMesh.PivotType.leftBottom);

        mRuleDirection = ruleDirection;
        switch (mRuleDirection)
        {
            case RuleDirection.vertical:
                {
                    planeMesh.meshRenderer.material = verticalRulerMaterial;
                    break;
                }
            case RuleDirection.horizontal:
                {
                    planeMesh.meshRenderer.material = horizontalRulerMaterial;
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

                    planeMesh.UVs[2] = new Vector2(1.0f, transform.lossyScale.y / imageLength);
                    planeMesh.UVs[3] = new Vector2(0.0f, transform.lossyScale.y / imageLength);
                    break;
                }
            case RuleDirection.horizontal:
                {
                    planeMesh.UVs[1] = new Vector2(transform.lossyScale.x / imageLength, 0.0f);
                    planeMesh.UVs[2] = new Vector2(transform.lossyScale.x / imageLength, 1.0f);

                    //UVs[2] = new Vector2(1.0f, transform.localScale.y );
                    //UVs[3] = new Vector2(0.0f, transform.localScale.y );
                    break;
                }
        }
        planeMesh.mesh.uv = planeMesh.UVs;
    }
}