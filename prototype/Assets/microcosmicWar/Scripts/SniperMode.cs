using UnityEngine;
using System.Collections;
/// <summary>
/// 狙击模式
/// 有关狙击的操作
/// </summary>
public class SniperMode : MonoBehaviour
{
    //狙击镜贴图
    public Texture reticle;
	//太近 提示贴图
	public Texture near;
    //地图的一个重要对象 
    public GameObject SystemObject;
    //场景中的摄像机
    public GameObject mainCamera;


    //启动狙击的对象
    private GameObject sniperObject;
    //摄像机组件
    private Camera cameraT;
    //_2DCameraFollow组件
    private _2DCameraFollow cameraFollow;
    //间隔intervalNumber秒执行
    private float intervalNumber = 0.01f;
    //每次推进的距离
    private float distance = 0.1f;
    //是否开启狙击模式
    private bool sniperBool = false;
    //平滑跟随跟随对象的备份
    private Transform targetTemp;
    //使用狙击的英雄
    private GameObject heroObject;
    //smooth
    private smooth smoothT;
	//射击状态
	private bool shootBool=true;

    //贴图的起始x y坐标
    private Vector3 vReticle;
    //用于贴图矩形大小
    private float rectWidth;
    private float rectHeight;

    //间隔调用委托函数的类
    //放大
    private zzTimer amplificationTime;
    //缩小
    private zzTimer reduceTime;
    //射击
	private zzTimer shootTime;

    private Ray ray;

    // Use this for initialization
    void Start()
    {
        //找到System对象
        if (!SystemObject)
        {
            SystemObject = GameObject.Find("System");
        }

        //找到摄像机
        if (!(mainCamera && cameraT))
        {
            mainCamera = GameObject.Find("Main Camera");
            cameraT = mainCamera.GetComponent<Camera>();
        }

        //找到_2DCameraFollow组件
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
    /// 切换(开启 关闭)狙击模式
    /// </summary>
    /// <param name="sSniper">狙击器</param>
    /// <param name="sHero">英雄</param>
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


            #warning 更改
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
    /// 返回状态 
    /// </summary>
    /// <returns>true狙击已经打开 false关闭</returns>
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
    /// 射击
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
            //渲染狙击镜图片到GUI上
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



    //放大物体
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

    //缩小物体
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