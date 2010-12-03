using UnityEngine;
using System.Collections;

public class BloodBar : MonoBehaviour
{


    protected Life life;

    protected float fullWidth;

    protected Vector3 localPostion;

    //血的等级,即所用颜色的索引
    protected int lifeLevel = 0;

    protected Material[] bloodBarMaterials;

    protected float levelRate;//1.0f/renderer.materials.Length

    public Color[] bloodColorList;

    void Start()
    {
        if (!life)
        {
            Life lLife = transform.parent.GetComponent<Life>();
            setLife(lLife);

        }

        fullWidth = transform.localScale.x;
        localPostion = transform.localPosition;

        bloodBarMaterials = renderer.materials;
        //levelRate=1.0f/bloodBarMaterials.Length;
        levelRate = 1.0f / (bloodColorList.Length - 1);

        //renderer.material = bloodBarMaterials[lifeLevel];
        renderer.material.color = bloodColorList[lifeLevel];
    }

    //void  Update (){
    //}

    public void UpdateBar(Life life)
    {
        float lFullBloodValue = life.getFullBloodValue();
        float lRate = life.getBloodValue() / lFullBloodValue;
        if (lRate < 0)
            SetRate(0);
        else
            SetRate(lRate);
    }

    public void updateLevel(float pRate)
    {
        //int lLevel =bloodBarMaterials.Length-1- Mathf.FloorToInt( pRate/levelRate );
        int lLevel = bloodColorList.Length - 1 - Mathf.FloorToInt(pRate / levelRate);
        if (lLevel == lifeLevel)
            return ;
        lifeLevel = lLevel;
        //renderer.material = bloodBarMaterials[lifeLevel];
        renderer.material.color = bloodColorList[lifeLevel];
    }

    //pRate>=0
    public void SetRate(float pRate)
    {
        updateLevel(pRate);
        Vector3 lTemp = transform.localScale;
        lTemp.x = fullWidth * pRate;
        transform.localScale = lTemp;

        lTemp = transform.localPosition;
        lTemp.x = localPostion.x + (transform.localScale.x - fullWidth) / 2.0f;
        transform.localPosition = lTemp;
    }


    public void setLife(Life pLife)
    {
        life = pLife;
        life.setBloodValueChangeCallback(UpdateBar);
    }
}