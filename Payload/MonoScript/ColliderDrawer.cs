using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Payload.MonoScript
{
    public class ColliderDrawer : MonoBehaviour
    {
        private bool Active = true;
        

        private KeyCode Switch = KeyCode.Insert;
        private KeyCode DrawDistInc = KeyCode.End;
        private KeyCode DrawDistDec = KeyCode.Delete;

        private ColliderManager cManager = new ColliderManager();

        

        private new Camera camera;

        
        private void Start()
        {
            camera = GetComponent<Camera>();

            //TODO:May not included in build
            cManager.lineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            cManager.lineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Disabled);


            InvokeRepeating("RefreshColliderHashSet",0.01f,1f);
            
        }

        void OnPostRender()
        {
            if (!Active) return;
            cManager.PostRenderProcess();
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch))
                Active = !Active;

            if (!Active) return;
            if (Input.GetKeyDown(DrawDistInc))
                cManager.DrawDistance += 10f;
            if (Input.GetKeyDown(DrawDistDec))
                cManager.DrawDistance -= 10f;
            
        }


        private void RefreshColliderHashSet()
        {
            if (Active)
            {
                foreach (var go in FindObjectsOfType<GameObject>())
                {
                    cManager.UpdateCollider(go,transform.position);
                }
            }
            
        }

        private void OnGUI()
        {
            if (!Active) return;
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Screen.width, Screen.height),"<color=#00FF00>"+AimingObjName+"</color>");
            
            GUILayout.Window(WindowID.TRANSFORM_WITH_TRIGGER_LIST, AllRect.HierRect, (id) =>
            {
                cManager.OnGUIDrawScrollView();
                if (GUILayout.Button("Close", GUILayout.Height(20)))
                {
                    Active = false;
                }

            }, "Trigger Hierarchy", AllGUIStyle.DEFAULT_WINDOW);
        }
        
        private string AimingObjName = string.Empty;
        private void FixedUpdate()
        {
            AimingObjName = string.Empty;
            if (!Active) return;
            AimingObjName = cManager.MouseRayCastCollider(camera);
        }

        
        private class ColliderManager
        {
            private HashSet<Collider> ColliderHashSet = new HashSet<Collider>();
            private HashSet<Collider2D> Collider2DHashSet = new HashSet<Collider2D>();

            private Vector2 GUIScrollPosition = new Vector2();

            public bool IsTriggerOnly = true;
            public float DrawDistance = 100f;

            public Material lineMat;

            public string filter = string.Empty;
            public bool filterCaseIgnore = true;

            public void UpdateCollider(GameObject gameObject,Vector3 centerPosi)
            {
                if (DrawDistance < 0f) DrawDistance = 0f;

                Collider collider = gameObject.GetComponent<Collider>();
                Collider2D collider2D = gameObject.GetComponent<Collider2D>();
                
                if (collider)
                {
                    if(Vector3.Distance(gameObject.transform.position, centerPosi) <= DrawDistance && ((IsTriggerOnly && collider.isTrigger) || (!IsTriggerOnly)))
                        ColliderHashSet.Add(collider);
                    else
                        ColliderHashSet.Remove(collider);
                }
                if (collider2D)
                {
                    if (Vector2.Distance(gameObject.transform.position, centerPosi) <= DrawDistance && ((IsTriggerOnly && collider2D.isTrigger) || (!IsTriggerOnly)))
                        Collider2DHashSet.Add(collider2D);
                    else
                        Collider2DHashSet.Remove(collider2D);
                }
            }
       
            public void PostRenderProcess()
            {
                GL.Begin(GL.LINES);
                lineMat.SetPass(0);
                GL.Color(new Color(0f, 1f, 0f, 1f));


                foreach (Collider c in ColliderHashSet)
                {
                    if (c) PostRenderDrawCollider(c);
                    else
                    {
                        ColliderHashSet.Remove(c);
                        break;
                    }
                }

                foreach (Collider2D c in Collider2DHashSet)
                {
                    if (c) PostRenderDrawCollider2D(c);
                    else
                    {
                        Collider2DHashSet.Remove(c);
                        break;
                    }
                }


                GL.End();
            }
            private void PostRenderDrawCollider2D(Collider2D c2d)
            {
                var vertices = new Vector3[4];
                var thisMatrix = c2d.transform.localToWorldMatrix;
                var storedRotation = c2d.transform.rotation;
                c2d.transform.rotation = Quaternion.identity;

                var extents = c2d.bounds.extents;
                vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
                vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, 0));
                vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, 0));
                vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, 0));

                c2d.transform.rotation = storedRotation;

                //UP
                GL.Vertex3(vertices[0].x, vertices[0].y, 0);
                GL.Vertex3(vertices[1].x, vertices[1].y, 0);

                //SIDE
                GL.Vertex3(vertices[0].x, vertices[0].y, 0);
                GL.Vertex3(vertices[2].x, vertices[2].y, 0);

                GL.Vertex3(vertices[1].x, vertices[1].y, 0);
                GL.Vertex3(vertices[3].x, vertices[3].y, 0);

                //BOTTOM
                GL.Vertex3(vertices[2].x, vertices[2].y, 0);
                GL.Vertex3(vertices[3].x, vertices[3].y, 0);
            }
            private void PostRenderDrawCollider(Collider c)
            {
                //Get all verticles
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

                //GL process
                //UP
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);
                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);

                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);
                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);

                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);
                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);

                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);

                //SIDE
                GL.Vertex3(vertices[0].x, vertices[0].y, vertices[0].z);
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);

                GL.Vertex3(vertices[1].x, vertices[1].y, vertices[1].z);
                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);

                GL.Vertex3(vertices[3].x, vertices[3].y, vertices[3].z);
                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);

                GL.Vertex3(vertices[2].x, vertices[2].y, vertices[2].z);
                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);

                //BOTTOM
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);
                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);

                GL.Vertex3(vertices[5].x, vertices[5].y, vertices[5].z);
                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);

                GL.Vertex3(vertices[7].x, vertices[7].y, vertices[7].z);
                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);

                GL.Vertex3(vertices[6].x, vertices[6].y, vertices[6].z);
                GL.Vertex3(vertices[4].x, vertices[4].y, vertices[4].z);
            }

            public void OnGUIDrawScrollView()
            {
                GUILayout.BeginHorizontal();
                IsTriggerOnly = GUILayout.Toggle(IsTriggerOnly, "IsTriggerOnly");
                GUILayout.Label("DetectDist(0-500)");
                DrawDistance = GUILayout.HorizontalSlider(DrawDistance, 0f, 500f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                filter = GUILayout.TextField(filter, GUILayout.Width(Screen.width * .2f));
                filterCaseIgnore = GUILayout.Toggle(filterCaseIgnore, "Ignore case");
                GUILayout.EndHorizontal();

                GUIScrollPosition = GUILayout.BeginScrollView(GUIScrollPosition);
                GUILayout.BeginVertical();
                foreach (Collider t in ColliderHashSet)
                {
                    if (filter != string.Empty && (filterCaseIgnore ? (!t.gameObject.name.ToUpper().Contains(filter.ToUpper())) : (!t.gameObject.name.Contains(filter)))) continue;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TransformModifier.Activate(t.transform);
                    }
                    GUILayout.Label(Utils.GetGameObjectPath(t.gameObject), AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }
                foreach (Collider2D t2d in Collider2DHashSet)
                {
                    if (filter != string.Empty && (filterCaseIgnore ? (!t2d.gameObject.name.ToUpper().Contains(filter.ToUpper())) : (!t2d.gameObject.name.Contains(filter)))) continue;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TransformModifier.Activate(t2d.transform);
                    }
                    GUILayout.Label(Utils.GetGameObjectPath(t2d.gameObject), AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            public string MouseRayCastCollider(Camera camera)
            {
                string str = String.Empty;
                RaycastHit[] hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), DrawDistance);
                foreach (RaycastHit hit in hits)
                {
                    if ((IsTriggerOnly && hit.collider.isTrigger) || (!IsTriggerOnly))
                    {
                        str += Utils.GetGameObjectPath(hit.transform.gameObject) + "\n";
                    }
                }


                RaycastHit2D[] hit2ds = new RaycastHit2D[0];

                if (camera.orthographic) hit2ds = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                else hit2ds = Physics2D.RaycastAll(Utils.ScreenToWorldPointPerspective(Input.mousePosition, camera), Vector2.zero);
                foreach (RaycastHit2D hit2d in hit2ds)
                {
                    if (hit2d)
                    {
                        if ((IsTriggerOnly && hit2d.collider.isTrigger) || (!IsTriggerOnly))
                            str += Utils.GetGameObjectPath(hit2d.transform.gameObject) + "\n";
                    }
                }

                return str;
            }

        }
    }
}
