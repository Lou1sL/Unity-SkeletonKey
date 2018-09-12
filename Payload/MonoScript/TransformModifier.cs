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
        private MonoBehaviour TargetComponent;

        

        

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
                foreach (MonoBehaviour mb in TargetTransform.GetComponents<MonoBehaviour>())
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("□", GUILayout.Width(20)))
                    {
                        TargetComponent = mb;
                    }
                    if (GUILayout.Button("RM", GUILayout.Width(40)))
                    {
                        Destroy(mb);
                    }
                    GUILayout.Label(mb.GetType().Name, AllGUIStyle.DEFAULT_LABEL);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (GUILayout.Button("Close", GUILayout.Height(20))) Active = false;

            }, "Components On " + Utils.GetGameObjectPath(TargetTransform.gameObject), AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUIPropertyWindow()
        {
            GUILayout.Window(WindowID.TRANSFORM_MODIFIER_PROPERTIES_LIST, AllRect.PropRect, (id) =>
            {
                TargetComponent.enabled = (GUILayout.Toggle(TargetComponent.enabled, "Enabled"));
                ScrollPositionProp = GUILayout.BeginScrollView(ScrollPositionProp);
                GUILayout.BeginVertical();
                //
                IList<PropertyInfo> props = new List<PropertyInfo>(TargetComponent.GetType().GetProperties());
                foreach (PropertyInfo prop in props)
                {
                    GUILayout.BeginHorizontal();

                    try
                    {
                        if (prop.CanRead)
                        {
                            object val = prop.GetValue(TargetComponent, null);
                            GUILayout.Label(prop.Name + "(" + prop.PropertyType.Name + ") : " + val, AllGUIStyle.DEFAULT_LABEL);
                            if (prop.CanWrite)
                            {
                                if (prop.PropertyType == typeof(bool))
                                    prop.SetValue(TargetComponent, GUILayout.Toggle((bool)val, ""), null);
                                else if (prop.PropertyType == typeof(int))
                                    prop.SetValue(TargetComponent, Convert.ToInt32(GUILayout.TextField(((int)val) + "")), null);
                                else if (prop.PropertyType == typeof(float))
                                    prop.SetValue(TargetComponent, Convert.ToSingle(GUILayout.TextField(((float)val) + "")), null);
                                else if (prop.PropertyType == typeof(double))
                                    prop.SetValue(TargetComponent, Convert.ToDouble(GUILayout.TextField(((double)val) + "")), null);
                                else if (prop.PropertyType == typeof(string))
                                    prop.SetValue(TargetComponent, GUILayout.TextField((string)val), null);
                            }
                        }
                        else
                        {
                            GUILayout.Label(prop.Name + "(" + prop.PropertyType.Name + ") : __UNREADABLE", AllGUIStyle.DEFAULT_LABEL);
                        }
                    }
                    catch (Exception e)
                    {
                        //TODO:...Damn
                        this.InjectLogError("Something Went Wrong When Drawing Properties",e);
                    }
                    

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                if (GUILayout.Button("Close", GUILayout.Height(20))) Active = false;

            }, "Properties On " + TargetComponent.GetType().Name, AllGUIStyle.DEFAULT_WINDOW);
        }
        private void OnGUITransformPathInput()
        {
            TransformPath = GUI.TextField(AllRect.PathInputerRect, TransformPath);
        }
        
        public static void Activate(string path)
        {
            if (!Instance)
                return;
            Instance.TransformPath = path;
            Instance.Activate();
        }
    }
}
