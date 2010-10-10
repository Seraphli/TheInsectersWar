using UnityEngine;
using System.Collections;

public class _2DCameraFollow : MonoBehaviour
{
    // The target we are following
    public Transform target;
    // Smooth switcher
    public bool useSmooth = true;
    // How much we 
    public float positionDamping = 0.74f;

    void Start()
    {

    }

    public void setTaget(Transform pTarget)
    {
        target = pTarget;
    }

    protected float tranF(float OffsetValue, float lastValue, float nowValue)
    {
        return (lastValue + OffsetValue /* Time.deltaTime*/ - nowValue) / positionDamping * Time.deltaTime;
    }

    protected float tranFX(float OV)
    {
        return tranF(OV, target.position.x, transform.position.x);
    }

    protected float tranFY(float OV)
    {
        return tranF(OV, target.position.y, transform.position.y);
    }

    protected bool TarTranNotEqual()
    {
        if (transform.position.x != target.position.x || transform.position.y != target.position.y)
            return true;
        else
            return false;
    }

    void Update()
    {
        if (target)
        {
            if (useSmooth)
            {
                float translationx;
                float translationy;

                if (Input.GetButton("left"))
                {
                    translationx = tranFX(-10.0f);
                    if (Input.GetButton("up"))
                    {
                        translationy = tranFY(4.0f);
                    }
                    else if (Input.GetButton("down"))
                    {
                        translationy = tranFY(-4.0f);
                    }
                    else
                        translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("right"))
                {
                    translationx = tranFX(10.0f);
                    if (Input.GetButton("up"))
                    {
                        translationy = tranFY(4.0f);
                    }
                    else if (Input.GetButton("down"))
                    {
                        translationy = tranFY(-4.0f);
                    }
                    else
                        translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("up"))
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(4.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else if (Input.GetButton("down"))
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(-4.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                else
                {
                    translationx = tranFX(0.0f);
                    translationy = tranFY(0.0f);
                    if (TarTranNotEqual())
                        transform.Translate(translationx, translationy, 0);
                }
                //transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, timeDamping);
                //transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 1.74f);
                //transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 1.74f); 
                //transform.position = target.position;
            }
            else
            {
                Vector3 lTargetPos = target.position;
                lTargetPos.z = 0.0f;
                transform.position = lTargetPos;
                //transform.position.x = target.position.x;
                //transform.position.y = target.position.y;
            }
        }
    }
}
