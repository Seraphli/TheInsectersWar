using UnityEngine;
using System.Collections;

public class Airplane:MonoBehaviour
{
    public ActionCommandControl actionCommandControl;
    protected Transform reverseObjectTransform;
    float Xscale;

    public float speed=8.0f;
    public Emitter emitter;

    void Start()
    {
        reverseObjectTransform = transform.Find("reverse").transform;
        Xscale = reverseObjectTransform.localScale.x;
        UpdateFaceShow();
    }

    void UpdateFaceShow()
    {
        int lFace = actionCommandControl.getFaceValue();
        Vector3 lTemp = reverseObjectTransform.localScale;
        lTemp.x = lFace * Xscale;
        reverseObjectTransform.localScale = lTemp;
    }


    void Update()
    {
        if (actionCommandControl.updateFace())
        {
            UpdateFaceShow();
        }
        transform.Translate(
            speed * actionCommandControl.getFaceValue() * Time.deltaTime,
            0f, 0f);

        var lCommand = actionCommandControl.getCommand();
        if (lCommand.Fire)
        {
            print("lCommand.Fire");
            EmitBullet();
        }
    }

    public void EmitBullet()
    {
        //if (life.isDead())
        //{
        //    return;
        //}
        var lbullet = emitter.EmitBullet()[0];
        lbullet.rigidbody.velocity =
            new Vector3(speed * actionCommandControl.getFaceValue(),0f,0f);
    }
}