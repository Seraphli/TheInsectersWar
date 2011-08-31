using UnityEngine;

[System.Serializable]
public class RenderMaterialResourceInfo : GenericResourceInfo
{
    public static implicit operator RenderMaterialResourceInfo(GenericResource<Texture2D> pValue)
    {
        var lInfo = pValue.info;
        return new RenderMaterialResourceInfo() { 
            extension = lInfo.extension,
            resourceID = lInfo.resourceID,
            resourceType = lInfo.resourceType };
    }

    public static explicit operator GenericResource<Texture2D>(RenderMaterialResourceInfo pValue)
    {
        return GameResourceManager.Main.getImage(pValue.resourceID);
    }

    public static explicit operator Material(RenderMaterialResourceInfo pValue)
    {
        return GameSystem.Singleton.getRenderMaterial(pValue.resourceID).material;
    }

    public GenericResource<Texture2D> resource
    {
        get { return (GenericResource<Texture2D>)this; }
    }

    public static Material exceptionMaterial
    {
        get
        {
            return GameSystem.Singleton.getRenderMaterial(0).material;
        }
    }
}

[System.Serializable]
public class ModelResourceInfo : GenericResourceInfo
{
    public static implicit operator ModelResourceInfo(GenericResource<PaintingModelData> pValue)
    {
        var lInfo = pValue.info;
        return new ModelResourceInfo()
        {
            extension = lInfo.extension,
            resourceID = lInfo.resourceID,
            resourceType = lInfo.resourceType
        };
    }

    public static explicit operator GenericResource<PaintingModelData>(ModelResourceInfo pValue)
    {
        return GameResourceManager.Main.getModel(pValue.resourceID);
    }

    public GenericResource<PaintingModelData> resource
    {
        get { return (GenericResource<PaintingModelData>)this; }
    }
}