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
        private GameObjectSearch search;
        
        private string filter = string.Empty;
        private bool SearchMode = false;
        
        private void Start()
        {
            GOTree = new GameObjectTree();
            GOTree.UpdateTree();
            search = new GameObjectSearch();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(Switch))
                Active = !Active;
        }

        private void OnGUI()
        {
            if (!Active) return;
            
            AllRect.HierRect = GUILayout.Window(WindowID.TRANSFORM_WITH_TRIGGER_LIST, AllRect.HierRect, (id) =>
            {
                if (GUILayout.Button("Update Hierarchy"))
                    GOTree.UpdateTree();

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

                GUI.DragWindow(new Rect(0, 0, AllRect.HierRect.width, 20));
            }, "Scene Hierarchy", AllGUIStyle.DEFAULT_WINDOW);
        }
        
        
    }
}
