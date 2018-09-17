using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Payload.MonoScript
{
    public class TransformModifier:MonoBehaviour
    {
        

        private bool Active = false;

        private KeyCode Switch = KeyCode.Home;
        private KeyCode MvSpdInc = KeyCode.End;
        private KeyCode MvSpdDec = KeyCode.Delete;

        private float MvSpd = 100f;

        private Transform TargetTransform;
        private Component TargetComponent;

        
        
        private Vector2 ScrollPosition = new Vector2();
        private Vector2 ScrollPositionProp = new Vector2();

        private static TransformModifier Instance = null;
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(Switch))
            {
                if (Active)
                    DeActivate();
                else
                    Activate();
            }

            if (Active)
            {
                MvSpdModify();

                if (TargetTransform)
                    MovingInput();
                else
                {
                    this.InjectLogError("GameObject Has Been Destoried!");
                    DeActivate();
                }
            }
        }

        private void DeActivate()
        {
            Active = false;
            TargetTransform = null;
            TargetComponent = null;
        }
        private void Activate()
        {
            GameObject go = GameObject.Find(TransformPath);

            if (go)
            {
                Active = true;
                TargetTransform = go.transform;
                TargetComponent = null;
            }
            else
            {
                this.InjectLogError("GameObject Not Found!Are you sure you entered the right path?");

                Active = false;
                TargetTransform = null;
                TargetComponent = null;
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

        private string TransformPath = string.Empty;
        private void OnGUI()
        {
            if (Active)
            {
                if (TargetTransform)
                {
                    OnGUIComponentWindow();
                    if (TargetComponent != null)
                        OnGUIPropertyWindow();
                }
            }
            else
                OnGUITransformPathInput();

        }

        private void OnGUIComponentWindow()
        {
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_COMPONENT_LIST, AllRect.CompoRect, (id) =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("MoveSpd(0-500)");
                MvSpd = GUILayout.HorizontalSlider(MvSpd, 0f, 200f);
                GUILayout.EndHorizontal();

                TargetTransform.gameObject.SetActive(GUILayout.Toggle(TargetTransform.gameObject.activeInHierarchy, "Active"));
                GUILayout.Label("Position: " + Utils.Vec32Str(TargetTransform.position));
                GUILayout.Label("EulerAng: " + Utils.Vec32Str(TargetTransform.eulerAngles));
                GUILayout.Label("Scale: " + Utils.Vec32Str(TargetTransform.lossyScale));
                
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();
                //
                foreach (Component cp in TargetTransform.GetComponents<Component>())
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TargetComponent = cp;
                    }
                    if (GUILayout.Button("RM", GUILayout.Width(40)))
                    {
                        Destroy(cp);
                    }
                    GUILayout.Label(cp.GetType().Name, AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (GUILayout.Button("Close")) DeActivate();

            }, "Components On " + Utils.GetGameObjectPath(TargetTransform.gameObject), AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUIPropertyWindow()
        {
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_PROPERTIES_LIST, AllRect.PropRect, (id) =>
            {
                ScrollPositionProp = GUILayout.BeginScrollView(ScrollPositionProp);

                Reflector.DrawVarList(TargetComponent);

                GUILayout.EndScrollView();

                if (GUILayout.Button("Close")) TargetComponent = null;

            }, "Var On " + TargetComponent.GetType().Name, AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUITransformPathInput()
        {
            TransformPath = GUI.TextField(AllRect.PathInputerRect, TransformPath);
        }
        
        public static void Activate(Transform transform)
        {
            if (!Instance)
                return;
            Instance.TransformPath = string.Empty;
            Instance.Active = true;
            Instance.TargetTransform = transform;
            Instance.TargetComponent = null;
        }
    }
}
