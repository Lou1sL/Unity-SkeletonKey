using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class TriggerDrawer : MonoBehaviour
    {
        private static bool Active = true;

        private static KeyCode Switch = KeyCode.End;
        private static KeyCode DrawDistInc = KeyCode.PageUp;
        private static KeyCode DrawDistDec = KeyCode.PageDown;

        private static float DrawDistance = 100f;
        private static HashSet<Collider> TriggerList = new HashSet<Collider>();


        private static Vector3[] GetColliderVertexPositions(Collider c)
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
        private static void DrawVertex(Vector3[] v)
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

        private static Material lineMat;

        private void Start()
        {
            //TODO:May not included in build
            lineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Disabled);
            
            RefreshTriggerList();
        }

        void OnPostRender()
        {
            if (!Active) return;

            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(new Color(0f, 1f, 0f, 1f));

            foreach (Collider c in TriggerList)
            {
                DrawVertex(GetColliderVertexPositions(c));
            }

            GL.End();
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                Active = !Active;
                RefreshTriggerList();
            }
            if (Input.GetKeyDown(DrawDistInc))
            {
                DrawDistance += 10f;
                RefreshTriggerList();
            }
            if (Input.GetKeyDown(DrawDistDec))
            {
                DrawDistance -= 10f;
                RefreshTriggerList();
            }
            if (DrawDistance < 0f) DrawDistance = 0f;
        }


        private void RefreshTriggerList()
        {
            CancelInvoke();

            if (Active)
            {
                GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

                foreach (var go in allGameObjects)
                {
                    Collider c = go.GetComponent<Collider>();

                    if (Vector3.Distance(go.transform.position, transform.position) <= DrawDistance)
                    {
                        if (c && c.isTrigger)
                        {
                            TriggerList.Add(c);
                        }
                    }
                    else if (TriggerList.Contains(c))
                    {
                        TriggerList.Remove(c);
                    }
                }
            }

            Invoke("RefreshTriggerList", 0.5f);
        }
    }
}
