using UnityEngine;
using System.Collections;

public class AddObjectToManager : MonoBehaviour, IEnumerable
{
    public zzSceneManager sceneManager;
    public PlayStateManager playStateManager;

    public void addObject(GameObject pObject)
    {
        sceneManager.addObject(pObject);
        playStateManager.updateObject(pObject);

        var lPaintingMesh = pObject.GetComponent<PaintingMesh>();
        if (lPaintingMesh)
        {
            //设定模型的Z坐标
            var lRenderObjectTransform = lPaintingMesh.transform.Find("Render");
            lRenderObjectTransform.position = GameSystem.Singleton
                .getRenderObjectPos(lRenderObjectTransform.position);
        }

    }

    public IEnumerator GetEnumerator()
    {
        return sceneManager.GetEnumerator();
    }
}