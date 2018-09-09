using System.Collections.Generic;
using UnityEngine;

namespace Payload.MonoScript
{
    public class ColliderDrawer : MonoBehaviour
    {
        private bool Active = true;
        private bool IsTriggerOnly = true;

        private KeyCode Switch = KeyCode.End;
        private KeyCode DrawDistInc = KeyCode.PageUp;
        private KeyCode DrawDistDec = KeyCode.PageDown;

        private Rect HierRect = new Rect(Screen.width * 0.02f, Screen.height * 0.02f, Screen.width * 0.3f, Screen.height*0.96f);
        private Vector2 ScrollPosition = new Vector2();


        private float DrawDistance = 100f;

        private HashSet<Collider> TriggerHashSet = new HashSet<Collider>();
        private HashSet<Collider2D> Trigger2DHashSet = new HashSet<Collider2D>();

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
            
            RefreshTriggerHashSet();
        }

        void OnPostRender()
        {
            if (!Active) return;

            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(new Color(0f, 1f, 0f, 1f));


            foreach (Collider c in TriggerHashSet)
            {
                if (c) DrawVertex(GetColliderVertexPositions(c));
                else
                {
                    TriggerHashSet.Remove(c);
                    break;
                }
            }

            foreach (Collider2D c in Trigger2DHashSet)
            {
                if (c) DrawVertex2D(GetCollider2DVertexPositions(c));
                else
                {
                    Trigger2DHashSet.Remove(c);
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
                RefreshTriggerHashSet();
            }

            if (!Active) return;
            if (Input.GetKeyDown(DrawDistInc))
            {
                DrawDistance += 10f;
                RefreshTriggerHashSet();
            }
            if (Input.GetKeyDown(DrawDistDec))
            {
                DrawDistance -= 10f;
                RefreshTriggerHashSet();
            }
            if (DrawDistance < 0f) DrawDistance = 0f;
        }


        private void RefreshTriggerHashSet()
        {
            CancelInvoke();

            if (Active)
            {
                foreach (var go in FindObjectsOfType<GameObject>())
                {
                    Collider c = go.GetComponent<Collider>();

                    if (c)
                    {
                        if (Vector3.Distance(go.transform.position, transform.position) <= DrawDistance && ((IsTriggerOnly && c.isTrigger) || (!IsTriggerOnly)))
                        {
                            TriggerHashSet.Add(c);
                        }
                        else if (TriggerHashSet.Contains(c))
                        {
                            TriggerHashSet.Remove(c);
                        }
                    }

                    Collider2D c2d = go.GetComponent<Collider2D>();

                    if (c2d)
                    {
                        if (Vector2.Distance(go.transform.position, transform.position) <= DrawDistance && ((IsTriggerOnly && c.isTrigger) || (!IsTriggerOnly)))
                        {
                            Trigger2DHashSet.Add(c2d);
                        }
                        else if (Trigger2DHashSet.Contains(c2d))
                        {
                            Trigger2DHashSet.Remove(c2d);
                        }

                    }
                }
            }

            Invoke("RefreshTriggerHashSet", 2f);
        }

        private void OnGUI()
        {
            if (!Active) return;
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Screen.width, Screen.height),"<color=#00FF00>"+AimingObjName+"</color>");
            
            GUILayout.Window(WindowID.TRANSFORM_WITH_TRIGGER_LIST, HierRect, (id) =>
            {
                GUILayout.BeginHorizontal();
                IsTriggerOnly = GUILayout.Toggle(IsTriggerOnly, "IsTriggerOnly");
                GUILayout.Label("DetectDist(0-500)");
                DrawDistance = GUILayout.HorizontalSlider(DrawDistance, 0f, 500f);
                GUILayout.EndHorizontal();

                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();
                foreach (Collider t in TriggerHashSet)
                {
                    GUILayout.BeginHorizontal();
                    string str = Utils.GetGameObjectPath(t.gameObject);
                    if (GUILayout.Button("□",GUILayout.Width(20)))
                    {
                        TransformModifier.Activate(str);
                    }
                    GUILayout.Label(str, GUIStyles.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }
                foreach (Collider2D t2d in Trigger2DHashSet)
                {
                    GUILayout.BeginHorizontal();
                    string str = Utils.GetGameObjectPath(t2d.gameObject);
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TransformModifier.Activate(str);
                    }
                    GUILayout.Label(str, GUIStyles.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }, "Trigger Hierarchy", GUIStyles.DEFAULT_WINDOW);
        }
        
        private string AimingObjName = string.Empty;
        private void FixedUpdate()
        {
            AimingObjName = string.Empty;
            if (!Active) return;

            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), DrawDistance);
            foreach (RaycastHit hit in hits)
            {
                if ((IsTriggerOnly && hit.collider.isTrigger) || (!IsTriggerOnly))
                {
                    AimingObjName += Utils.GetGameObjectPath(hit.transform.gameObject)+"\n";
                }
            }


            RaycastHit2D[] hit2ds = new RaycastHit2D[0];
            
            if (Camera.main.orthographic) hit2ds = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            else hit2ds = Physics2D.RaycastAll(Utils.ScreenToWorldPointPerspective(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit2d in hit2ds)
            {
                if (hit2d)
                {
                    if((IsTriggerOnly && hit2d.collider.isTrigger) || (!IsTriggerOnly))
                        AimingObjName += Utils.GetGameObjectPath(hit2d.transform.gameObject) + "\n";
                }
            }
            
        }




    }
}
