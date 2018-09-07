using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class TransformModifier:MonoBehaviour
    {
        private bool Active = false;

        private KeyCode Switch = KeyCode.ScrollLock;
        private KeyCode MvSpdInc = KeyCode.PageUp;
        private KeyCode MvSpdDec = KeyCode.PageDown;

        private float MvSpd = 100f;

        private Transform TargetTransform;

        private Rect PathInputerRect = new Rect(Screen.width * 0.35f, Screen.height * 0.05f, Screen.width * 0.3f, 20);
        private Rect StatisticRect = new Rect(Screen.width * 0.35f, Screen.height * 0.05f, Screen.width * 0.3f, Screen.height * 0.3f);
        private Vector2 ScrollPosition = new Vector2();

        private void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                if (Active)
                {
                    Active = false;
                    TargetTransform = null;
                }
                else
                {
                    GameObject go = GameObject.Find(TransformPath);

                    if (go)
                    {
                        Active = true;
                        TargetTransform = go.transform;
                    }
                    else
                    {
                        Debug.LogError("__Injector--: TransformMover: \r\n GameObject Not Found!Are you sure you entered the right path?");

                        Active = false;
                        TargetTransform = null;
                    }
                }
            }

            if (!Active) return;

            MvSpdModify();

            if (TargetTransform)
            {

                MovingInput();
            }
            else
            {
                Debug.LogError("__Injector--: TransformMover: \r\n GameObject Has Been Destoried!");
            }

            

        }

        private void MvSpdModify()
        {
            if (Input.GetKeyDown(MvSpdInc)) MvSpd += 10f;
            if (Input.GetKeyDown(MvSpdDec)) MvSpd -= 10f;
            if (MvSpd < 0f) MvSpd = 0f;
        }
        private void MovingInput()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                TargetTransform.position += Vector3.forward * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TargetTransform.position += Vector3.back * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                TargetTransform.position += Vector3.left * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                TargetTransform.position += Vector3.right * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                TargetTransform.position += Vector3.up * MvSpd * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightControl))
            {
                TargetTransform.position += Vector3.down * MvSpd * Time.deltaTime;
            }
        }

        public static string TransformPath = string.Empty;
        private void OnGUI()
        {
            if (Active)
            {
                //Transform components
                if (TargetTransform)
                {
                    string str = string.Empty;
                    
                    foreach (MonoBehaviour mb in TargetTransform.GetComponents<MonoBehaviour>())
                        str += mb.GetType().Name + "\n";
                    
                    GUILayout.Window(WindowID.TRANSFORM_MODIFIER_COMPONENT_LIST, StatisticRect, (id) =>
                    {
                        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(StatisticRect.width), GUILayout.Height(StatisticRect.height - 20));
                        GUILayout.Label(str, new GUIStyle(GUI.skin.label) { fontSize = 13 });
                        GUILayout.EndScrollView();
                        
                    }, "Components On " + Utils.GetGameObjectPath(TargetTransform.gameObject), new GUIStyle(GUI.skin.window) { fontSize = 15 });

                }
            }
            else
            {
                //Transform path input
                TransformPath = GUI.TextField(PathInputerRect, TransformPath, 50);
            }
            
        }


    }
}
