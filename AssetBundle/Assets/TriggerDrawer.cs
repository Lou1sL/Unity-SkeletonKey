using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerDrawer : MonoBehaviour
{
    private bool Active = true;
    private KeyCode Switch = KeyCode.PageUp;
    private List<Collider> GetTriggers()
    {
        List<Collider> clist = new List<Collider>();

        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

        foreach (var gameObject in allGameObjects)
        {
            Collider c = gameObject.GetComponent<Collider>();

            if (c && c.isTrigger)
            {
                clist.Add(c);
            }
        }
        return clist;
    }
    private Vector3[] GetColliderVertexPositions(Collider c)
    {
        var vertices = new Vector3[8];
        var thisMatrix = c.transform.localToWorldMatrix;
        var storedRotation = c.transform.rotation;
        c.transform.rotation = Quaternion.identity;

        var extents = c.GetComponent<Collider>().bounds.extents;
        vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
        vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
        vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
        vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);

        c.transform.rotation = storedRotation;
        return vertices;
    }
    private void DrawVertex(Vector3[] v)
    {
        //UP
        GL.Vertex3(v[0].x, v[0].y, v[0].z);
        GL.Vertex3(v[1].x, v[1].y, v[1].z);

        GL.Vertex3(v[1].x, v[1].y, v[1].z);
        GL.Vertex3(v[3].x, v[3].y, v[3].z);

        GL.Vertex3(v[3].x, v[3].y, v[3].z);
        GL.Vertex3(v[2].x, v[2].y, v[2].z);

        GL.Vertex3(v[2].x, v[2].y, v[2].z);
        GL.Vertex3(v[0].x, v[0].y, v[0].z);

        //SIDE
        GL.Vertex3(v[0].x, v[0].y, v[0].z);
        GL.Vertex3(v[4].x, v[4].y, v[4].z);

        GL.Vertex3(v[1].x, v[1].y, v[1].z);
        GL.Vertex3(v[5].x, v[5].y, v[5].z);

        GL.Vertex3(v[3].x, v[3].y, v[3].z);
        GL.Vertex3(v[7].x, v[7].y, v[7].z);

        GL.Vertex3(v[2].x, v[2].y, v[2].z);
        GL.Vertex3(v[6].x, v[6].y, v[6].z);

        //BOTTOM
        GL.Vertex3(v[4].x, v[4].y, v[4].z);
        GL.Vertex3(v[5].x, v[5].y, v[5].z);

        GL.Vertex3(v[5].x, v[5].y, v[5].z);
        GL.Vertex3(v[7].x, v[7].y, v[7].z);

        GL.Vertex3(v[7].x, v[7].y, v[7].z);
        GL.Vertex3(v[6].x, v[6].y, v[6].z);

        GL.Vertex3(v[6].x, v[6].y, v[6].z);
        GL.Vertex3(v[4].x, v[4].y, v[4].z);
    }

    private Material lineMat;

    private void Awake()
    {
        //TODO:May not included in build
        lineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Disabled);
    }

    void OnPostRender()
    {
        if (!Active) return;

        GL.Begin(GL.LINES);
        lineMat.SetPass(0);
        GL.Color(new Color(0f, 1f, 0f, 1f));

        foreach (Collider c in GetTriggers())
        {
            DrawVertex(GetColliderVertexPositions(c));
        }

        GL.End();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Switch)) Active = !Active;
    }
}