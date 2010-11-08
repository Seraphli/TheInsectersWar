using UnityEngine;
using System.Collections;

public class smooth : MonoBehaviour
{

    // The target we are following
    public Vector3 target;

    // Smooth switcher
    public Vector3 local;

    public GameObject mainCamera;
    public float range = 100f;

    // λ����;һ֡�ڵ��߼�, ���Կ���Ҫ��positionDamping��ʱ��,����Ŀ��
    public float positionDamping = 0.74f;

    //void Start()
    //{

    //}


    /// <summary>
    /// ÿ֡����,���ƽ������ֵ
    /// </summary>
    /// <param name="OffsetValue">��Ŀ��ֵ��ƫ����,���������ƫ�Ƶ�ת���λ��</param>
    /// <param name="lastValue">Ŀ��ֵ</param>
    /// <param name="nowValue">Ŀǰ��ֵ</param>
    /// <returns></returns>
    protected float tranF(float OffsetValue, float lastValue, float nowValue)
    {
        //Ҫ��positionDamping��ʱ��,����Ŀ��λ��
        return (lastValue + OffsetValue /* Time.deltaTime*/ - nowValue) / positionDamping * Time.deltaTime;
    }

    /// <summary>
    /// ÿ֡����,���x���ϵ�ƽ������ֵ
    /// </summary>
    /// <param name="OV">��Ŀ��ֵ��xƫ����</param>
    /// <returns></returns>
    protected float tranFX(float OV)
    {
        return tranF(OV, target.x, local.x);
    }

    /// <summary>
    /// ÿ֡����,���y���ϵ�ƽ������ֵ
    /// </summary>
    /// <param name="OV">��Ŀ��ֵ��yƫ����</param>
    /// <returns></returns>
    protected float tranFY(float OV)
    {
        return tranF(OV, target.y, local.y);
    }

    /// <summary>
    /// �Ƿ�Ҫ�ƶ����ж�
    /// </summary>
    /// <returns></returns>
    protected bool TarTranNotEqual()
    {
        if (local.x != target.x || local.y != target.y)
            return true;
        else
            return false;
    }

    void Update()
    {
        if (target != null && target != null)
        {
            float translationx=0f;
            float translationy=0f;

            if (Input.GetButton("left"))
            {
                mainCamera.transform.Translate(-10f * Time.deltaTime, 0, 0);

                translationx = tranFX(-range);
                if (Input.GetButton("up"))
                {
                    mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                    translationy = tranFY(-range);
                }
                else if (Input.GetButton("down"))
                {
                    mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                    translationy = tranFY(range);
                }
                else
                {
                    //translationy = tranFY(0.0f);
                }

                //local.x += translationx;
                //local.y += translationy;

            }
            else if (Input.GetButton("right"))
            {
                mainCamera.transform.Translate(10f * Time.deltaTime, 0, 0);

                translationx = tranFX(range);
                if (Input.GetButton("up"))
                {
                    mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                    translationy = tranFY(-range);
                }
                else if (Input.GetButton("down"))
                {
                    mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                    translationy = tranFY(range);
                }
                //else
                //{
                //    //translationy = tranFY(0.0f);
                //}

                //local.x += translationx;
                //local.y += translationy;
            }
                
            else if (Input.GetButton("up"))
            {
                //translationx = tranFX(0.0f);
                translationy = tranFY(-range);
                mainCamera.transform.Translate(0, 10f * Time.deltaTime, 0);

                //local.x += translationx;
                //local.y += translationy;
            }
            else if (Input.GetButton("down"))
            {
                //translationx = tranFX(0.0f);
                translationy = tranFY(range);
                mainCamera.transform.Translate(0, -10f * Time.deltaTime, 0);

                //local.x += translationx;
                //local.y += translationy;

            }
            else
            {


                //translationx = tranFX(0.0f);
                //translationy = tranFY(0.0f);

                //local.x += translationx;
                //local.y += translationy;

            }


            local.x += translationx;
            local.y += translationy;


            //print(translationx);
            //if (translationx >= 0)
            //{
            //    local.x += translationx;
            //}
            //else
            //{
            //    mainCamera.transform.Translate(translationx, 0, 0);
            //}

            //if (translationy >= 0)
            //{
            //    local.y += translationy;
            //}
            //else
            //{
            //    mainCamera.transform.Translate(0, translationy, 0);
            //}

        }
        //print(local.x);
    }
}
