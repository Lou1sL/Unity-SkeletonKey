using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Payload.MonoScript
{
    public class SceneHierarchy : MonoBehaviour
    {
        private bool Active = true;
        

        private KeyCode Switch = KeyCode.Insert;

        private GameObjectTree GOTree;
        private DrawCollider drawCollider;
        private GameObjectSearch search;

        private new Camera camera;

        private Material LineMat;
        private float DrawDistance = 100f;
        private string filter = string.Empty;
        private bool SearchMode = false;
        
        private void Start()
        {
            GOTree = new GameObjectTree();
            GOTree.UpdateTree();
            drawCollider = new DrawCollider();
            search = new GameObjectSearch();
            camera = GetComponent<Camera>();

            //TODO:May not included in build
            LineMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            LineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Disabled);
        }
        private void OnPostRender()
        {
            if (!Active) return;
            drawCollider.OnPostRenderer(LineMat);
        }
        private void Update()
        {
            if (Input.GetKeyDown(Switch))
                Active = !Active;
        }

        private void OnGUI()
        {
            if (!Active) return;
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Screen.width, Screen.height),"<color=#00FF00>"+AimingObjName+"</color>");
            
            GUILayout.Window(WindowID.TRANSFORM_WITH_TRIGGER_LIST, AllRect.HierRect, (id) =>
            {
                if (GUILayout.Button("Update Hierarchy && Collider Cache"))
                {
                    GOTree.UpdateTree();
                    drawCollider.UpdateCache(FindObjectsOfType<GameObject>(),camera.transform.position,DrawDistance,filter);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("GizmosDist(0-500)", GUILayout.Width(110));
                DrawDistance = GUILayout.HorizontalSlider(DrawDistance, 0f, 500f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                filter = GUILayout.TextField(filter);

                if (GUILayout.Button("?", GUILayout.Width(20)) && filter != string.Empty)
                {
                    search.Search(FindObjectsOfType<GameObject>(), filter);
                    SearchMode = true;
                }

                if (SearchMode && GUILayout.Button("X", GUILayout.Width(20))) SearchMode = false;

                GUILayout.EndHorizontal();

                if (SearchMode) search.OnGUISearch();
                else GOTree.OnGUIHierarchy();


                if (GUILayout.Button("Close")) Active = false;
            }, "Collider Hierarchy", AllGUIStyle.DEFAULT_WINDOW);
        }
        
        private string AimingObjName = string.Empty;
        private void FixedUpdate()
        {
            AimingObjName = string.Empty;
            if (!Active) return;
            AimingObjName = MouseRayCastCollider();
        }

        private string MouseRayCastCollider()
        {
            string str = String.Empty;
            RaycastHit[] hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), DrawDistance);
            foreach (RaycastHit hit in hits)
            {
                    str += Utils.GetGameObjectPath(hit.transform.gameObject) + "\n";
            }
            
            RaycastHit2D[] hit2ds = new RaycastHit2D[0];

            if (camera.orthographic) hit2ds = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            else hit2ds = Physics2D.RaycastAll(Utils.ScreenToWorldPointPerspective(Input.mousePosition, camera), Vector2.zero);
            foreach (RaycastHit2D hit2d in hit2ds)
            {
                if (hit2d)
                {
                        str += Utils.GetGameObjectPath(hit2d.transform.gameObject) + "\n";
                }
            }
            return str;
        }
    }
}
