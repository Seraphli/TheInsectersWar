
function Update () {
}

@MenuItem ("2D/Move Onto 2D Plane ^2")
static function MoveOnto2DPlane () {
     for (var transform in Selection.transforms) 
	 {
          transform.position.z = 0;
     }
}

@MenuItem ("2D/Move Onto 2D Plane ^2", true)
static function ValidateMoveOnto2DPlane () 
{
     return (Selection.activeTransform != null);
}

@MenuItem ("2D/Make Selection 2D Rigidbody")
static function MakeSelection2DRigidbody () 
{
     //MoveOnto2DPlane();
 
     for (var transform in Selection.transforms) {
          var lRigidbody : Rigidbody = transform.GetComponent(Rigidbody);
      
          if (!lRigidbody)
               transform.gameObject.AddComponent(Rigidbody);
      
          var configurableJoint : ConfigurableJoint = 
transform.GetComponent(ConfigurableJoint);
      
          if (!configurableJoint)
               configurableJoint = 
transform.gameObject.AddComponent(ConfigurableJoint);
      
          //configurableJoint.configuredInWorldSpace = true;
          configurableJoint.xMotion = ConfigurableJointMotion.Free;
          configurableJoint.yMotion = ConfigurableJointMotion.Free;
          configurableJoint.zMotion = ConfigurableJointMotion.Locked;
          configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
          configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
          configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
     } 
}

@MenuItem ("2D/Make Selection 2D Rigidbody", true)
static function ValidateMakeSelection2DRigidbody () 
{
     return (Selection.activeTransform != null);
}