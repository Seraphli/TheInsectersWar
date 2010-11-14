using UnityEngine;
using System.Collections;


/// <summary>
/// ���˴����ļ�������Ӣ������ ����ѻ�̨��ʹ�þѻ�ǹ��ʱ�������ѻ�ģʽ
/// </summary>
public class StartSniperMode : MonoBehaviour
{


    //��ͼ��һ����Ҫ���� 
    public GameObject SystemObject;
    //�����е������
    public GameObject mainCamera;

    //��������
    private Camera cameraT;
    //������ǰ��ActionCommandControl
    private ActionCommandControl lActionCommandControlTemp;
    //mainInput
    private mainInput IMainInput;
    //SniperMode
    private SniperMode lSniperMode;
    //�ѻ�̨
    private GameObject sniperObjectT;

    // Use this for initialization
    void Start()
    {
        //�ҵ�System����
        if (!SystemObject)
        {
            SystemObject = GameObject.Find("System");
        }

        //�ҵ������
        if (!(mainCamera && cameraT))
        {
            mainCamera = GameObject.Find("Main Camera");
            cameraT = mainCamera.GetComponent<Camera>();
        }
        //�ҵ�SniperMode���
        if (!lSniperMode)
        {
            lSniperMode = mainCamera.GetComponent<SniperMode>();
        }

        //�ҵ�mainInput���
        if (!IMainInput)
        {
            IMainInput = SystemObject.GetComponent<mainInput>();
        }

        
    }

    // Update is called once per frame
    //�ھѻ�̨ǰ��Ծ��ʱ�������ѻ�
    void Update()
    {

        //��⿴�Ƿ�����Ծ   
        if (Input.GetButtonDown("jump"))
        {
            
            if (!lSniperMode.getSniperBool())
            {
                
                //�ѻ���δ����

                if (surveyBool())
                {
                    startSniper();
                }
            }
            else
            {
                //�ѻ����ѿ���

                if (raycastGameObject()&&lSniperMode.checkSniperObject(sniperObjectT))
                {
                    closeSniper();
                }
            }
        }



    }

    //�����ѻ�
    private void startSniper()
    {

        lActionCommandControlTemp = IMainInput.actionCommandControl;
        IMainInput.actionCommandControl = null;
        reset();
        lSniperMode.changeSniper(sniperObjectT,gameObject);

    }


    //�رվѻ�
    private void closeSniper()
    {

        IMainInput.actionCommandControl = lActionCommandControlTemp;
        reset();
        lSniperMode.changeSniper();
        
        //print()

    }

    //�������λ��Ӣ�۵�ǰ����
    private void reset()
    {
        Vector3 v3 = gameObject.transform.position;
        v3.z = -10f;
        mainCamera.transform.position = v3;
    }

    //����Ƿ��ھѻ�̨ �ѻ�̨�Ƿ���
    private bool surveyBool()
    {

        if (raycastGameObject())
        {
            sniperObjectT = raycastGameObject();
            if (sniperObjectT.GetComponent<SniperEntrance>().getStatus())
            {
                return true;
            }
        }

        return false;
    }

    //��Ͷ���ѻ���
    //���ؾѻ�̨
    private GameObject raycastGameObject()
    {
        //��Ͷ���Ӣ�۵�ǰλ�õ�����
        RaycastHit[] hits;
        Vector3 vc3 = transform.position;
        vc3.z -= 50;
        hits = Physics.RaycastAll(vc3, transform.forward, 100.0F);
        if (hits.Length > 0)
        {
            for (int i = 0; i <=(hits.Length - 1); i++)
            {
                //һֱ��Ѱ������ ���Ƿ�ѻ�̨ ֪���������
                Transform hit = hits[i].transform;
                do
                {
                    if (hit.tag == "sniper")
                    {
                        return hit.gameObject;
                    }
                    hit = hit.parent;
                }
                while (hit);
            }
        }
        return null;
    }
}
