using System.Collections.Generic;
using UnityEngine;

namespace Payload.MonoScript
{
    public class TriggerDrawer : MonoBehaviour
    {
        private bool Active = true;

        private KeyCode Switch = KeyCode.End;
        private KeyCode DrawDistInc = KeyCode.PageUp;
        private KeyCode DrawDistDec = KeyCode.PageDown;

        private float DrawDistance = 100f;
        private HashSet<Collider> TriggerList = new HashSet<Collider>();
        private HashSet<Collider2D> Trigger2DList = new HashSet<Collider2D>();

        private Vector3[] GetColliderVertexPositions(Collider c)
        {
            var vertices = new Vector3[8];
            var thisMatrix = c.transform.localToWorldMatrix;
            var storedRotation = c.transform.rotation;
            c.transform.rotation = Quaternion.identity;

            var extents = c.bounds.extents;
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
        private Vector3[] GetCollider2DVertexPositions(Collider2D c2d)
        {
            var vertices = new Vector3[8];
            var thisMatrix = c2d.transform.localToWorldMatrix;
            var storedRotation = c2d.transform.rotation;
            c2d.transform.rotation = Quaternion.identity;

            var extents = c2d.bounds.extents;
            vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
            vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
            vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
            vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
            vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
            vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
            vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
            vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);

            c2d.transform.rotation = storedRotation;
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
        private void DrawVertex2D(Vector3[] v)
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
                if (c) DrawVertex(GetColliderVertexPositions(c));
                else
                {
                    TriggerList.Remove(c);
                    break;
                }
            }

            foreach (Collider2D c in Trigger2DList)
            {
                if (c) DrawVertex2D(GetCollider2DVertexPositions(c));
                else
                {
                    Trigger2DList.Remove(c);
                    break;
                }
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

                    if (c)
                    {
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

                    Collider2D c2d = go.GetComponent<Collider2D>();

                    if (c2d)
                    {
                        if (Vector2.Distance(go.transform.position, transform.position) <= DrawDistance)
                        {
                            if (c2d && c2d.isTrigger)
                            {
                                Trigger2DList.Add(c2d);
                            }
                        }
                        else if (Trigger2DList.Contains(c2d))
                        {
                            Trigger2DList.Remove(c2d);
                        }

                    }
                }
            }

            Invoke("RefreshTriggerList", 2f);
        }
    }
}
