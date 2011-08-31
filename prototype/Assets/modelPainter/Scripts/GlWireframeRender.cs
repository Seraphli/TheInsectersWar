using UnityEngine;
using System.Collections;


class GlWireframeRender : MonoBehaviour
{
    Vector3 P0;
    Vector3 P1;
    Vector3 P2;

    public Transform meshOwner;
    public Color lineColor;

    Vector3[] vertices;

    static Material lineMaterial;

    void Start()
    {
        CreateLineMaterial();
        setOwner(meshOwner);

    }

    void setOwner(Transform pOwner)
    {
        meshOwner = pOwner;
        var filter = meshOwner.GetComponent<MeshFilter>();
        var mesh = filter.mesh;
        var lVertices = mesh.vertices;
        var lTriangles = mesh.triangles;
        vertices = new Vector3[lTriangles.Length];
        int i = -1;
        int lMaxIndex = lTriangles.Length - 1;
        while (i < lMaxIndex)
        {
            int lIndex = ++i;
            vertices[lIndex] = lVertices[lTriangles[lIndex]];
        }

        //for (int k = 0; k < triangles.length / 3; k++)
        //{
        //    vertices[i++] = (vertices[triangles[k * 3]]);
        //    vertices[i++] = (vertices[triangles[k * 3 + 1]]);
        //    vertices[i++] = (vertices[triangles[k * 3 + 2]]);
        //}

    }

    void OnPostRender()
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        int i = -1;
        int lMaxIndex = vertices.Length - 1;
        while (i < lMaxIndex)
        {
            P0 = meshOwner.TransformPoint(vertices[++i]);
            P1 = meshOwner.TransformPoint(vertices[++i]);
            P2 = meshOwner.TransformPoint(vertices[++i]);

            GL.Vertex3(P0.x, P0.y, P0.z);
            GL.Vertex3(P1.x, P1.y, P1.z);
            GL.Vertex3(P2.x, P2.y, P2.z);
            GL.Vertex3(P0.x, P0.y, P0.z);
        }

        GL.End();
    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Front Fog { Mode Off } " +
                "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

}