using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JointObject : zzEditableObject
{

    public override void applyPlayState()
    {
        UpdateJoint();
    }

    public override void applyPauseState()
    {
        foreach (var lJoint in JointList)
        {
            Destroy(lJoint);
        }
        JointList = new ConfigurableJoint[0];
    }

    //六个扫描序列为x x y y z z
    public zzRigidbodySweepDetector[] sweepDetectors;

    [SerializeField]
    ConfigurableJoint[] JointList = new ConfigurableJoint[0];

    [SerializeField]
    bool _freezeObjectRotation = false;

    void updateFreeze()
    {
        ConfigurableJointMotion lAngularMotion;
        if (_freezeObjectRotation)
            lAngularMotion = ConfigurableJointMotion.Locked;
        else
            lAngularMotion = ConfigurableJointMotion.Free;


        ConfigurableJointMotion lMoveMotion = ConfigurableJointMotion.Locked;
        foreach (var lJoint in JointList)
        {
            lJoint.angularXMotion = lAngularMotion;
            lJoint.angularYMotion = lAngularMotion;
            lJoint.angularZMotion = lAngularMotion;

            lJoint.xMotion = lMoveMotion;
            lJoint.yMotion = lMoveMotion;
            lJoint.zMotion = lMoveMotion;
        }

    }

    [FieldUI("冻结被约束物体旋转",verticalDepth=10)]
    public bool freezeObjectRotation
    {
        get { return _freezeObjectRotation; }

        set
        {
            _freezeObjectRotation = value;
            updateFreeze();
        }
    }

    void getRigidbodySweep(zzRigidbodySweepDetector pDetector, float pDistance, HashSet<Rigidbody> pBody)
    {
        pDetector.distance = pDistance;
        foreach (var lHit in pDetector.SweetTest())
        {
            pBody.Add(lHit.rigidbody);
        }
    }

    [ContextMenu("UpdateJoint")]
    public void UpdateJoint()
    {
        HashSet<Rigidbody> lBodyToConnect = new HashSet<Rigidbody>();
        var lScale = transform.lossyScale;
        getRigidbodySweep(sweepDetectors[0], lScale.x, lBodyToConnect);
        getRigidbodySweep(sweepDetectors[1], lScale.x, lBodyToConnect);

        getRigidbodySweep(sweepDetectors[2], lScale.y, lBodyToConnect);
        getRigidbodySweep(sweepDetectors[3], lScale.y, lBodyToConnect);

        getRigidbodySweep(sweepDetectors[4], lScale.z, lBodyToConnect);
        getRigidbodySweep(sweepDetectors[5], lScale.z, lBodyToConnect);


        var lNewJointList = new List<ConfigurableJoint>(lBodyToConnect.Count);
        foreach (var lJoint in JointList)
        {
            if (lBodyToConnect.Contains(lJoint.connectedBody))
            {
                //添加到新列表中,从 创建表中移除
                lNewJointList.Add(lJoint);
                lBodyToConnect.Remove(lJoint.connectedBody);
            }
            else
            {
                Destroy(lJoint);
            }
        }

        //连接未连接的刚体
        foreach (var lRigidbody in lBodyToConnect)
        {
            var lJoint = gameObject.AddComponent<ConfigurableJoint>();
            lNewJointList.Add(lJoint);
            lJoint.connectedBody = lRigidbody;
        }
        JointList = lNewJointList.ToArray();
        updateFreeze();
    }
}