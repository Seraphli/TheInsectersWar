using UnityEngine;
using System.Collections;
/// <summary>
/// �ѻ�ģʽ
/// �йؾѻ��Ĳ���
/// </summary>
public class SniperMode : MonoBehaviour
{
    //�ѻ�����ͼ
    public Texture reticle;
	//̫�� ��ʾ��ͼ
	public Texture near;
    //��ͼ��һ����Ҫ���� 
    public GameObject SystemObject;
    //�����е������
    public GameObject mainCamera;


    //�����ѻ��Ķ���
    private GameObject sniperObject;
    //��������
    private Camera cameraT;
    //_2DCameraFollow���
    private _2DCameraFollow cameraFollow;
    //���intervalNumber��ִ��
    private float intervalNumber = 0.01f;
    //ÿ���ƽ��ľ���
    private float distance = 0.1f;
    //�Ƿ����ѻ�ģʽ
    private bool sniperBool = false;
    //ƽ������������ı���
    private Transform targetTemp;
    //ʹ�þѻ���Ӣ��
    private GameObject heroObject;
    //smooth
    private smooth smoothT;
	//���״̬
	private bool shootBool=true;

    //��ͼ����ʼx y����
    private Vector3 vReticle;
    //������ͼ���δ�С
    private float rectWidth;
    private float rectHeight;

    //�������ί�к�������
    //�Ŵ�
    private zzTimer amplificationTime;
    //��С
    private zzTimer reduceTime;
    //���
	private zzTimer shootTime;

    private Ray ray;

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

        //�ҵ�_2DCameraFollow���
        if (!cameraFollow)
        {
            cameraFollow = mainCamera.GetComponent<_2DCameraFollow>();
        }


        amplificationTime = gameObject.AddComponent<zzTimer>();
        amplificationTime.setInterval(intervalNumber);
        amplificationTime.setImpFunction(amplification);
        amplificationTime.enabled = false;

        reduceTime = gameObject.AddComponent<zzTimer>();
        reduceTime.setInterval(intervalNumber);
        reduceTime.setImpFunction(reduce);
        reduceTime.enabled = false;
		
		shootTime = gameObject.AddComponent<zzTimer>();
        shootTime.setInterval(sniperObject.GetComponent<SniperEntrance>().getSniperData().interval);
        shootTime.setImpFunction(shootBoolFunction);
        shootTime.enabled = false;
		
		
    }
	
	private void shootBoolFunction()
	{
		shootBool=true;
		shootTime.enabled=false;
	}
	
	

    // Update is called once per frame
    void Update()
    {
        if (sniperBool)
        {
            if (Input.GetButton("fire"))
            {
                shoot();
                //print("1");
            }
            reticleSmooth();
        }
        Debug.DrawLine(ray.origin, ray.origin);
    }

    private void reticleSmooth()
    {
        vReticle = smoothT.local;
        mainCamera = smoothT.mainCamera;
    }

    /// <summary>
    /// �л�(���� �ر�)�ѻ�ģʽ
    /// </summary>
    /// <param name="sSniper">�ѻ���</param>
    /// <param name="sHero">Ӣ��</param>
    public void changeSniper(GameObject sSniper,GameObject sHero)
    {
        if (!sniperBool)
        {
            sniperBool = true;
            sniperObject = sSniper;

            heroObject = sHero;
            Vector3 v3 = new Vector3(heroObject.transform.position.x,heroObject.transform.position.y,-10f);



            targetTemp = cameraFollow.target;
            cameraFollow.target = null;

            amplificationTime.enabled = true;

            if (!smoothT)
            {
                smoothT=mainCamera.AddComponent<smooth>();
            }


            #warning ����
            rectHeight = Screen.height * 4;
            rectWidth = rectHeight;

            vReticle.y= (Screen.height - rectHeight) / 2;
            vReticle.x = (Screen.width - rectWidth) / 2;

            Vector3 vTemp = new Vector3();

            vTemp = vReticle;

            smoothT.target = vTemp;
            smoothT.local = vReticle;

            smoothT.mainCamera = mainCamera;
        }

    }
    public void changeSniper()
    {
        sniperBool = false;
        sniperObject = null;

        cameraFollow.target = targetTemp;
        targetTemp = null;

        reduceTime.enabled = true;

        
    }




    /// <summary>
    /// ����״̬ 
    /// </summary>
    /// <returns>true�ѻ��Ѿ��� false�ر�</returns>
    public bool getSniperBool()
    {
        return sniperBool;
    }

    public bool checkSniperObject(GameObject dObject)
    {
        if (sniperObject == dObject)
        {
            return true;

        }
        return false;
    }








    /// <summary>
    /// ���
    /// </summary>
    public void shoot()
    {
        //print("2");
		if(!shootBool)
		{
			return ;
		}
        RaycastHit[] hits;
        Vector3 vc3=vReticle;
        vc3.x +=rectWidth / 2;
        vc3.y +=rectHeight/2;
        vc3.z = 0;
        ray = camera.ScreenPointToRay(vc3);



        mainCamera.transform.position = ray.origin;

        hits = Physics.RaycastAll(ray.origin, transform.forward, 100.0F);
        print(ray.origin);

        ArrayList lifeLists = new ArrayList();
        if (hits.Length > 0)
        {
            print(hits.Length);
            for (int i = 0; i <= (hits.Length - 1); i++)
            {
                Transform hit = hits[i].transform;
                do
                {
                    Life lifeTemp = hit.gameObject.GetComponent<Life>();
                    if (lifeTemp)
                    {
                            foreach (Life lList in lifeLists)
                            {
                                if (lList == lifeTemp)
                                {
                                    lifeLists.Remove(lList);
                                }
                            }
                        lifeLists.Add(lifeTemp);
                        lifeTemp = null;
                    }
                    hit = hit.parent;
                }
                while (hit);
            }
        }
        foreach (Life lList in lifeLists)
        {
            lList.injure(sniperObject.GetComponent<SniperEntrance>().getSniperData().damage);
        }
		
		shootBool=false;
		shootTime.enabled=true;
    }

	
	
	
    void OnGUI()
    {
        
        if (sniperBool)
        {
            //��Ⱦ�ѻ���ͼƬ��GUI��
            GUI.DrawTexture(
                new Rect(vReticle.x, vReticle.y,
                    rectWidth,rectHeight), reticle);
			
			Vector3 v3=mainCamera.transform.position;
		v3.z=heroObject.transform.position.z;
		float temp=Vector3.Distance(v3,heroObject.transform.position);
		//if(temp<=50)
		//{
		//	GUI.DrawTexture(
         //       new Rect(near.x, vReticle.y,
         //           rectWidth,rectHeight), near);
		//}
        }
		
    }



    //�Ŵ�����
    private void amplification()
    {
        if (cameraT.orthographicSize >= 5 && sniperBool == true)
        {
            cameraT.orthographicSize -= distance;
        }
        else
        {
            amplificationTime.enabled = false;
        }

    }

    //��С����
    private void reduce()
    {
        if (cameraT.orthographicSize <= 8 && sniperBool == false)
        {
            cameraT.orthographicSize += distance;
        }
        else
        {
            reduceTime.enabled = false;
        }
    }


}