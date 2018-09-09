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
        

        private bool IsActive = false;

        private KeyCode Switch = KeyCode.ScrollLock;
        private KeyCode MvSpdInc = KeyCode.PageUp;
        private KeyCode MvSpdDec = KeyCode.PageDown;

        private float MvSpd = 100f;

        private Transform TargetTransform;
        private MonoBehaviour TargetComponent;

        private Rect PathInputerRect = new Rect(Screen.width * 0.35f, Screen.height * 0.02f, Screen.width * 0.3f, 20);

        private Rect CompoRect = new Rect(Screen.width * 0.34f, Screen.height * 0.02f, Screen.width * 0.3f, Screen.height * 0.4f);
        private Rect PropRect = new Rect(Screen.width * 0.34f, Screen.height * 0.42f, Screen.width * 0.3f, Screen.height * 0.56f);

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
                if (IsActive)
                    DeActivate();
                else
                    Activate();
            }

            if (IsActive)
            {
                MvSpdModify();

                if (TargetTransform)
                    MovingInput();
                else
                {
                    Debug.LogError("__Injector--: TransformMover: \r\n GameObject Has Been Destoried!");
                    DeActivate();
                }
            }
        }

        private void DeActivate()
        {
            IsActive = false;
            TargetTransform = null;
            TargetComponent = null;
        }
        private void Activate()
        {
            GameObject go = GameObject.Find(TransformPath);

            if (go)
            {
                IsActive = true;
                TargetTransform = go.transform;
                TargetComponent = null;
            }
            else
            {
                Debug.LogError("__Injector--: TransformMover: \r\n GameObject Not Found!Are you sure you entered the right path?");

                IsActive = false;
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
            if (IsActive)
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
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_COMPONENT_LIST, CompoRect, (id) =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("MoveSpd(0-500)");
                MvSpd = GUILayout.HorizontalSlider(MvSpd, 0f, 200f);
                GUILayout.EndHorizontal();

                TargetTransform.gameObject.SetActive(GUILayout.Toggle(TargetTransform.gameObject.activeInHierarchy, "Active"));
                GUILayout.Label("Position: " + Utils.Vec2Str(TargetTransform.position));
                GUILayout.Label("EulerAng: " + Utils.Vec2Str(TargetTransform.eulerAngles));
                GUILayout.Label("Scale: " + Utils.Vec2Str(TargetTransform.lossyScale));
                
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.BeginVertical();
                //
                foreach (MonoBehaviour mb in TargetTransform.GetComponents<MonoBehaviour>())
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TargetComponent = mb;
                    }
                    GUILayout.Label(mb.GetType().Name, GUIStyles.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

            }, "Components On " + Utils.GetGameObjectPath(TargetTransform.gameObject), GUIStyles.DEFAULT_WINDOW);
        }
        private void OnGUIPropertyWindow()
        {
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_PROPERTIES_LIST, PropRect, (id) =>
            {
                TargetComponent.enabled = (GUILayout.Toggle(TargetComponent.enabled, "Enabled"));
                ScrollPositionProp = GUILayout.BeginScrollView(ScrollPositionProp);
                GUILayout.BeginVertical();
                //
                IList<PropertyInfo> props = new List<PropertyInfo>(TargetComponent.GetType().GetProperties());
                foreach (PropertyInfo prop in props)
                {
                    GUILayout.BeginHorizontal();
                    
                    if (prop.CanRead)
                    {
                        object val = prop.GetValue(TargetComponent, null);
                        GUILayout.Label(prop.Name + "(" + prop.PropertyType.Name + ") : " + val, GUIStyles.DEFAULT_LABEL);
                        if (prop.CanWrite)
                        {
                            if (prop.PropertyType == typeof(bool))
                            {
                                try
                                {
                                    prop.SetValue(TargetComponent, GUILayout.Toggle((bool)val, ""), null);
                                }
                                catch (Exception e)
                                {
                                    //TODO:...Damn
                                    Debug.LogError("__Injector--: TransformModifier: \r\n BOOL!" + e.StackTrace);
                                }
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                try
                                {
                                    prop.SetValue(TargetComponent, Convert.ToInt32(GUILayout.TextField(((int)val) + "")), null);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("__Injector--: TransformModifier: \r\n INT!" + e.StackTrace);
                                }
                            }
                            else if (prop.PropertyType == typeof(float))
                            {
                                try
                                {
                                    prop.SetValue(TargetComponent, Convert.ToSingle(GUILayout.TextField(((float)val) + "")), null);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("__Injector--: TransformModifier: \r\n FLOAT!" + e.StackTrace);
                                }
                            }
                            else if (prop.PropertyType == typeof(double))
                            {
                                try
                                {
                                    prop.SetValue(TargetComponent, Convert.ToDouble(GUILayout.TextField(((double)val) + "")), null);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("__Injector--: TransformModifier: \r\n DOUBLE!" + e.StackTrace);
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                try
                                {
                                    prop.SetValue(TargetComponent, GUILayout.TextField((string)val), null);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("__Injector--: TransformModifier: \r\n BOOL!" + e.StackTrace);
                                }
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label(prop.Name + "(" + prop.PropertyType.Name + ") : __UNREADABLE", GUIStyles.DEFAULT_LABEL);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

            }, "Properties On " + TargetComponent.GetType().Name, GUIStyles.DEFAULT_WINDOW);
        }
        private void OnGUITransformPathInput()
        {
            TransformPath = GUI.TextField(PathInputerRect, TransformPath);
        }


        public static void Activate(string path)
        {
            if(!Instance)
                Debug.LogError("__Injector--: TransformMover: \r\n Instance Not Initialized!");
            Instance.TransformPath = path;
            Instance.Activate();
        }
    }
}
