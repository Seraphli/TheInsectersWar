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

    zzPlaneMesh planeMesh = new zzPlaneMesh();

    float preRulerRange;

    float rulerRange
    {
        get
        {
            switch (mRuleDirection)
            {
                case RuleDirection.vertical:
                    {
                        return transform.lossyScale.y;
                    }
                case RuleDirection.horizontal:
                    {
                        return transform.lossyScale.x ;
                    }
            }
            return 0f;

        }
    }

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
        changeRange();
        preRulerRange = rulerRange;
    }

    void OnDestroy()
    {
        planeMesh.clear();
    }

    void Update()
    {
        if (!gameObject.GetComponent<MeshFilter>())
            Start();
        float lNowRulerRange = rulerRange;
        if (preRulerRange != lNowRulerRange)
        {
            changeRange();
            preRulerRange = lNowRulerRange;
        }
    }

    private void changeRange()
    {
        switch (mRuleDirection)
        {
            case RuleDirection.vertical:
                {
                    planeMesh.UVs[2] = new Vector2(1.0f, transform.lossyScale.y / imageLength);
                    planeMesh.UVs[3] = new Vector2(0.0f, transform.lossyScale.y / imageLength);
                    break;
                }
            case RuleDirection.horizontal:
                {
                    planeMesh.UVs[1] = new Vector2(transform.lossyScale.x / imageLength, 0.0f);
                    planeMesh.UVs[2] = new Vector2(transform.lossyScale.x / imageLength, 1.0f);

                    break;
                }
        }
        planeMesh.mesh.uv = planeMesh.UVs;
    }
}